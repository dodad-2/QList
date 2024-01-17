using UnityEngine;
using MelonLoader;
using Il2CppTMPro;

namespace QList.UI;

[RegisterTypeInIl2Cpp]
public class HotkeyListener : MonoBehaviour
{
    public static HotkeyListener? Instance;
    /// <summary>
    /// Listeners cleared after every invocation.
    /// </summary>
    public static Action<KeyCode[]>? OnHotkey;
    /// <summary>
    /// Listeners cleared after every invocation.
    /// </summary>
    public static Action? OnCancelHotkey;

    private TMP_InputField? keybindDisplay;

    private List<KeyCode> currentCombo = new();
    private List<KeyCode> validKeys = new();

    public void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(gameObject);

        keybindDisplay = transform.GetComponentInChildren<TMP_InputField>();

        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if ((KeyCode)keyCode != KeyCode.CapsLock && (KeyCode)keyCode != KeyCode.Escape)
                validKeys.Add((KeyCode)keyCode);
        }

        gameObject.SetActive(false);

        Instance = this;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.CapsLock))
            CancelListen();
        else
        {
            if (keybindDisplay == null || Instance == null)
                return;

            foreach (var keyCode in validKeys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (!currentCombo.Contains(keyCode))
                    {
                        AddKey(keyCode);
                        keybindDisplay.SetText(GetComboString(currentCombo.ToArray()));
                        Log.LogOutput($"HotkeyListener.Update: Adding {keyCode}");
                    }
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    NotifyNewKeybind();
                    Instance.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
    public void OnDisable()
    {
        Log.LogOutput($"HotkeyListener.OnDisable");
        currentCombo.Clear();
        OnHotkey = null;
        OnCancelHotkey = null;
    }
    public static bool BeginListen()
    {
        Log.LogOutput($"HotkeyListener.BeginListen");
        if (Instance == null || Instance.gameObject.activeSelf)
            return false;

        Instance.gameObject.SetActive(true);

        return true;
    }
    public static void CancelListen()
    {
        if (Instance == null || !Instance.gameObject.activeSelf)
            return;

        OnCancelHotkey?.Invoke();
        Instance.gameObject.SetActive(false);
    }
    private void NotifyNewKeybind()
    {
        if (currentCombo != null && currentCombo.Count() > 0)
            OnHotkey?.Invoke(currentCombo.ToArray());
    }
    private void AddKey(KeyCode keyCode)
    {
        currentCombo.Add(keyCode);
    }
    public static string GetComboString(KeyCode[] combo)
    {
        var comboString = "";
        int count = combo.Count();

        for (int e = 0; e < count; e++)
        {
            if (e == 0)
                comboString = combo[e].ToString();
            else
                comboString = $"{comboString}+{combo[e].ToString()}";
        }

        return comboString;
    }
}