using HarmonyLib;
using InputSystem;

namespace StationeersTabletHotkey.Utils;

public static class KeyWrapBindingsHelper
{
    private delegate void BindDelegate(KeyWrap keyWrap, InputPhase phase, Action callback, KeyInputState inputState);

    private static readonly BindDelegate _cachedBind;

    static KeyWrapBindingsHelper()
    {
        var keyWrapBindingsType = AccessTools.TypeByName("InputSystem.KeyWrapBindings");

        if (keyWrapBindingsType == null)
        {
            STH_Plugin.MainLogger.LogError("Failed to resolve [InputSystem.KeyWrapBindings] type.");
            return;
        }

        var method = AccessTools.Method(keyWrapBindingsType, "Bind",
            [typeof(KeyWrap), typeof(InputPhase), typeof(Action), typeof(KeyInputState)]);

        if (method == null)
        {
            STH_Plugin.MainLogger.LogError("Failed to locate [KeyWrapBindings.Bind] method.");
            return;
        }

        _cachedBind = AccessTools.MethodDelegate<BindDelegate>(method);
    }

    public static void Bind(this KeyWrap keyWrap, InputPhase phase, Action callback, KeyInputState inputState = KeyInputState.All)
    {
        if (_cachedBind == null)
        {
            STH_Plugin.MainLogger.LogError("Cached bind delegate is null. Cannot invoke Bind.");
            return;
        }

        _cachedBind(keyWrap, phase, callback, inputState);
    }
}