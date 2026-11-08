using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MinionLib.Layout;
using MinionLib.Minion;
using static MinionLib.Layout.DefaultMinionLayout;

namespace TheTailor.Minions;

[HarmonyPatch]
internal static class MinionLayoutPatch
{
    /// <summary>
    ///     Replaces the 'generate grid points' function in cases where the given creature is a Tailor minion
    /// </summary>
    public static IReadOnlyList<Vector2> TailorMinionGenerateGridPoints(MinionPosition position, int count)
    {
        var list = DefaultMinionLayout.GenerateGridPoints(position, count);
        List<Vector2> newList = new();
        foreach (var vector in list)
        {
            newList.Add(new Vector2((vector.X * 1.6f) - 0f, 0f));
        }
        return newList;
    }

    /*
    [HarmonyPatch(typeof(DefaultMinionLayout), "GenerateGridPoints")]
    internal static void Postfix(ref IReadOnlyList<Vector2> __result, DefaultMinionLayout __instance)
    {
        // TODO does not discriminate between The Tailor and other characters' minions if they're also using MinionLib
        // - May have to request this as a feature to MinionLib, or create a separate patch/singleton

        List<Vector2> newList = new();
        foreach (var vector in __result)
        {
            newList.Add(new Vector2((vector.X * 1.6f) - 0f, 0f));
        }

        __result = newList;
    }
    */

    [HarmonyPatch(typeof(DefaultMinionLayout), "CalculateMinionPositions")]
    internal static bool Prefix(ref IReadOnlyList<MinionNodePosition> __result, NCombatRoom room, IEnumerable<NCreature> unhandledMinions)
    {
        __result = GetMinionOwnerNodePairs(room, unhandledMinions).SelectMany(delegate(OwnerWithMinionsNodes pair)
        {
            OwnerWithMinionsNodes ownerWithMinionsNodes = pair;
            ownerWithMinionsNodes.Deconstruct(out NCreature Owner, out IReadOnlyList<NCreature> Minions);
            NCreature ownerNode = Owner;
            IReadOnlyList<NCreature> source = Minions;
            ILookup<MinionPosition, NCreature> grouped = source.ToLookup((NCreature c) => ((MinionModel) c.Entity.Monster!).Position);
            return grouped.SelectMany(delegate(IGrouping<MinionPosition, NCreature> g)
            {
                MinionPosition key = g.Key;
                Vector2 offset = CalculateBaseOffset(key, grouped);

                IEnumerable<Vector2> second;
                if (key == MinionPosition.Front && g.Count(tm => tm.Entity.Monster is TailorMinion) > 0)
                {
                    second = from v in TailorMinionGenerateGridPoints(key, g.Count())
                        select v * MinionSize + offset + ownerNode.Position;
                }
                else
                {
                    second = from v in GenerateGridPoints(key, g.Count())
                        select v * MinionSize + offset + ownerNode.Position;
                }

                return g.Zip(second, (NCreature node, Vector2 position) => new MinionNodePosition(node, position));
            });
        }).ToList();
        return false;
    }
}
