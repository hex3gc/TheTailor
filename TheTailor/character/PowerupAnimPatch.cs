using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace TheTailor;

[HarmonyPatch]
internal static class PowerupAnimPatch
{
    [HarmonyPatch(typeof(CreatureCmd), "TriggerAnim")]
    internal static void Postfix(Creature creature, string triggerName, float waitTime)
    {
        if (triggerName != "PowerUp") return;
        var visuals = creature.GetCreatureNode().Visuals;
        var particles = visuals.GetChildrenRecursive<GpuParticles2D>();
        if (particles == null || particles.Count() <= 0) return;
        foreach (var particle in particles)
            if (particle.Name == "TailorStringParticles" || particle.Name == "TailorGlowParticles")
                particle.Emitting = true;
    }
}