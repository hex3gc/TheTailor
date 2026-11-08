using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheTailor.Minions;

[HarmonyPatch(typeof(Creature), nameof(Creature.LoseHpInternal), typeof(decimal), typeof(ValueProp))]
public static class MinionGuardianOwnerDamageSuppressPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Creature __instance, decimal amount, ValueProp props, ref DamageResult __result)
    {
        var suppressedOwner = TailorMinionOverkillPatch.SuppressedOwner.Value;
        if (suppressedOwner == null || __instance != suppressedOwner || amount <= 0m) return true;

        __result = new DamageResult(__instance, props);
        return false;
    }
}