using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace QList;

internal static class Resources // TODO rewrite this
{
    //private static Dictionary<string, Sprite> bundleSprites;
    internal static Dictionary<string, Texture2D>? BundleTextures;
    internal static Il2CppAssetBundle? Bundle;

    #region Variables
    private static Dictionary<string, Il2CppAssetBundle> BundleHash { get; set; }
    #endregion

    #region Bundles
    /// <summary>
    /// Makes bundles available to other mods
    /// </summary>
    public static bool RegisterBundle(Assembly assembly, string name)
    {
        if (BundleHash == null)
            BundleHash = new Dictionary<string, Il2CppAssetBundle>();

        if (BundleHash.ContainsKey(name))
        {
            Log.LogOutput($"Bundle '{name}' already loaded", Log.LogLevel.Warning);
            return false;
        }

        try
        {
            Il2CppAssetBundle bundle = null;

            Log.LogOutput($"Loading '{name}' from assembly '{assembly.FullName}'", Log.LogLevel.Debug);

            foreach (string fileName in assembly.GetManifestResourceNames())
                Log.LogOutput($"Resource name: {fileName}", Log.LogLevel.Debug);

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
                Log.LogOutput($"Unable to load '{name}'.", Log.LogLevel.Warning);
            }
            else
            {
                BundleHash.Add(name, bundle);
                Log.LogOutput($"Registered bundle '{name}'", Log.LogLevel.Info);
            }
        }
        catch (Exception e)
        {
            Log.LogOutput(e, Log.LogLevel.Error);
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
        if (!RegisterBundle(System.Reflection.Assembly.GetExecutingAssembly(), Mod.bundleKey))
        {
            Log.LogOutput($"Unable to initialize resources: Cannot register bundle", Log.LogLevel.Error);
            return;
        }

        Bundle = GetBundle(Mod.bundleKey);

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
            Log.LogOutput($"No textures loaded", Log.LogLevel.Debug);
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
    private static void CreateSpritesFromBundle()
    {
        /*
        if (bundle == null)
        {
            Log($"Unable to create sprites: Bundle is null", QLibrary.Log.LogLevel.Warning);
            return;
        }

        Sprite[] sprites = null;

        var textures = bundle.LoadAllAssets<Texture2D>();

        if (textures.Length == 0)
        {
            Log($"Unable to create sprites: No textures found in '{bundle.mainAsset.name}'", QLibrary.Log.LogLevel.Message);
            return;
        }

        sprites = new Sprite[textures.Length];

        for (int e = 0; e < textures.Length; e++)
        {
            sprites[e] = Sprite.Create(textures[e], new Rect(Vector2.zero, new Vector2(textures[e].width, textures[e].height)), Vector2.zero);
            sprites[e].name = textures[e].name;
        }

        bundleSprites = new();

        int idAppend = 0;
        string key;

        foreach (var sprite in sprites)
        {
            key = sprite.name;

            while (bundleSprites.ContainsKey(key))
            {
                idAppend++;
                key = $"{key}{idAppend}";
            }

            bundleSprites.Add(key, sprite);

            Log($"Loaded Sprite '{key}'");
        }*/
    }
    #endregion

    #region Access
    /// <summary>
    /// Deprecated
    /// </summary>
    public static Sprite GetSprite(string name)
    {
        return null;
        /*
        if (bundleSprites == null)
            Initialize();

        if (bundleSprites == null || !bundleSprites.ContainsKey(name))
            return null;

        return bundleSprites[name];
        */
    }
    public static Texture2D? GetTexture(string name)
    {
        if (BundleTextures == null || !BundleTextures.ContainsKey(name))
            return null;

        return BundleTextures[name];
    }
    #endregion

    #region Helpers
    public static Image? CreateImageFromTexture(string name, Texture2D texture)
    {
        if (texture == null)
        {
            Log.LogOutput($"Unable to create image from sprite with uid '{name}'");
            return null;
        }

        var imageObject = new GameObject($"(Image) {name}");

        var imageComponent = imageObject.AddComponent<Image>();
        imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f, 100f, 0);

        GameObject.DontDestroyOnLoad(imageObject);

        imageObject.SetActive(false);

        return imageComponent;
    }
    #endregion
}