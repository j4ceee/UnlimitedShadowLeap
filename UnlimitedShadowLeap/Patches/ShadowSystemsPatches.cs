using HarmonyLib;
using UnityEngine;

namespace UnlimitedShadowLeap.Patches;

[HarmonyPatch(typeof(ShadowSystemsController))]
public class ShadowSystemsPatches
{
    [HarmonyPatch("CheckPowerValidDestiny")]
    [HarmonyPrefix]
    public static bool CheckPowerValidDestiny_Patch(ShadowSystemsController __instance, Vector3 origin, Vector3 direction, float magnitude, ref RaycastHit hitInfo, ref int __result)
    {
        //Plugin.Logger.LogInfo($"Plugin {PluginInformation.PLUGIN_GUID}: CheckPowerValidDestiny patched with unlimited distance");
        
        Vector3 vector = __instance.playerController.transform.position - __instance.playerController.targetCamera.transform.position;
        Vector3 from = Vector3.Project(vector, __instance.playerController.targetCamera.transform.forward);
        
        // Use a custom magnitude for the raycast
        float newDistance = Plugin.ConfigDistance.Value + from.magnitude;
        
        // Perform the raycast with the increased magnitude
        if (!Physics.Raycast(origin, direction, out hitInfo, newDistance, LDK.GetCollisionLayerMask(Constants.COL_LAYER_MASKS.POWER_DESTINATION)) || 
            !(hitInfo.transform != __instance.transform))
        {
            __result = 0;
            return false; // Skip original method
        }

        if (!Plugin.ConfigDisableBlockers.Value)
        {
            __instance.powerBlockerFound = Physics.Raycast(origin, direction, out RaycastHit hitPowerBlocker,
                                               newDistance,
                                               LDK.GetCollisionLayerMask(Constants.COL_LAYER_MASKS.POWER_BLOCKER)) &&
                                           hitPowerBlocker.transform != __instance.transform;
            
            if (__instance.powerBlockerFound && hitPowerBlocker.distance < hitInfo.distance)
            {
                __result = -1;
                return false; // Skip original method
            }
        }
        else
        {
            __instance.powerBlockerFound = false;
        }
        
        __result = 1;
        return false; // Skip original method
    }
}