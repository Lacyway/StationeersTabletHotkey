using System.Reflection;
using Assets.Scripts;
using Assets.Scripts.Inventory;
using Assets.Scripts.Objects.Entities;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.UI;
using HarmonyLib;
using InputSystem;
using StationeersTabletHotkey.Utils;
using UnityEngine;

namespace StationeersTabletHotkey.Patches;

public static class KeybindState
{
    public static readonly KeyWrap ChangeAdvTabletMode = new(KeyCode.Keypad0);
    public static KeyCode KeyCode = KeyCode.Keypad0;

    public static void ChangeMode()
    {
        STH_Plugin.MainLogger.LogInfo("Running ChangeMode");
        var localHuman = Human.LocalHuman;
        if (localHuman == null)
        {
#if DEBUG
            STH_Plugin.MainLogger.LogInfo("Could not find LocalHuman"); 
#endif
            return;
        }

        if (localHuman.IsUnresponsive || localHuman.IsSleeping || InputMouse.IsMouseControl || InputWindowBase.IsInputWindow)
        {
#if DEBUG
            STH_Plugin.MainLogger.LogInfo("Failed Responsive check"); 
#endif
            return;
        }

        var slot = InventoryManager.ActiveHandSlot;
        var advTablet = slot.Get<AdvancedTablet>();
        if (advTablet == null)
        {
#if DEBUG
            STH_Plugin.MainLogger.LogInfo("Item was not AdvTablet"); 
#endif
            return;
        }

        if (GameManager.RunSimulation)
        {
#if DEBUG
            STH_Plugin.MainLogger.LogInfo("Running on Server"); 
#endif
            OnServer.InteractWith(advTablet.InteractButton1, default);
        }
        else
        {
#if DEBUG
            STH_Plugin.MainLogger.LogInfo("Running on Client"); 
#endif
            NetworkClient.InteractWith(advTablet.InteractButton1, default);
        }
    }
}

[HarmonyPatch]
public static class Patch_SetBindings
{
    [HarmonyTargetMethod]
    public static MethodBase TargetMethod() => AccessTools.Method(typeof(KeyManager), "SetBindings");

    [HarmonyPostfix]
    public static void Postfix()
    {
        KeybindState.ChangeAdvTabletMode.Bind(InputPhase.Down, KeybindState.ChangeMode, KeyInputState.Game);
        KeybindState.ChangeAdvTabletMode.AssignKey(KeybindState.KeyCode);
    }
}

[HarmonyPatch]
public static class Patch_SetupKeyBindings
{
    private static readonly MethodInfo _addKeyInfo = AccessTools.Method(typeof(KeyManager), "AddKey");

    [HarmonyTargetMethod]
    public static MethodBase TargetMethod() => AccessTools.Method(typeof(KeyManager), "SetupKeyBindings");

    [HarmonyPostfix]
    public static void Postfix()
    {
        var group = KeyManager.GetControlsGroup("Interaction");
        if (group == null)
        {
            STH_Plugin.MainLogger.LogError("Could not find group [Interaction]");
            return;
        }

        _addKeyInfo?.Invoke(null, ["ChangeTabletMode", KeybindState.KeyCode, group, false]);
        ControlsAssignment.RefreshState();
    }
}

[HarmonyPatch]
public static class Patch_LoadKeyboardSetting
{
    [HarmonyTargetMethod]
    public static MethodBase TargetMethod() => AccessTools.Method(typeof(KeyManager), "LoadKeyboardSetting");

    [HarmonyPrefix]
    public static void Prefix()
    {
        KeybindState.KeyCode = KeyManager.GetKey("ChangeTabletMode");
        KeybindState.ChangeAdvTabletMode.AssignKey(KeybindState.KeyCode);
    }
}