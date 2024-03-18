namespace QList.UI;

using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using QList.OptionTypes;
using UnityEngine;

[RegisterTypeInIl2Cpp]
public class ModOptionsController : MonoBehaviour
{
    #region Variables
    private static readonly string defaultTitle = "Select a mod to configure";
    private static readonly string titleString =
        "{0} <size=20><color=orange>{1} </color></size><size=16>by {2}</size>";
    public static ModOptionsController? Instance;

    private static ModOptionContainer? focus;

    private static RectTransform? content;
    private static RectTransform? categoryPrefab;
    private static OptionComponent? optionPrefab;
    private static RectTransform? descriptionPrefab;
    private static TextMeshProUGUI? titleText;

    public static string? lastMod;
    #endregion

    #region Unity Methods
    public ModOptionsController(IntPtr ptr)
        : base(ptr) { }

    public void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(gameObject);

        Instance = this;

        FindPrefabs();
    }

    public void OnDisable()
    {
        ClearOptions();
    }
    #endregion

    #region UI
    private void FindPrefabs()
    {
        content = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        categoryPrefab = content.GetChild(0).GetComponent<RectTransform>(); // TODO less hard coding
        optionPrefab = content.GetChild(1).GetComponent<OptionComponent>();
        descriptionPrefab = content.GetChild(2).GetComponent<RectTransform>();
        titleText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void UpdateOptions()
    {
        if (
            focus == null
            || titleText == null
            || content == null
            || categoryPrefab == null
            || optionPrefab == null
            || descriptionPrefab == null
        )
            return;

        ClearOptions();

        titleText.text = String.Format(
            titleString,
            focus.mod.Info.Name,
            focus.mod.Info.Version,
            focus.mod.Info.Author
        ); //$"{focus.mod.Info.Name} <size=20>{focus.mod.Info.Version}</size> <size=18>by</size> <size=20>{focus.mod.Info.Author}</size>";

        List<string> categories = new List<string>();
        var options = focus.GetOptions();

        foreach (var option in options)
        {
            if (!categories.Contains(option.category))
                categories.Add(option.category);
        }

        RectTransform? newCategory;
        RectTransform? newDescription;
        OptionComponent? newOptionComponent;
        TextMeshProUGUI? descriptionText;

        foreach (var category in categories)
        {
            newCategory = GameObject.Instantiate(categoryPrefab).GetComponent<RectTransform>();
            newCategory.name = $"(Category) {category}";
            newCategory.SetParent(content, false);
            newCategory.GetComponentInChildren<TextMeshProUGUI>().text = category;
            newCategory.gameObject.SetActive(true);

            foreach (var option in options.Where(x => x.category.Equals(category)))
            {
                newOptionComponent = GameObject
                    .Instantiate(optionPrefab)
                    .GetComponent<OptionComponent>();
                newOptionComponent.name = $"(Option) {option.name}";
                newOptionComponent.transform.SetParent(content, false);
                newOptionComponent.gameObject.SetActive(true);

                newDescription = null;
                descriptionText = null;

                if (option.description != null && option.description.Length > 0)
                {
                    newDescription = GameObject
                        .Instantiate(descriptionPrefab)
                        .GetComponent<RectTransform>();
                    newDescription.name = $"(Description) {option.name}";
                    newDescription.SetParent(content, false);
                    newDescription.gameObject.SetActive(true);

                    descriptionText = newDescription.GetComponentInChildren<TextMeshProUGUI>();
                }

                newOptionComponent.SetOption(option, descriptionText);
            }
        }
    }

    private void ClearOptions()
    {
        if (content == null || titleText == null)
            return;

        titleText.text = defaultTitle;

        foreach (
            var button in content
                .GetComponentsInChildren<RectTransform>()
                .Where(x => !x.name.Equals("Content"))
        )
            GameObject.Destroy(button.gameObject);
    }
    #endregion

    #region Events
    public static void ShowOptionsFor(string mod)
    {
        if (Instance == null || mod == null)
            return;

        var search = Options.CurrentModOptions.Where(x => x.Key.Equals(mod));

        if (search == null || search.Count() == 0)
        {
            Log.LogOutput($"Mod '{mod}' not registered", Log.LogLevel.Warning);
            return;
        }

        lastMod = mod;

        var modContainer = search.First();

        focus = modContainer.Value;

        Instance.UpdateOptions();
    }
    #endregion
}
