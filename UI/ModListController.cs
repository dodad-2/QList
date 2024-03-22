namespace QList.UI;

using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[RegisterTypeInIl2Cpp]
public class ModListController : MonoBehaviour
{
    #region Variables
    public static ModListController? Instance;
    private Button listButtonPrefab;
    private Transform content;
    #endregion

    #region Unity Methods
    public ModListController(IntPtr ptr)
        : base(ptr) { }

    public void Awake()
    {
        listButtonPrefab = transform.GetComponentInChildren<Button>(true);
        content = transform.GetComponentInChildren<VerticalLayoutGroup>().transform;
    }

    public void OnEnable()
    {
        RefreshList();
    }

    public void OnDisable()
    {
        ClearList();
    }
    #endregion

    #region List
    public void RefreshList()
    {
        if (listButtonPrefab == null || content == null)
        {
            Log.LogOutput(
                $"Unable to display mod list: prefab or content is null",
                Log.ELevel.Error
            );
            return;
        }

        foreach (var mod in Options.CurrentModOptions)
            CreateModButton(mod.Value.mod.Info.Name, mod.Key);
    }

    public void ClearList()
    {
        foreach (
            var button in content
                .GetComponentsInChildren<RectTransform>()
                .Where(x => !x.name.Equals("Content") && !x.name.Equals("Name"))
        )
        {
            GameObject.Destroy(button.gameObject);
        }
    }
    #endregion

    #region Helpers
    private void CreateModButton(string name, string key)
    {
        var modButton = GameObject.Instantiate(listButtonPrefab);
        modButton.transform.SetParent(content, false);
        modButton.onClick.AddListener(
            new Action(
                delegate
                {
                    ModOptionsController.ShowOptionsFor(key);
                }
            )
        );
        modButton.name = key; //mod.Info.Name;
        TextMeshProUGUI text = modButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = name; //mod.Info.Name;
        modButton.gameObject.SetActive(true);
    }
    #endregion
}
