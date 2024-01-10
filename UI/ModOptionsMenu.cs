namespace QList.UI;

using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using UnityEngine.UI;

[RegisterTypeInIl2Cpp]
public class ModOptionsMenu : MonoBehaviour
{
    #region Variables
    public static ModOptionsMenu? Instance;

    private TextMeshProUGUI? listTitle;
    private TextMeshProUGUI? modTitle;
    private RectTransform? listContentContainer;

    private CameraPosition? cameraPosition;
    #endregion

    #region Unity Methods
    public ModOptionsMenu(IntPtr ptr) : base(ptr) { }
    private void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(gameObject);

        Instance = this;

        Initialize();
        Close();
    }
    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            Manager.ToggleModOptionsMenu();
    }
    #endregion

    #region Static
    public static void Open()
    {
        Log.LogOutput("ModOptionsMenu.Open");
        if (Instance == null)
            return;

        Instance?.gameObject.SetActive(true);

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
        Log.LogOutput("ModOptionsMenu.Close");
        if (Instance == null)
            return;

        Instance?.gameObject.SetActive(false);

        if (Instance.cameraPosition != null)
            Instance.cameraPosition.reposition = false;
    }
    public static void Toggle()
    {
        Log.LogOutput("ModOptionsMenu.Toggle");
        if (Instance == null)
            return;

        if (Instance.gameObject.activeSelf)
            Close();
        else
            Open();
    }
    #endregion

    #region Initialize
    private void Initialize()
    {
        listTitle = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        modTitle = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        listContentContainer = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        if (Manager.ExistingImagePrefabHash != null && Manager.ExistingImagePrefabHash.ContainsKey("window_md"))
        {
            foreach (var image in GetComponentsInChildren<Image>().Where(x => x.name.ToLower().Equals("window_md")))
                image.sprite = Manager.ExistingImagePrefabHash["window_md"].sprite;
        }

        var fonts = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<TMP_FontAsset>());

        if (fonts == null || fonts.Count() == 0)
        {
            Log.LogOutput($"Unable to query fonts", Log.LogLevel.Warning);
            return;
        }

        var font = fonts.Where(x => x.name.ToLower().Contains("bebas"));

        if (font == null || font.First() == null)
        {
            Log.LogOutput($"Unable to find font", Log.LogLevel.Warning);
            return;
        }

        font = font.ToArray();

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
            text.font = font.First().TryCast<TMP_FontAsset>();
    }
    #endregion
}