using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace TheTailor.Minions;

/// <summary>
///     Remembers which minions already have a "move to front" queued so a double click cannot queue it twice.
/// </summary>
internal static class TailorMinionActionQueue
{
    private static readonly HashSet<uint> Queued = [];

    public static bool IsQueued(Creature minion)
    {
        return minion.CombatId is { } id && Queued.Contains(id);
    }

    public static void Release(uint actorCombatId)
    {
        Queued.Remove(actorCombatId);
    }

    public static void Clear()
    {
        Queued.Clear();
    }

    /// <summary>
    ///     Sends the action through the game's action queue so it is executed in sync for all players.
    /// </summary>
    public static bool TryEnqueue(Creature minion)
    {
        if (!CombatManager.Instance.IsInProgress || minion.CombatId is not { } id || minion.PetOwner == null)
            return false;

        var synchronizer = RunManager.Instance.ActionQueueSynchronizer;
        if (synchronizer.CombatState != ActionSynchronizerCombatState.PlayPhase || !Queued.Add(id)) return false;

        try
        {
            synchronizer.RequestEnqueue(new MoveTailorMinionToFrontGameAction(minion.PetOwner, id));
        }
        catch
        {
            Queued.Remove(id);
            throw;
        }

        return true;
    }
}

/// <summary>
///     Moves a Tailor minion to the front of its owner's minions.
/// </summary>
public sealed class MoveTailorMinionToFrontGameAction(Player owner, uint actorCombatId) : GameAction
{
    public override ulong OwnerId => owner.NetId;

    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    protected override async Task ExecuteAction()
    {
        try
        {
            var minion = owner.Creature.CombatState?.GetCreature(actorCombatId);
            if (minion is not { IsAlive: true, Monster: TailorMinion } || minion.PetOwner != owner)
            {
                Cancel();
                return;
            }

            await MoveToFront(new GameActionPlayerChoiceContext(this), minion);

            if (CombatManager.Instance.IsInProgress) await CombatManager.Instance.CheckWinCondition();
        }
        finally
        {
            TailorMinionActionQueue.Release(actorCombatId);
        }
    }

    private static async Task MoveToFront(PlayerChoiceContext choiceContext, Creature minion)
    {
        var player = minion.PetOwner!;
        var accessor = new PetsOrderAccessor(player);
        if (accessor.Pets == null || !accessor.Pets.Contains(minion) || accessor.Pets.Count <= 1) return;

        accessor.Pets.Remove(minion);
        accessor.Pets.Insert(0, minion);
        _ = MinionAnimCmd.Rearrange(duration: 0.5f);
        await CreatureCmd.TriggerAnim(minion, "cast", 0f);
        accessor.SetManualRearranged();
        PetOrderSnapshotManager.TakeSnapshot(player);

        await TailorMinionCmd.PutOstyAtBack(choiceContext, player);
    }

    public override INetAction ToNetAction()
    {
        return new NetMoveTailorMinionToFrontGameAction { ActorCombatId = actorCombatId };
    }

    public override string ToString()
    {
        return $"{nameof(MoveTailorMinionToFrontGameAction)} owner={OwnerId} actor={actorCombatId}";
    }
}

public struct NetMoveTailorMinionToFrontGameAction : INetAction
{
    public uint ActorCombatId;

    public GameAction ToGameAction(Player player)
    {
        return new MoveTailorMinionToFrontGameAction(player, ActorCombatId);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteUInt(ActorCombatId, 6);
    }

    public void Deserialize(PacketReader reader)
    {
        ActorCombatId = reader.ReadUInt(6);
    }

    public override string ToString()
    {
        return $"{nameof(NetMoveTailorMinionToFrontGameAction)} actor={ActorCombatId}";
    }
}

/// <summary>
///     Clicking a Tailor minion moves it to the front.
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
internal static class TailorMinionClickPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCreature __instance)
    {
        __instance.Hitbox.Connect(Control.SignalName.GuiInput,
            Callable.From<InputEvent>(inputEvent => OnGuiInput(__instance, inputEvent)));
    }

    private static void OnGuiInput(NCreature actorNode, InputEvent inputEvent)
    {
        var byMouse = inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouse && mouse.IsReleased();
        var byController = inputEvent is InputEventAction { Action: var action } actionEvent
                           && action == MegaInput.select
                           && actionEvent.IsPressed()
                           && actorNode.Hitbox.HasFocus();

        if (!byMouse && !byController) return;

        if (actorNode.Entity.Monster is not TailorMinion || NTargetManager.Instance.IsInSelection) return;

        // Ignore the release that just confirmed another creature's targeting.
        if (byMouse && NTargetManager.Instance.LastTargetingFinishedFrame == actorNode.GetTree().GetFrame()) return;

        TryMoveToFront(actorNode.Entity);
        actorNode.GetViewport().SetInputAsHandled();
    }

    private static void TryMoveToFront(Creature minion)
    {
        if (!minion.IsAlive || minion.CombatState == null) return;

        if (!CombatManager.Instance.IsInProgress || CombatManager.Instance.PlayerActionsDisabled) return;

        if (RunManager.Instance.ActionQueueSynchronizer.CombatState != ActionSynchronizerCombatState.PlayPhase) return;

        if (minion.PetOwner == null || !LocalContext.IsMe(minion.PetOwner) ||
            minion.CombatState.CurrentSide != minion.Side) return;

        if (TailorMinionActionQueue.IsQueued(minion)) return;

        TailorMinionActionQueue.TryEnqueue(minion);
    }
}
