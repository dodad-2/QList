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
    private static Button? mainMenuModOptionsButton, pauseMenuButton, mainMenuBackButton, pauseMenuResumeButton;
    private static Il2CppSilica.UI.PauseMenu? PauseMenuInstance;
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

    [HarmonyPatch(typeof(Il2CppSilica.UI.PauseMenu), nameof(Il2CppSilica.UI.PauseMenu.CloseScreen))]
    private static class PauseMenu_CloseScreen
    {
        private static void Postfix(Il2CppSilica.UI.PauseMenu __instance)
        {
            OnClickCloseModOptionsMenu();
            //Log.LogOutput($"PauseMenu_CloseScreen: Postfix {(__instance != null ? __instance.name : "null")}");
        }
    }
    [HarmonyPatch(typeof(Il2CppSilica.UI.PauseMenu), nameof(Il2CppSilica.UI.PauseMenu.Toggle))]
    private static class PauseMenu_Toggle
    {
        private static void Postfix(Il2CppSilica.UI.PauseMenu __instance)
        {
            PauseMenuInstance = __instance;
            OnClickCloseModOptionsMenu();

            //Log.LogOutput($"PauseMenu_Toggle: Postfix {(__instance != null ? __instance.name : "null")}");
        }
    }
    [HarmonyPatch(typeof(Il2CppSilica.UI.MainMenu), nameof(Il2CppSilica.UI.MainMenu.CloseScreen))]
    private static class MainMenu_CloseScreen
    {
        private static void Postfix(Il2CppSilica.UI.MenuScreenType type)
        {
            OnClickCloseModOptionsMenu();

            //Log.LogOutput($"MainMenu_CloseScreen: Postfix {type}");
        }
    }
    [HarmonyPatch(typeof(Il2CppSilica.UI.MainMenu), nameof(Il2CppSilica.UI.MainMenu.OpenScreen))]
    private static class MainMenu_OpenScreen
    {
        private static void Postfix(Il2CppSilica.UI.MainMenu __instance, Il2CppSilica.UI.MenuScreenType type)
        {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("menu") && type == Il2CppSilica.UI.MenuScreenType.None)
                __instance.OpenScreen(Il2CppSilica.UI.MenuScreenType.Intro);

            //Log.LogOutput($"MainMenu_OpenScreen: Postfix {type}");
        }
    }
    [HarmonyPatch(typeof(Il2CppSilica.UI.UIManager), nameof(Il2CppSilica.UI.UIManager.Awake))]
    private static class UIManager_Awake
    {
        private static void Postfix(Il2CppSilica.UI.UIManager __instance)
        {
            //Log.LogOutput($"UIManager_Awake: Postfix {(__instance != null ? __instance.name : "null")}");

            if (SceneManager.GetActiveScene().name.ToLower().Contains("menu"))
                ModifyMainMenu();
            else if (!SceneManager.GetActiveScene().name.ToLower().Contains("intro"))
                ModifyPauseMenu();

            OnClickCloseModOptionsMenu();
        }
    }
    #endregion

    #region UI
    public static void SetModOptionsMenuEnabled(bool state)
    {
        if (Il2CppSilica.UI.MenuManager.Instance.IsMenuOpen(Il2CppSilica.UI.MenuType.Main) && mainMenuOptions != null)
        {
            mainMenuOptions?.SetActive(!state);
        }
        else
        {
            if (PauseMenuInstance != null)
            {
                if (state)
                    PauseMenuInstance.CloseScreen(PauseMenuInstance.currentScreen);
                else
                    PauseMenuInstance.OpenScreen(PauseMenuInstance.currentScreen);
            }
        }

        if (state)
            ModOptionsMenu.Open();
        else
            ModOptionsMenu.Close();
    }
    public static void ToggleModOptionsMenuEnabled()
    {
        if (ModOptionsMenu.Instance == null)
            return;

        if (ModOptionsMenu.Instance.gameObject.activeSelf)
            SetModOptionsMenuEnabled(false);
        else
            SetModOptionsMenuEnabled(true);
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
    private static void ModifyPauseMenu()
    {
        var pauseMenu = Il2CppSilica.UI.MenuManager.Instance.GetComponentInChildren<Il2CppSilica.UI.PauseMenu>();

        if (pauseMenu != null)
        {
            ingameOptions = pauseMenu.Screens[0].gameObject;

            ingameLogo = pauseMenu.GetComponentsInChildren<Image>(true).Where(x => x.name.Equals("Logo")).First()?.gameObject;

            var resumeButtonTemplate = pauseMenu.GetComponentsInChildren<Button>(true).Where(x => x.name.ToLower().Equals("resume")).First().gameObject;

            if (resumeButtonTemplate != null)
            {
                pauseMenuButton = GameObject.Instantiate(resumeButtonTemplate).GetComponent<Button>();

                pauseMenuButton.transform.SetParent(ingameOptions.transform.parent.GetComponentInChildren<VerticalLayoutGroup>().transform);
                pauseMenuButton.transform.SetSiblingIndex(2);

                var modsButtonRect = pauseMenuButton.GetComponent<RectTransform>();

                FormatButton(pauseMenuButton);
            }
        }
    }
    private static void ModifyMainMenu()
    {
        var mainMenu = Il2CppSilica.UI.MenuManager.Instance.GetComponentInChildren<Il2CppSilica.UI.MainMenu>();

        if (mainMenu != null)
        {
            GameObject? backButtonTemplate = null;

            foreach (var optionsButton in mainMenu.OptionsButtons)
            {
                optionsButton.onClick.AddListener(new Action(OnClickCloseModOptionsMenu));
            }

            foreach (var backButton in mainMenu.BackToMainMenuButtons)
            {
                if (backButtonTemplate == null)
                    backButtonTemplate = backButton.gameObject;

                backButton.onClick.AddListener(new Action(OnClickCloseModOptionsMenu));
            }

            if (backButtonTemplate != null)
            {
                var modOptionsButtonObject = GameObject.Instantiate(backButtonTemplate.gameObject);

                var modOptionsButton = modOptionsButtonObject.GetComponent<Button>();

                if (modOptionsButton == null)
                {
                    Log.LogOutput($"Unable to create main menu button: modsButton is null", Log.LogLevel.Error);
                    return;
                }

                var mainMenuOptionsSearch = mainMenu.GetComponentsInChildren<Il2CppSilica.UI.OptionsForm>(true).Where(x => x.name.ToLower().Equals("options"));

                if (mainMenuOptionsSearch == null || mainMenuOptionsSearch.Count() == 0)
                {
                    Log.LogOutput($"Unable to find main menu OptionsForm", Log.LogLevel.Error);
                    return;
                }

                mainMenuOptions = mainMenuOptionsSearch.First().gameObject;

                modOptionsButton.transform.SetParent(mainMenuOptions.transform.parent);

                var modsButtonRect = modOptionsButton.GetComponent<RectTransform>();
                modsButtonRect.localPosition = new Vector3(modsButtonRect.localPosition.x, modsButtonRect.localPosition.y + 100f, 0);

                FormatButton(modOptionsButton);

                mainMenuModOptionsButton = modOptionsButton;
            }
        }
    }
    #endregion

    #region Helpers
    private static void OnClickCloseModOptionsMenu()
    {
        if (ModOptionsMenu.Instance != null && ModOptionsMenu.Instance.gameObject.activeSelf)
            SetModOptionsMenuEnabled(false);
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
        button.onClick.AddListener(new Action(ToggleModOptionsMenuEnabled));
        button.transform.localScale = Vector3.one;

        var textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = modButtonText;

        button.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
    }
    #endregion
}