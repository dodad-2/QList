using MelonLoader;

namespace QList;

internal static class PreferencesConfig
{
    public static string filePath { get; private set; }
    public static void SetFilePath(MelonMod mod)
    {
        filePath = $"{MelonLoader.Utils.MelonEnvironment.UserDataDirectory}/{mod.Info.Name}.cfg";
    }
}