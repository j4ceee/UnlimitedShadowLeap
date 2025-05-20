using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace UnlimitedShadowLeap;

public static class PluginInformation
{
    public const string PluginName = "Unlimited Shadow Leap";
    public const string PluginVersion = "1.0.0";
    public const string PluginGuid = "jacee.dev.UnlimitedShadowLeap";
}

[BepInPlugin(PluginInformation.PluginGuid, PluginInformation.PluginName, PluginInformation.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private Harmony Harmony { get; set; }
    public static ConfigEntry<int> ConfigDistance;
    public static ConfigEntry<bool> ConfigDisableBlockers;
    public static ConfigEntry<bool> ConfigDisableRestrictions;
    
    private void Awake() // Plugin startup logic
    {
        // config
        ConfigDistance = Config.Bind("Shadow Powers",
            "Power Distance",
            100,
            new ConfigDescription("The distance Aragami can use shadow powers (e.g. leap, create shadows, ...). Vanilla: 10", new AcceptableValueRange<int>(5, 400)));
        ConfigDisableBlockers = Config.Bind("Shadow Powers",
            "Disable Power Blockers",
            true,
            new ConfigDescription("When disabled ('true'), shadow powers won't be blocked by map borders. Vanilla: false", new AcceptableValueList<bool>(true, false)));
        ConfigDisableRestrictions = Config.Bind("Shadow Powers",
            "Disable Power Restrictions",
            true,
            new ConfigDescription("When disabled ('true'), shadow powers won't be blocked when falling or sliding. Vanilla: false", new AcceptableValueList<bool>(true, false)));
        
        // logging
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInformation.PluginGuid} is loaded!");
        
        // patches
        PatchAll();
    }
    
    private void PatchAll()
    {
        Logger?.LogDebug("Patching...");

        Harmony ??= new Harmony(PluginInformation.PluginGuid);

        try
        {
            Harmony.PatchAll();
            Logger?.LogDebug("Patched!");
        }
        catch (Exception e)
        {
            Logger?.LogError($"Failed to patch: {e}");
        }
    }
}
