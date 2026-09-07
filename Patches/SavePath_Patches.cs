#if DEBUG
using HarmonyLib;
using Networking.Servers;

namespace StationeersTabletHotkey.Patches;

public static class SavePath_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StationSaveUtils), nameof(StationSaveUtils.DefaultPath), MethodType.Getter)]
    public static bool Prefix(ref string __result)
    {
        __result = StationSaveUtils.ExeDirectory.FullName;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerCookie), "PATH_VERSIONED", MethodType.Getter)]
    public static bool Get_PATH_VERSIONED_Prefix(ref string __result)
    {
        __result = Path.Combine(StationSaveUtils.ExeDirectory.FullName, "PlayerCookie-v2.xml");
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerCookie), "PATH_NONVERSIONED", MethodType.Getter)]
    public static bool Get_PATH_NONVERSIONED_Prefix(ref string __result)
    {
        __result = Path.Combine(StationSaveUtils.ExeDirectory.FullName, "PlayerCookie.xml");
        return false;
    }
}
#endif