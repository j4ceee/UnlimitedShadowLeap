using HarmonyLib;

namespace UnlimitedShadowLeap.Patches;

[HarmonyPatch(typeof(GlobalSettings))]
internal static class GlobalPatches
{
    [HarmonyPatch(nameof(GlobalSettings.Reset))]
    [HarmonyPostfix]
    public static void Reset_Patch(GlobalSettings __instance)
    {
        __instance.ARAGAMI_DEBUG.GOD_MODE = true;
    }
}