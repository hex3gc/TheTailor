using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace TheTailor.Minions;

/// <summary>
///     Summoning of Tailor minions (vanilla pets).
/// </summary>
public static class MinionCmd
{
    public static async Task<Creature> AddMinion<T>(PlayerChoiceContext choiceContext, Player player,
        MinionSummonOptions options = default)
        where T : TailorMinion
    {
        ArgumentNullException.ThrowIfNull(player);

        var pet = await PlayerCmd.AddPet<T>(player);
        PetOrderSnapshotManager.TakeSnapshot(player);

        if (pet.Monster is TailorMinion minion) await minion.OnSummon(choiceContext, player, options);

        _ = MinionAnimCmd.Rearrange();

        return pet;
    }
}

/// <summary>
///     Remembers the order of a player's pets by combat id. Used for the overkill chain and for reordering.
/// </summary>
public static class PetOrderSnapshotManager
{
    private static readonly object Sync = new();
    private static ConditionalWeakTable<Player, List<uint>> _snapshots = new();

    public static void TakeSnapshot(Player player)
    {
        if (player.PlayerCombatState == null) return;

        var ids = player.PlayerCombatState.Pets
            .Where(p => p.CombatId.HasValue)
            .Select(p => p.CombatId!.Value)
            .ToList();

        lock (Sync)
        {
            _snapshots.Remove(player);
            _snapshots.Add(player, ids);
        }
    }

    public static IReadOnlyList<Creature> GetSnapshot(Player player, bool onlyAlive = true, bool includeMissing = true)
    {
        if (player.PlayerCombatState == null) return [];

        var currentPets = player.PlayerCombatState.Pets;
        var combatState = player.Creature.CombatState;

        List<uint> orderedIds;
        lock (Sync)
        {
            orderedIds = _snapshots.TryGetValue(player, out var ids) ? [.. ids] : [];
        }

        if (includeMissing)
        {
            HashSet<uint> known = [.. orderedIds];
            foreach (var pet in currentPets)
                if (pet.CombatId.HasValue && known.Add(pet.CombatId.Value))
                    orderedIds.Add(pet.CombatId.Value);
        }

        List<Creature> result = [];
        HashSet<uint> added = [];
        foreach (var id in orderedIds)
        {
            if (!added.Add(id)) continue;

            var pet = combatState?.GetCreature(id) ?? currentPets.FirstOrDefault(p => p.CombatId == id);
            if (pet == null || (onlyAlive && !pet.IsAlive)) continue;

            result.Add(pet);
        }

        if (includeMissing)
            foreach (var pet in currentPets)
                if (!pet.CombatId.HasValue && (!onlyAlive || pet.IsAlive))
                    result.Add(pet);

        return result;
    }

    public static void ClearAllSnapshots()
    {
        lock (Sync)
        {
            _snapshots = new ConditionalWeakTable<Player, List<uint>>();
        }
    }
}

/// <summary>
///     Gives access to the raw pet list of a player so it can be reordered. Index 0 is the front-most pet.
///     Disposing it takes a new order snapshot and (unless <see cref="SetManualRearranged" /> was called)
///     animates the minions to their new positions.
/// </summary>
public class PetsOrderAccessor(Player player) : IDisposable
{
    private readonly int _count = GetRawPetsList(player)?.Count ?? 0;
    public readonly List<Creature>? Pets = GetRawPetsList(player);
    private bool _manualRearranged;

    public void Dispose()
    {
        if ((Pets?.Count ?? 0) != _count)
            throw new InvalidOperationException(
                "PetsOrderAccessor should not be used for operations other than reordering");

        PetOrderSnapshotManager.TakeSnapshot(player);
        if (!_manualRearranged) _ = MinionAnimCmd.Rearrange();

        GC.SuppressFinalize(this);
    }

    public void SetManualRearranged(bool value = true)
    {
        _manualRearranged = value;
    }

    public static List<Creature>? GetRawPetsList(Player player)
    {
        return (List<Creature>?)player.PlayerCombatState?.Pets;
    }
}

public readonly record struct MinionNodePosition(NCreature Node, Vector2 Position);

/// <summary>
///     Places Tailor minions in a row in front of their owner, front-most pet first.
/// </summary>
public static class MinionLayout
{
    private const float SpacingX = 150f * 1.6f;
    private const float OffsetX = 200f;

    public static bool IsMinionNode(NCreature node)
    {
        return node.Entity is { Monster: TailorMinion, IsAlive: true, PetOwner: not null };
    }

    public static IReadOnlyList<MinionNodePosition> GetCurrentPositions(NCombatRoom room)
    {
        return room.CreatureNodes.Where(IsMinionNode).Select(n => new MinionNodePosition(n, n.Position)).ToList();
    }

    public static IReadOnlyList<MinionNodePosition> Calculate(NCombatRoom room)
    {
        List<MinionNodePosition> result = [];
        foreach (var group in room.CreatureNodes.Where(IsMinionNode).GroupBy(n => n.Entity.PetOwner!))
        {
            var ownerNode = room.GetCreatureNode(group.Key.Creature);
            if (ownerNode == null || group.Key.PlayerCombatState == null) continue;

            var nodes = group.ToDictionary(n => n.Entity, n => n);
            var ordered = group.Key.PlayerCombatState.Pets
                .Select(p => nodes.GetValueOrDefault(p))
                .OfType<NCreature>()
                .ToList();

            var xs = GridXs(ordered.Count);
            for (var i = 0; i < ordered.Count; i++)
                result.Add(new MinionNodePosition(ordered[i],
                    new Vector2(xs[i] * SpacingX + OffsetX + ownerNode.Position.X, ownerNode.Position.Y)));
        }

        return result;
    }

    /// <summary>
    ///     X coordinates (in units of minion spacing) of a row of <paramref name="count" /> minions; the first entry is
    ///     the front-most one. The row stretches with the minion count but flattens out when it gets long.
    /// </summary>
    private static IReadOnlyList<float> GridXs(int count)
    {
        if (count <= 0) return [];

        const float turningPoint = 3f;
        const float slope = 0.75f;
        var fullWidth = (count + 1) * 0.5f;
        var width = fullWidth <= turningPoint
            ? fullWidth
            : turningPoint + slope * MathF.Log((fullWidth - turningPoint) / slope + 1);

        return Enumerable.Range(2, count).Select(i => float.Lerp(width, 0f, (float)i / (count + 1))).ToList();
    }
}

public static class MinionAnimCmd
{
    private static Tween? _activeTween;

    public static async Task Rearrange(bool animated = true, float duration = 0.25f)
    {
        var room = NCombatRoom.Instance;
        if (room == null) return;

        var destination = MinionLayout.Calculate(room);
        if (animated)
            await AnimatedMove(destination, duration);
        else
            InstantMove(destination);
    }

    public static void InstantMove(IEnumerable<MinionNodePosition> nodePositions)
    {
        foreach (var (node, position) in nodePositions)
            if (GodotObject.IsInstanceValid(node))
                node.Position = position;
    }

    public static async Task AnimatedMove(IEnumerable<MinionNodePosition> nodePositions, float duration = 0.25f)
    {
        var room = NCombatRoom.Instance;
        if (room == null) return;

        var valid = nodePositions.Where(p => GodotObject.IsInstanceValid(p.Node)).ToList();

        // Finish the previous movement first so anything awaiting it resumes.
        var previous = _activeTween;
        _activeTween = null;
        if (previous != null && previous.IsValid())
        {
            previous.EmitSignal(Tween.SignalName.Finished);
            previous.Kill();
        }

        if (valid.Count == 0) return;

        var tween = room.CreateTween();
        tween.SetParallel();
        foreach (var (node, position) in valid)
            tween.TweenProperty(node, "position", position, duration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);

        _activeTween = tween;
        await room.ToSignal(tween, Tween.SignalName.Finished);

        if (_activeTween == tween) _activeTween = null;
    }
}

/// <summary>
///     Subscribes to combat events to keep minions arranged and to drop stale pet order snapshots.
/// </summary>
public static class MinionHooks
{
    public static void Initialize()
    {
        CombatManager.Instance.TurnStarted += OnTurnStarted;
        CombatManager.Instance.TurnEnded += OnTurnEnded;
        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
        CombatManager.Instance.CombatEnded += OnCombatEnded;
    }

    private static void OnTurnStarted(CombatState combatState)
    {
        if (combatState.CurrentSide == CombatSide.Player) _ = MinionAnimCmd.Rearrange();
    }

    private static void OnTurnEnded(CombatState combatState)
    {
        TailorMinionActionQueue.Clear();

        // CurrentSide has already switched when TurnEnded fires.
        if (combatState.CurrentSide == CombatSide.Enemy) _ = MinionAnimCmd.Rearrange();
    }

    private static void OnCombatSetUp(CombatState combatState)
    {
        TailorMinionActionQueue.Clear();
        PetOrderSnapshotManager.ClearAllSnapshots();
    }

    private static void OnCombatEnded(CombatRoom combatRoom)
    {
        TailorMinionActionQueue.Clear();
        PetOrderSnapshotManager.ClearAllSnapshots();
    }
}

/// <summary>
///     Keeps local minions clickable even when the game toggles interactability.
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.ToggleIsInteractable))]
internal static class MinionInteractablePatch
{
    [HarmonyPrefix]
    private static void Prefix(NCreature __instance, ref bool on)
    {
        if (__instance.Entity.Monster is TailorMinion && LocalContext.IsMe(__instance.Entity.PetOwner)) on = true;
    }
}

/// <summary>
///     NCombatRoom.AddCreature re-spreads all pets of the owner. Restore the minions' positions afterwards and
///     start new minions at their owner so they slide into place.
/// </summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
internal static class MinionAddCreaturePatch
{
    [HarmonyPrefix]
    private static void Prefix(NCombatRoom __instance, out IReadOnlyList<MinionNodePosition> __state)
    {
        __state = MinionLayout.GetCurrentPositions(__instance);
    }

    [HarmonyPostfix]
    private static void Postfix(NCombatRoom __instance, Creature creature, IReadOnlyList<MinionNodePosition> __state)
    {
        MinionAnimCmd.InstantMove(__state);

        if (creature.PetOwner == null || creature.Monster is not TailorMinion) return;

        var node = __instance.GetCreatureNode(creature);
        var ownerNode = __instance.GetCreatureNode(creature.PetOwner.Creature);
        if (node != null && ownerNode != null) node.Position = ownerNode.Position;
    }
}

/// <summary>
///     The game only removes dead enemies from the combat; remove dead friendly minions as well.
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), "KillWithoutCheckingWinCondition")]
internal static class MinionKillPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Task __result, Creature creature)
    {
        __result = AwaitAndCleanupAsync(__result, creature);
    }

    private static async Task AwaitAndCleanupAsync(Task originalTask, Creature creature)
    {
        await originalTask;

        if (creature is { Side: CombatSide.Player, IsPet: true, Monster: TailorMinion, CombatState: not null })
        {
            var combatState = creature.CombatState;
            if (combatState.Creatures.Contains(creature) &&
                Hook.ShouldCreatureBeRemovedFromCombatAfterDeath(combatState, creature))
            {
                CombatManager.Instance?.RemoveCreature(creature);
                combatState.RemoveCreature(creature);
            }
        }
    }
}

/// <summary>
///     PersonalHivePower only expects Osty as a pet dealer; attribute minion damage to the owner instead of crashing.
/// </summary>
[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
internal static class MinionPersonalHivePatch
{
    [HarmonyPrefix]
    private static void Prefix(ref Creature? dealer)
    {
        if (dealer is { Monster: TailorMinion, PetOwner.Creature: not null })
            dealer = dealer.PetOwner.Creature;
    }
}