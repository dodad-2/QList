using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Il2Cpp;
using HarmonyLib;
using Il2CppTMPro;

using Il2CppInterop.Runtime;

namespace QList.UI;

internal static class Manager
{
    #region Variables
    private const string modButtonText = "Mod Options";
    internal static GameObject Container
    {
        get
        {
            if (container == null)
                CreateContainer();

            return container;
        }
    }
    internal static Dictionary<string, Image>? ExistingImagePrefabHash = new();
    private static GameObject? container, mainMenuOptions, ingameOptions, ingameLogo;
    private static Button? mainMenuButton, pauseMenuButton, mainMenuBackButton, pauseMenuResumeButton;
    #endregion

    #region Init / Uninit
    internal static void Initialize()
    {
        Log.LogOutput($"Manager initializing..", Log.LogLevel.Info);
        CreateAssets();
        CreateModOptionsMenu();
    }
    internal static void Uninitialize()
    {
        // Necessary?
    }
    #endregion

    #region Assets
    private static void CreateAssets()
    {
        CreateImagePrefabsFromExisting();
    }
    private static void CreateImagePrefabsFromExisting()
    {
        ExistingImagePrefabHash = new();

        string[] textureNames = new string[]
        {
            "window_md",
        };

        var textures = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<Texture2D>());
        Image? current;

        if (textures == null)
        {
            Log.LogOutput("Unable to load sprites", Log.LogLevel.Error);
            return;
        }

        foreach (var textureResource in textures.ToArray().Where(x => x != null && textureNames.Contains(x.name)))
        {
            if (textureResource == null)
                continue;

            Texture2D? texture = textureResource.TryCast<Texture2D>();

            if (texture == null)
            {
                Log.LogOutput($"Unable to create sprite for {textureResource.name}", Log.LogLevel.Error);
                continue;
            }

            string uid = texture.name;

            int id = 1;

            while (ExistingImagePrefabHash.ContainsKey(uid))
            {
                uid = $"{uid}{id}";
                id++;
            }

            Log.LogOutput($"Creating Image prefab for {uid}", Log.LogLevel.Info);

            current = Resources.CreateImageFromTexture(uid, texture);

            if (current == null)
            {
                Log.LogOutput($"Unable to create image for '{uid}'", Log.LogLevel.Error);
                continue;
            }

            current.transform.SetParent(Container.transform);
            ExistingImagePrefabHash.Add(uid, current);
        }
    }
    #endregion

    #region Patches - UI
    /*
    [HarmonyPatch(typeof(Il2CppSilica.UI.MainMenu), nameof(Il2CppSilica.UI.MainMenu.Start))]
    private static class MainMenu_Start
    {
        private static void Postfix(Il2CppSilica.UI.MainMenu __instance)
        {
        }
    }
    */
    [HarmonyPatch(typeof(Il2CppSilica.UI.UIManager), nameof(Il2CppSilica.UI.UIManager.Awake))]
    private static class UIManager_Awake
    {
        private static void Postfix(Il2CppSilica.UI.UIManager __instance)
        {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("menu"))
                ModifyMainMenu(null); // Why pass null?
            else if (!SceneManager.GetActiveScene().name.ToLower().Contains("intro"))
                ModifyPauseMenu(null);
        }
    }
    #endregion

    #region UI
    public static void ToggleModOptionsMenu()
    {
        if (ModOptionsMenu.Instance == null)
            return;

        ModOptionsMenu.Toggle();

        if (Il2CppSilica.UI.MenuManager.Instance.IsMenuOpen(Il2CppSilica.UI.MenuType.Main))
            SetMainMenuOptionsVisibility(!ModOptionsMenu.Instance.gameObject.activeSelf);
        else
            SetIngameMenuOptionsVisibility(!ModOptionsMenu.Instance.gameObject.activeSelf);
    }

    private static void CreateModOptionsMenu()
    {
        if (Resources.Bundle == null)
        {
            Log.LogOutput($"Unable to create mod options menu: bundle is null", Log.LogLevel.Error);
            return;
        }

        var options = Resources.Bundle.LoadAsset<GameObject>("ModOptionsMenu.prefab");

        if (options == null)
        {
            Log.LogOutput($"Unable to create mod options menu: prefab not found", Log.LogLevel.Error);
            return;
        }

        options = GameObject.Instantiate(options);
        options.transform.SetParent(Container.transform, false);
    }
    private static void CreateContainer()
    {
        container = new GameObject("(QList) Manager Resources", Il2CppType.Of<RectTransform>());
        GameObject.DontDestroyOnLoad(container);
    }
    private static void ModifyPauseMenu(Il2CppSilica.UI.Menu __instance)
    {
        // TODO rewrite both of these methods

        var search = Il2CppSilica.UI.MenuManager.Instance.GetComponentsInChildren<VerticalLayoutGroup>(true).Where(x => x.name.ToLower().Equals("menu"));

        var pauseMenu = Il2CppSilica.UI.MenuManager.Instance.GetComponentInChildren<Il2CppSilica.UI.PauseMenu>(true);

        ingameOptions = pauseMenu.GetComponentInChildren<Il2CppSilica.UI.OptionsForm>(true).gameObject;

        var logoSearch = pauseMenu.GetComponentsInChildren<Image>(true).Where(x => x.name.Equals("Logo"));

        if (logoSearch != null && logoSearch.Count() != 0)
        {
            ingameLogo = logoSearch.First()?.gameObject;
        }

        if (search == null || search.Count() == 0)
        {
            Log.LogOutput($"Unable to create pause menu button: search is null or empty", Log.LogLevel.Error);
            return;
        }

        var leftHandMenu = search.First();

        if (leftHandMenu == null)
        {
            Log.LogOutput($"Unable to create pause menu button: leftHandMenu is null", Log.LogLevel.Error);
            return;
        }

        var search2 = leftHandMenu.gameObject.GetComponentsInChildren<Button>(true).Where(x => x.name.ToLower().Equals("resume"));

        if (search2 == null || search2.Count() == 0)
        {
            Log.LogOutput($"Unable to create pause menu button: search2 is null or empty", Log.LogLevel.Error);
            return;
        }

        pauseMenuResumeButton = search2.First();

        if (pauseMenuResumeButton == null)
        {
            Log.LogOutput($"Unable to create pause menu button: resumeButton is null", Log.LogLevel.Error);
            return;
        }

        pauseMenuResumeButton.onClick.AddListener(new Action(OnClickBackButton));

        var modOptionsButton = GameObject.Instantiate(pauseMenuResumeButton.gameObject).GetComponent<Button>();

        if (modOptionsButton == null)
        {
            Log.LogOutput($"Unable to create pause menu button: modsButton is null", Log.LogLevel.Error);
            return;
        }
        pauseMenuButton = modOptionsButton;

        modOptionsButton.transform.SetParent(leftHandMenu.transform);
        modOptionsButton.transform.SetSiblingIndex(2);

        var modsButtonRect = modOptionsButton.GetComponent<RectTransform>();

        FormatButton(modOptionsButton);
    }
    private static void ModifyMainMenu(Il2CppSilica.UI.Menu __instance)
    {
        var search = Il2CppSilica.UI.MenuManager.Instance.GetComponentsInChildren<Il2CppSilica.UI.MenuScreen>(true).Where(x => x.name.ToLower().Equals("options") && x.transform.parent.name.ToLower().Equals("main menu"));

        if (search == null || search.Count() == 0)
        {
            Log.LogOutput($"Unable to create main menu button: search is null or empty", Log.LogLevel.Error);
            return;
        }

        var optionsMenu = search.First();

        if (optionsMenu == null)
        {
            Log.LogOutput($"Unable to create main menu button: optionsMenu is null or empty", Log.LogLevel.Error);
            return;
        }

        mainMenuOptions = optionsMenu.gameObject.GetComponentInChildren<Il2CppSilica.UI.OptionsForm>().gameObject;

        var gearIconRotator = Il2CppSilica.UI.MenuManager.Instance.GetComponentInChildren<Il2CppSilica.UI.Effects.Rotator>();

        if (gearIconRotator != null)
        {
            var gearButton = gearIconRotator.GetComponent<Button>();
            gearButton.onClick.AddListener(new Action(OnClickBackButton));
        }

        var search2 = optionsMenu.gameObject.GetComponentsInChildren<Button>(true).Where(x => x.name.ToLower().Equals("back"));

        if (search2 == null || search2.Count() == 0)
        {
            Log.LogOutput($"Unable to create main menu button: search2 is null or empty", Log.LogLevel.Error);
            return;
        }

        mainMenuBackButton = search2.First();

        if (mainMenuBackButton == null)
        {
            Log.LogOutput($"Unable to create main menu button: backButton is null or empty", Log.LogLevel.Error);
            return;
        }

        mainMenuBackButton.onClick.AddListener(new Action(OnClickBackButton));

        var modOptionsButton = GameObject.Instantiate(mainMenuBackButton.gameObject).GetComponent<Button>();

        if (modOptionsButton == null)
        {
            Log.LogOutput($"Unable to create main menu button: modsButton is null", Log.LogLevel.Error);
            return;
        }

        mainMenuButton = modOptionsButton;

        modOptionsButton.transform.SetParent(optionsMenu.transform);

        var modsButtonRect = modOptionsButton.GetComponent<RectTransform>();
        modsButtonRect.localPosition = new Vector3(modsButtonRect.localPosition.x, modsButtonRect.localPosition.y + 100f, 0);

        FormatButton(modOptionsButton);
    }
    #endregion

    #region Helpers
    private static void OnClickBackButton()
    {
        if (ModOptionsMenu.Instance == null)
            return;

        if (ModOptionsMenu.Instance.gameObject.activeSelf)
            ToggleModOptionsMenu();
    }
    private static void SetMainMenuOptionsVisibility(bool state)
    {
        mainMenuOptions?.SetActive(state);
    }
    private static void SetIngameMenuOptionsVisibility(bool state)
    {
        ingameOptions?.SetActive(state);
        ingameLogo?.SetActive(state);
    }
    private static void FormatButton(Button button)
    {
        button.name = modButtonText;
        button.onClick.RemoveAllListeners();
        button.onClick.m_PersistentCalls.Clear();
        button.onClick.AddListener(new Action(ToggleModOptionsMenu));
        button.transform.localScale = Vector3.one;

        var textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = modButtonText;

        button.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
    }
    #endregion
}