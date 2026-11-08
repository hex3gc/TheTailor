using System.Reflection;
using BaseLib.Config;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using TheTailor.Config;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace TheTailor;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "TheTailor";
    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new TheTailorConfig());
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
    }
}