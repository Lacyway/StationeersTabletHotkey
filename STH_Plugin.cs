using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace StationeersTabletHotkey;

[BepInPlugin("lcw.lacyway.sth", "StationeersTabletHotkey", PluginVersion)]
public sealed class STH_Plugin : BaseUnityPlugin
{
    public static ManualLogSource MainLogger => _instance.Logger;
    public const string PluginVersion = "1.0.0";

    private static STH_Plugin _instance;
    private Harmony _harmony;

    private void Awake()
    {
        _instance = this;

        _harmony = new Harmony("lcw.lacyway.sth");
        _harmony.PatchAll();

        Logger.LogInfo("Successfully applied all patches!");
        Logger.LogInfo($"{nameof(STH_Plugin)} v{PluginVersion} has been loaded.");
    }
}