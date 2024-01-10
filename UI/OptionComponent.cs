namespace QList.UI;

using MelonLoader;
using UnityEngine;
using QList.OptionTypes;
using Il2CppTMPro;
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

    private BaseOption? option;
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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        onValueChangedUntypedDelegate = new Action<object, object>(OnValueChangedUntyped);
        onOptionInfoUpdatedDelegate = new Action<BaseOption>(OnOptionInfoUpdated);

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

                if (intOption.slider)
                {
                    if (sliderEdit == null || sliderInputField == null || slider == null)
                        break;

                    slider.maxValue = intOption.max;
                    slider.minValue = intOption.min;
                    slider.wholeNumbers = true;
                    sliderInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    sliderInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
                    sliderInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;

                    //slider.SetValueWithoutNotify(intOption.currentValue);
                    //sliderInputField.SetText(intOption.currentValue.ToString());

                    sliderEdit.gameObject.SetActive(true);
                    currentInput = sliderEdit;
                }
                else
                {
                    if (intEdit == null || intInputField == null)
                        break;

                    //intInputField.SetText(intOption.currentValue.ToString());

                    intEdit.gameObject.SetActive(true);
                    currentInput = intEdit;
                }

                break;
            case FloatOption:
                FloatOption? floatOption = option as FloatOption;

                if (floatOption == null)
                    break;

                if (floatOption.slider)
                {
                    if (sliderEdit == null || sliderInputField == null || slider == null)
                        break;

                    slider.maxValue = floatOption.max;
                    slider.minValue = floatOption.min;
                    slider.wholeNumbers = false;
                    sliderInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    sliderInputField.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                    sliderInputField.characterValidation = TMP_InputField.CharacterValidation.Decimal;

                    //slider.SetValueWithoutNotify(floatOption.currentValue);
                    //sliderInputField.SetText(floatOption.currentValue.ToString());

                    sliderEdit.gameObject.SetActive(true);
                    currentInput = sliderEdit;
                }
                else
                {
                    if (floatEdit == null || floatInputField == null)
                        break;

                    //floatInputField.SetText(floatOption.currentValue.ToString());

                    floatEdit.gameObject.SetActive(true);
                    currentInput = floatEdit;
                }

                break;
            case BoolOption:
                BoolOption? boolOption = option as BoolOption;

                if (boolOption == null || boolEdit == null)
                    break;

                boolEdit.SetActive(true);
                currentInput = boolEdit;
                break;
            case StringOption:
                StringOption? stringOption = option as StringOption;

                if (stringOption == null || stringEdit == null)
                    break;

                stringEdit.SetActive(true);
                currentInput = stringEdit;
                break;
            case ButtonOption:
                ButtonOption? buttonOption = option as ButtonOption;

                if (buttonOption == null || buttonEdit == null)
                    break;

                buttonEdit.SetActive(true);
                currentInput = buttonEdit;
                break;
        }
    }
    #endregion

    #region Option
    public void SetOption(BaseOption option, TextMeshProUGUI? description = null)
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

                if (intOption == null || slider == null || sliderInputField == null || intInputField == null)
                    break;

                if (intOption.slider)
                {
                    slider.SetValueWithoutNotify(intOption.currentValue);
                    sliderInputField.SetTextWithoutNotify(intOption.currentValue.ToString());
                }
                else
                {
                    intInputField.SetText(intOption.currentValue.ToString());
                }
                break;
            case FloatOption:
                FloatOption? floatOption = option as FloatOption;

                if (floatOption == null || slider == null || sliderInputField == null || floatInputField == null)
                    break;

                var floatRounded = Math.Round((double)floatOption.currentValue, 3).ToString();

                if (floatOption.slider)
                {
                    slider.SetValueWithoutNotify(floatOption.currentValue);
                    sliderInputField.SetTextWithoutNotify(floatRounded);
                }
                else
                {
                    floatInputField.SetText(floatRounded);
                }
                break;
            case BoolOption:
                BoolOption? boolOption = option as BoolOption;

                if (boolOption == null || boolToggle == null || boolText == null)
                    break;

                UpdateBoolInput(boolOption.currentValue);

                break;
            case StringOption:
                StringOption? stringOption = option as StringOption;

                if (stringOption == null || stringInputField == null)
                    break;

                stringInputField.SetTextWithoutNotify(stringOption.currentValue == null ? "" : stringOption.currentValue);

                break;
            case ButtonOption:
                ButtonOption? buttonOption = option as ButtonOption;

                if (buttonOption == null || buttonText == null)
                    break;

                buttonText.SetText(buttonOption.ButtonName);

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
    }
    private void UnsubscribeFromOption()
    {
        if (option == null || onValueChangedUntypedDelegate == null)
            return;

        option.OnValueChangedUntyped -= onValueChangedUntypedDelegate;
        option.OnInfoUpdated -= onOptionInfoUpdatedDelegate;
    }
    private void OnOptionInfoUpdated(BaseOption option)
    {
        if (title != null)
            title.SetText(option.name);

        if (description != null)
            description.SetText(option.description);

        if (option is ButtonOption)
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

            if (int.TryParse(newValue, System.Globalization.NumberStyles.Float, null, out intParse))
            {
                option.SetValue(intParse);
            }
        }
        else if (option is FloatOption)
        {
            float floatParse;

            if (float.TryParse(newValue, System.Globalization.NumberStyles.Float, null, out floatParse))
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
        var buttonOption = option as ButtonOption;

        if (buttonOption != null)
            buttonOption.Click();
    }
    #endregion
}