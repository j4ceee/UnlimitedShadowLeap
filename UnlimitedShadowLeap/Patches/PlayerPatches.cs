using HarmonyLib;
using UnityEngine;

namespace UnlimitedShadowLeap.Patches;

[HarmonyPatch(typeof(PlayerController))]
internal static class PlayerPatches
{
    /*
    [HarmonyPatch("CheckGroundLeap")]
    [HarmonyPrefix]
    public static bool CheckGroundLeap_Patch(PlayerController __instance, ref bool __result, ref Vector3 controlPoint, RaycastHit rayControlPoint, Vector3 d)
    {
        //Plugin.Logger.LogInfo($"Plugin {PluginInformation.PLUGIN_GUID}: CheckGroundLeap");

        // Set the teleport type and animator bool that the original method would set
        __instance.teleportType = PlayerController.TeleportType.Ground;
        __instance.animator.SetBool("AerialLocation", false);

        // Set the control point to the hit point
        controlPoint = rayControlPoint.point;

        // Call CheckReposition and return its result
        MethodInfo repoMethod = __instance.GetType().GetMethod("CheckReposition", BindingFlags.NonPublic | BindingFlags.Instance);
        __result = (bool)repoMethod.Invoke(__instance, new object[] { rayControlPoint, d });

        // Skip the original method
        return false;
    }
    */
    
    [HarmonyPatch("EnableLeapRange")]
    [HarmonyPostfix]
    public static void EnableLeapRange_Patch(PlayerController __instance, bool b)
    {
        if (b)
        {
            Vector3 vector = __instance.transform.position - __instance.targetCamera.transform.position;
            Vector3 from = Vector3.Project(vector, __instance.targetCamera.transform.forward);
            float radius = (Plugin.ConfigDistance.Value + from.magnitude) * 1.6f;

            __instance.effectLeapRangeInstance.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
    
    [HarmonyPatch(nameof(PlayerController.CheckTeleport))]
    [HarmonyPrefix]
    public static bool CheckTeleport_Patch(PlayerController __instance)
    {
        if (!Plugin.ConfigDisableRestrictions.Value)
        {
            return true; // execute vanilla function
        }
        
        GameManager gm = (GameManager)AccessTools.Field(typeof(PlayerController), "gm").GetValue(__instance);
        CameraSettings cameraSettings = (CameraSettings)AccessTools.Field(typeof(PlayerController), "cameraSettings").GetValue(__instance);
        
        if (gm.InsideMenuThatStopsGameplay() || gm.gameCinematic || gm.gameMovie || cameraSettings.isPhotoMode || gm.consoleActive || !__instance.isMine)
        {
            return false;
        }
        
        __instance.canLeapAndLand = false;
        
        if (!__instance.shadowSystemsController.canTeleport)
        {
            return false;
        }
        
        __instance.canLeapAndLand = true;
        return false;
    }
}