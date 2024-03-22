using MelonLoader;

namespace QList;

internal static class Config
{
    public static string FilePath { get; private set; } = "";

    public static void SetFilePath(MelonMod mod)
    {
        FilePath = $"{MelonLoader.Utils.MelonEnvironment.UserDataDirectory}/{mod.Info.Name}.cfg";
    }
}
