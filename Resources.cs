using System.Reflection;
using UnityEngine;

namespace QList;

internal static class Resources
{
    internal static Dictionary<string, Texture2D>? BundleTextures;
    internal static Il2CppAssetBundle? Bundle;

    #region Variables
    private static Dictionary<string, Il2CppAssetBundle> BundleHash { get; set; }
    #endregion

    #region Bundles
    /// <summary>
    /// This feature has been moved to QAPI but remains here to allow QList to run independently
    /// </summary>
    public static bool RegisterBundle(Assembly assembly, string name)
    {
        if (BundleHash == null)
            BundleHash = new Dictionary<string, Il2CppAssetBundle>();

        if (BundleHash.ContainsKey(name))
        {
            Log.LogOutput($"Bundle '{name}' already loaded", Log.ELevel.Warning);
            return false;
        }

        try
        {
            Il2CppAssetBundle? bundle = null;

            Log.LogOutput(
                $"Loading '{name}' from assembly '{assembly.FullName}'",
                Log.ELevel.Debug
            );

            foreach (string fileName in assembly.GetManifestResourceNames())
                Log.LogOutput($"Resource name: {fileName}", Log.ELevel.Debug);

            MemoryStream memoryStream;

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, buffer.Length);
            }

            bundle = Il2CppAssetBundleManager.LoadFromMemory(memoryStream.ToArray());

            if (bundle == null)
            {
                Log.LogOutput($"Unable to load '{name}'.", Log.ELevel.Warning);
            }
            else
            {
                BundleHash.Add(name, bundle);
                Log.LogOutput($"Registered bundle '{name}'", Log.ELevel.Info);
            }
        }
        catch (Exception e)
        {
            Log.LogOutput(e, Log.ELevel.Error);
            return false;
        }

        return true;
    }

    public static Il2CppAssetBundle GetBundle(string key)
    {
        return BundleHash[key];
    }
    #endregion
    public static void Initialize()
    {
        if (!RegisterBundle(System.Reflection.Assembly.GetExecutingAssembly(), QListMod.BundleKey))
        {
            Log.LogOutput(
                $"Unable to initialize resources: Cannot register bundle",
                Log.ELevel.Error
            );
            return;
        }

        Bundle = GetBundle(QListMod.BundleKey);

        if (Bundle == null)
        {
            Log.LogOutput($"Unable to initialize resources: Bundle is null");
            return;
        }

        LoadTexturesFromBundle();
    }

    #region Creation
    private static void LoadTexturesFromBundle()
    {
        if (Bundle == null)
        {
            Log.LogOutput($"Unable to load textures: bundle is null");
            return;
        }

        var textures = Bundle.LoadAllAssets<Texture2D>();

        if (textures == null || textures.Length == 0)
        {
            Log.LogOutput($"No textures loaded", Log.ELevel.Debug);
            return;
        }

        BundleTextures = new();
        string uid;
        int id;

        foreach (var texture in textures)
        {
            uid = texture.name;
            id = 1;

            while (BundleTextures.ContainsKey(uid))
            {
                uid = $"{texture.name}{id}";
                id++;
            }

            BundleTextures.Add(uid, texture);
            Log.LogOutput($"Loaded texture '{uid}'");
        }
    }
    #endregion
}
