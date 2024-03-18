namespace QList.UI;

using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[RegisterTypeInIl2Cpp]
public class ModOptionsMenu : MonoBehaviour
{
    #region Variables
    public static ModOptionsMenu? Instance;
    public static bool OpenOnEnable;

    private TextMeshProUGUI? listTitle;
    private TextMeshProUGUI? modTitle;
    private RectTransform? listContentContainer;

    private CameraPosition? cameraPosition;
    #endregion

    #region Unity Methods
    public ModOptionsMenu(IntPtr ptr)
        : base(ptr) { }

    private void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(gameObject);

        Instance = this;

        Initialize();
        Close();
    }
    #endregion

    #region Static

    public static void Open() // TODO allow opening a mod's page
    {
        if (Instance == null || Instance.gameObject.activeSelf)
            return;

        OpenOnEnable = false;

        Instance.gameObject.SetActive(true);

        if (Instance.cameraPosition == null)
        {
            var mainMenuCameraSearch = GameObject.Find("CAM_MainMenu");

            if (mainMenuCameraSearch != null)
            {
                var mainMenuCamera = mainMenuCameraSearch.GetComponent<Camera>();
                Instance.cameraPosition = mainMenuCamera.gameObject.AddComponent<CameraPosition>();
            }
        }

        if (Instance.cameraPosition != null)
            Instance.cameraPosition.reposition = true;
    }

    public static void Close()
    {
        if (Instance == null || !Instance.gameObject.activeSelf)
            return;

        Instance.gameObject.SetActive(false);

        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            OpenOnEnable = true;

        if (Instance.cameraPosition != null)
            Instance.cameraPosition.reposition = false;
    }
    #endregion

    #region Initialize
    private void Initialize()
    {
        listTitle = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        modTitle = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        listContentContainer = transform
            .GetChild(0)
            .GetChild(1)
            .GetChild(0)
            .GetChild(0)
            .GetComponent<RectTransform>();

        if (
            Manager.ExistingImagePrefabHash != null
            && Manager.ExistingImagePrefabHash.ContainsKey("window_md")
        )
        {
            foreach (
                var image in GetComponentsInChildren<Image>()
                    .Where(x => x.name.ToLower().Equals("window_md"))
            )
                image.sprite = Manager.ExistingImagePrefabHash["window_md"].sprite;
        }

        var fonts = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<TMP_FontAsset>());

        if (fonts == null || fonts.Count() == 0)
        {
            Log.LogOutput($"Unable to query fonts", Log.LogLevel.Warning);
            return;
        }

        var fontSearch = fonts.Where(x => x.name.ToLower().Contains("bebas"));

        if (fontSearch == null || fontSearch.First() == null)
        {
            Log.LogOutput($"Unable to find font (1)", Log.LogLevel.Warning);
            return;
        }

        fontSearch = fontSearch.ToArray(); // Is this necessary?
        var font = fontSearch.First().TryCast<TMP_FontAsset>();

        if (font == null)
        {
            Log.LogOutput($"Unable to find font (2)", Log.LogLevel.Warning);
            return;
        }

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
            text.font = font;
    }
    #endregion
}
