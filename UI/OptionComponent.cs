namespace QList.UI;

using Il2CppTMPro;
using MelonLoader;
using QList.OptionTypes;
using UnityEngine;
using UnityEngine.UI;

[RegisterTypeInIl2Cpp]
public class OptionComponent : MonoBehaviour
{
    #region Variables
    private static readonly string boolEnabledText = "Enabled";
    private static readonly Color boolEnabledColor = new Color(0, 1, 0, 1);
    private static readonly string boolDisabledText = "Disabled";
    private static readonly Color boolDisabledColor = new Color(1, 0, 0, 1);

    private Action<object, object>? onValueChangedUntypedDelegate;
    private Action<BaseOption>? onOptionInfoUpdatedDelegate;
    private Action<bool>? onAllowUserEditsUpdated;

    private BaseOption? option;
    private Selectable? selectable;
    private TextMeshProUGUI? title;
    private TextMeshProUGUI? description;
    private GameObject? currentInput;

    private GameObject? intEdit;
    private TMP_InputField? intInputField;

    private GameObject? floatEdit;
    private TMP_InputField? floatInputField;

    private GameObject? sliderEdit;
    private Slider? slider;
    private TMP_InputField? sliderInputField;

    private GameObject? boolEdit;
    private Toggle? boolToggle;
    private TextMeshProUGUI? boolText;

    private GameObject? stringEdit;
    private TMP_InputField? stringInputField;

    private GameObject? buttonEdit;
    private Button? button;
    private TextMeshProUGUI? buttonText;

    private GameObject? keybindEdit;
    private TMP_InputField? keybindInputField;
    private Button? keybindButton;

    private GameObject? dropdownEdit;
    private TMP_Dropdown? dropdownInputField;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        onValueChangedUntypedDelegate = new Action<object, object>(OnValueChangedUntyped);
        onOptionInfoUpdatedDelegate = new Action<BaseOption>(OnOptionInfoUpdated);
        onAllowUserEditsUpdated = new Action<bool>(OnAllowUserEditsUpdated);

        title = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        intEdit = transform.FindChild("Option/Editable/IntEdit").gameObject;

        if (intEdit != null)
        {
            intInputField = intEdit.transform.GetChild(0).GetComponent<TMP_InputField>();
            intInputField.onSubmit.AddListener(new Action<string>(OnEndIntEdit));
            intInputField.onEndEdit.AddListener(new Action<string>(OnEndIntEdit));
        }

        floatEdit = transform.FindChild("Option/Editable/FloatEdit").gameObject;

        if (floatEdit != null)
        {
            floatInputField = floatEdit.transform.GetChild(0).GetComponent<TMP_InputField>();
            floatInputField.onSubmit.AddListener(new Action<string>(OnEndFloatEdit));
            floatInputField.onEndEdit.AddListener(new Action<string>(OnEndFloatEdit));
        }

        sliderEdit = transform.FindChild("Option/Editable/SliderEdit").gameObject;

        if (sliderEdit != null)
        {
            slider = sliderEdit.transform.GetChild(0).GetComponent<Slider>();

            if (slider != null)
            {
                slider.onValueChanged.AddListener(new Action<float>(OnSliderValue));

                sliderInputField = sliderEdit.transform.GetChild(1).GetComponent<TMP_InputField>();
                sliderInputField.onSubmit.AddListener(new Action<string>(OnEndSliderEdit));
                sliderInputField.onEndEdit.AddListener(new Action<string>(OnEndSliderEdit));
            }
        }

        boolEdit = transform.FindChild("Option/Editable/BoolEdit").gameObject;

        if (boolEdit != null)
        {
            boolToggle = boolEdit.transform.GetChild(0).GetComponent<Toggle>();
            boolText = boolEdit.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            boolToggle.onValueChanged.AddListener(new Action<bool>(OnToggle));
        }

        stringEdit = transform.FindChild("Option/Editable/StringEdit").gameObject;

        if (stringEdit != null)
        {
            stringInputField = stringEdit.transform.GetChild(0).GetComponent<TMP_InputField>();
            stringInputField.onSubmit.AddListener(new Action<string>(OnEndStringEdit));
            stringInputField.onEndEdit.AddListener(new Action<string>(OnEndStringEdit));
        }

        buttonEdit = transform.FindChild("Option/Editable/ButtonEdit").gameObject;

        if (buttonEdit != null)
        {
            button = buttonEdit.transform.GetChild(0).GetComponent<Button>();
            button.onClick.AddListener(new Action(OnButtonClick));
            buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        keybindEdit = transform.FindChild("Option/Editable/KeybindEdit").gameObject;

        if (keybindEdit != null)
        {
            keybindButton = keybindEdit.GetComponentInChildren<Button>();
            keybindButton.onClick.AddListener(new Action(OnKeybindButtonClick));
            keybindInputField = keybindEdit.GetComponentInChildren<TMP_InputField>();
        }

        dropdownEdit = transform.FindChild("Option/Editable/DropdownEdit").gameObject;

        if (dropdownEdit != null)
        {
            dropdownInputField = dropdownEdit.GetComponentInChildren<TMP_Dropdown>();
            dropdownInputField.onValueChanged.AddListener(new Action<int>(OnDropdownChanged));
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromOption();
    }
    #endregion

    #region UI
    private void UpdateInputMode()
    {
        HideInput();
        ActivateInput();
        UpdateUIFromOption();
    }

    private void HideInput()
    {
        currentInput?.gameObject.SetActive(false);
    }

    private void ActivateInput()
    {
        if (option == null)
            return;

        switch (option)
        {
            case IntOption:
                IntOption? intOption = option as IntOption;

                if (intOption == null)
                    break;

                var intValue = intOption.GetValue();

                if (intOption.slider)
                {
                    if (sliderEdit == null || sliderInputField == null || slider == null)
                        break;

                    slider.maxValue = intOption.max;
                    slider.minValue = intOption.min;
                    slider.wholeNumbers = true;
                    sliderInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    sliderInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
                    sliderInputField.characterValidation = TMP_InputField
                        .CharacterValidation
                        .Integer;

                    slider.value = intValue;
                    sliderInputField.SetText(intValue.ToString());

                    sliderEdit.gameObject.SetActive(true);
                    currentInput = sliderEdit;
                    selectable = sliderInputField;
                }
                else
                {
                    if (intEdit == null || intInputField == null)
                        break;

                    if (intOption.entry == null)
                    {
                        intInputField.SetText(intValue.ToString());
                    }

                    intEdit.gameObject.SetActive(true);
                    currentInput = intEdit;
                    selectable = intInputField;
                }

                break;
            case FloatOption:
                FloatOption? floatOption = option as FloatOption;

                if (floatOption == null)
                    break;

                var floatValue = floatOption.GetValue();

                if (floatOption.slider)
                {
                    if (sliderEdit == null || sliderInputField == null || slider == null)
                        break;

                    slider.maxValue = floatOption.max;
                    slider.minValue = floatOption.min;
                    slider.wholeNumbers = false;
                    sliderInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    sliderInputField.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                    sliderInputField.characterValidation = TMP_InputField
                        .CharacterValidation
                        .Decimal;

                    slider.value = floatValue;
                    sliderInputField.SetText(floatValue.ToString());

                    sliderEdit.gameObject.SetActive(true);
                    currentInput = sliderEdit;
                    selectable = sliderInputField;
                }
                else
                {
                    if (floatEdit == null || floatInputField == null)
                        break;

                    //floatInputField.SetText(floatOption.currentValue.ToString());

                    floatEdit.gameObject.SetActive(true);
                    currentInput = floatEdit;
                    selectable = floatInputField;
                }

                break;
            case BoolOption:
                BoolOption? boolOption = option as BoolOption;

                if (boolOption == null || boolEdit == null || boolToggle == null)
                    break;

                boolEdit.SetActive(true);
                currentInput = boolEdit;
                selectable = boolToggle;

                break;
            case StringOption:
                StringOption? stringOption = option as StringOption;

                if (stringOption == null || stringEdit == null || stringInputField == null)
                    break;

                stringEdit.SetActive(true);
                currentInput = stringEdit;
                selectable = stringInputField;

                break;
            case ButtonOption:
                ButtonOption? buttonOption = option as ButtonOption;

                if (buttonOption == null || buttonEdit == null || button == null)
                    break;

                buttonEdit.SetActive(true);
                currentInput = buttonEdit;
                selectable = button;

                break;
            case KeybindOption:
                KeybindOption? keybindOption = option as KeybindOption;

                if (keybindOption == null || keybindEdit == null || keybindButton == null)
                    break;

                keybindEdit.SetActive(true);
                currentInput = keybindEdit;
                selectable = keybindButton;

                break;
            case DropdownOption:
                DropdownOption? dropdownOption = option as DropdownOption;

                if (dropdownOption == null || dropdownEdit == null || dropdownInputField == null)
                    break;

                dropdownEdit.SetActive(true);

                var valueNames = dropdownOption.GetValueNames();

                dropdownInputField.options.Clear();

                for (int e = 0; e < valueNames.Length; e++)
                    dropdownInputField.options.Add(new TMP_Dropdown.OptionData(valueNames[e]));

                currentInput = dropdownEdit;
                selectable = dropdownInputField;

                break;
        }

        if (selectable != null)
            selectable.interactable = option.allowUserEdits;
    }
    #endregion

    #region Option
    public void OnAllowUserEditsUpdated(bool value)
    {
        if (selectable != null)
            selectable.interactable = value;
    }

    public void SetOption(BaseOption? option, TextMeshProUGUI? description = null)
    {
        if (option == null || title == null)
            return;

        if (this.option != null)
            UnsubscribeFromOption();

        this.description = description;
        this.option = option;

        SubscribeToOption();
        UpdateInputMode();
        OnOptionInfoUpdated(option);
    }

    public BaseOption? GetOption()
    {
        return this.option;
    }

    public void UpdateUIFromOption()
    {
        if (option == null)
            return;

        switch (option)
        {
            case IntOption:
                IntOption? intOption = option as IntOption;

                if (
                    intOption == null
                    || slider == null
                    || sliderInputField == null
                    || intInputField == null
                )
                    break;

                if (intOption.slider)
                {
                    slider.SetValueWithoutNotify(intOption.currentValue);
                    sliderInputField.SetTextWithoutNotify(intOption.currentValue.ToString());
                    sliderInputField.interactable = option.allowUserEdits;
                }
                else
                {
                    intInputField.SetText(intOption.currentValue.ToString());
                    intInputField.interactable = option.allowUserEdits;
                }
                break;
            case FloatOption:
                FloatOption? floatOption = option as FloatOption;

                if (
                    floatOption == null
                    || slider == null
                    || sliderInputField == null
                    || floatInputField == null
                )
                    break;

                var floatRounded = Math.Round((double)floatOption.currentValue, 3).ToString();

                if (floatOption.slider)
                {
                    slider.SetValueWithoutNotify(floatOption.currentValue);
                    sliderInputField.SetTextWithoutNotify(floatRounded);
                    sliderInputField.interactable = option.allowUserEdits;
                }
                else
                {
                    floatInputField.SetText(floatRounded);
                    floatInputField.interactable = option.allowUserEdits;
                }
                break;
            case BoolOption:
                BoolOption? boolOption = option as BoolOption;

                if (boolOption == null || boolToggle == null || boolText == null)
                    break;

                UpdateBoolInput(boolOption.currentValue);
                boolToggle.interactable = option.allowUserEdits;

                break;
            case StringOption:
                StringOption? stringOption = option as StringOption;

                if (stringOption == null || stringInputField == null)
                    break;

                stringInputField.SetTextWithoutNotify(
                    stringOption.currentValue == null ? "" : stringOption.currentValue
                );
                stringInputField.interactable = option.allowUserEdits;

                break;
            case ButtonOption:
                ButtonOption? buttonOption = option as ButtonOption;

                if (buttonOption == null || buttonText == null || button == null)
                    break;

                buttonText.SetText(buttonOption.ButtonName);
                button.interactable = option.allowUserEdits;

                break;
            case KeybindOption:
                KeybindOption? keybindOption = option as KeybindOption;

                if (keybindOption == null || keybindInputField == null || keybindButton == null)
                    break;

                keybindInputField.SetText(
                    HotkeyListener.GetComboString(keybindOption.currentValue)
                );
                keybindButton.interactable = option.allowUserEdits;

                break;
            case DropdownOption:
                DropdownOption? dropdownOption = option as DropdownOption;

                if (dropdownOption == null || dropdownInputField == null)
                    break;

                dropdownInputField.SetValueWithoutNotify(dropdownOption.GetValue());
                dropdownInputField.interactable = option.allowUserEdits;

                break;
        }
    }
    #endregion

    #region Events
    private void SubscribeToOption()
    {
        if (option == null || onValueChangedUntypedDelegate == null)
            return;

        option.OnValueChangedUntyped += onValueChangedUntypedDelegate;
        option.OnInfoUpdated += onOptionInfoUpdatedDelegate;
        option.OnAllowUserEditsUpdated += onAllowUserEditsUpdated;
    }

    private void UnsubscribeFromOption()
    {
        if (option == null || onValueChangedUntypedDelegate == null)
            return;

        option.OnValueChangedUntyped -= onValueChangedUntypedDelegate;
        option.OnInfoUpdated -= onOptionInfoUpdatedDelegate;
        option.OnAllowUserEditsUpdated -= onAllowUserEditsUpdated;
    }

    private void OnOptionInfoUpdated(BaseOption option)
    {
        if (title != null)
            title.SetText(option.name);

        if (description != null)
            description.SetText(option.description);

        if (option is ButtonOption || option is DropdownOption)
            UpdateUIFromOption();
    }

    private void OnValueChangedUntyped(object oldValue, object newValue)
    {
        UpdateUIFromOption();
    }

    private void OnEndIntEdit(string newValue) // TODO combine all of these methods
    {
        OnNumberEdit(newValue);
    }

    private void OnEndFloatEdit(string newValue)
    {
        OnNumberEdit(newValue);
    }

    private void OnSliderValue(float newValue)
    {
        OnNumberEdit(newValue.ToString());
    }

    private void OnEndSliderEdit(string newValue)
    {
        OnNumberEdit(newValue);
    }

    private void OnNumberEdit(string newValue)
    {
        if (slider == null || option == null || newValue.Length == 0 || sliderInputField == null)
            return;

        if (option is IntOption)
        {
            int intParse;

            if (
                int.TryParse(
                    newValue,
                    System.Globalization.NumberStyles.Integer,
                    null,
                    out intParse
                )
            )
            {
                option.SetValue(intParse);
            }
        }
        else if (option is FloatOption)
        {
            float floatParse;

            if (
                float.TryParse(
                    newValue,
                    System.Globalization.NumberStyles.Float,
                    null,
                    out floatParse
                )
            )
            {
                option.SetValue(floatParse);
            }
        }
    }

    private void OnToggle(bool newValue)
    {
        if (option == null)
            return;

        option.SetValue(newValue);
        UpdateBoolInput(newValue);
    }

    private void UpdateBoolInput(bool newValue)
    {
        if (boolText == null || boolToggle == null)
            return;

        boolToggle.SetIsOnWithoutNotify(newValue);
        boolText.SetText(newValue ? boolEnabledText : boolDisabledText);
        boolText.color = (newValue ? boolEnabledColor : boolDisabledColor);
    }

    private void OnEndStringEdit(string newValue)
    {
        if (option == null || stringInputField == null || newValue == null)
            return;

        option.SetValue(newValue);
    }

    private void OnButtonClick()
    {
        if (option == null)
            return;

        var buttonOption = option as ButtonOption;

        if (buttonOption != null)
            buttonOption.Click();
    }

    private void OnDropdownChanged(int newValue)
    {
        if (option == null)
            return;

        option.SetValue(newValue);
    }

    //[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void OnKeybindButtonClick()
    {
        var keybindButton = option as KeybindOption;

        if (keybindButton == null)
            return;

        keybindButton.name = keybindButton.Name; // fix
        HotkeyListener.OnHotkey += new Action<KeyCode[]>(OnHotkey);
        HotkeyListener.OnCancelHotkey += new Action(OnCancelHotkey);
        HotkeyListener.BeginListen();
    }

    private void OnHotkey(KeyCode[] keyCodes)
    {
        var hotkeyButton = option as KeybindOption;

        if (hotkeyButton != null)
            hotkeyButton.SetValue(keyCodes);
    }

    private void OnCancelHotkey()
    {
        var hotkeyButton = option as KeybindOption;

        if (hotkeyButton != null)
            hotkeyButton.SetValue(new KeyCode[0]);
    }
    #endregion

    #region Helpers
    private void HotkeyButtonReroute() { }
    #endregion
}
