

/*
namespace TheTailor
{
    public static class Debug
    {
        [HarmonyPatch(typeof(LocTable))]
        internal static class LocStringPatch
        {
            [HarmonyPatch("GetLocString")]
            [HarmonyPrefix]
            public static bool Prefix(LocTable __instance, string key, string ____name, ref LocString __result)
            {
                Log.Warn($"GetLocString:'{key}' from '{____name}'");
                return true;
            }

            [HarmonyPatch("GetRawText")]
            [HarmonyPrefix]
            public static bool Prefix(LocTable __instance, string key, string ____name, ref string __result)
            {
                Log.Warn($"GetLocString:'{key}' from '{____name}'");
                return true;
            }
        }
    }
}
*/