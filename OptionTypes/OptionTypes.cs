namespace QList.OptionTypes;

using MelonLoader;

public abstract class BaseOption
{
    protected static readonly string defaultCategory = "Options";
    protected static readonly string defaultButtonName = "Activate";
    protected static readonly string defaultName = "(name not set)";
    protected static readonly string defaultDescription = "";

    public Action<object, object>? OnValueChangedUntyped;
    public Action<BaseOption>? OnValueChangedOption;

    public Action<BaseOption>? OnInfoUpdated;
    internal LemonAction<object, object>? onEntryValueChangedUntyped;

    internal MelonPreferences_Entry? entry;

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
            OnInfoUpdated?.Invoke(this);
        }
    }
    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
            OnInfoUpdated?.Invoke(this);
        }
    }
    public string Category
    {
        get
        {
            return category;
        }
    }

    internal string name;
    internal string description;
    internal string category;

    public BaseOption(MelonPreferences_Entry? preference)
    {
        this.entry = preference;
        this.category = preference == null ? defaultCategory : preference.Category.DisplayName;
        this.name = preference == null ? defaultName : preference.DisplayName;
        this.description = preference == null ? defaultDescription : preference.Description;
    }

    public virtual void SetValue(object newValue) { }
    internal virtual void OnDestroy() { }
}
public class IntOption : BaseOption
{
    public new Action<IntOption>? OnValueChangedOption;
    internal bool slider;
    internal int currentValue, min, max;

    public IntOption(MelonPreferences_Entry? entry = null, bool slider = false, int currentValue = 0, int min = 0, int max = 1, int step = 1) : base(entry)
    {
        this.entry = entry;
        this.min = min;
        this.max = max;
        this.slider = slider;
        this.currentValue = currentValue;

        if (this.entry != null)
        {
            this.onEntryValueChangedUntyped = new LemonAction<object, object>(OnValueChanged);
            this.entry.OnEntryValueChangedUntyped.Subscribe(onEntryValueChangedUntyped);

            try
            {
                this.currentValue = Convert.ToInt32(this.entry.BoxedValue);
            }
            catch (Exception e)
            {
                Log.LogOutput($"{this.GetType()}: Could not parse '{Name}': {e}", Log.LogLevel.Warning);
            }
        }
    }

    public int GetValue()
    {
        try
        {
            if (entry != null)
                currentValue = (int)entry.BoxedValue;
        }
        catch (Exception e)
        {
            Log.LogOutput($"{this.GetType()}.GetValue: Could not parse '{Name}': {e}", Log.LogLevel.Warning);
        }

        return currentValue;
    }
    public override void SetValue(object newValue)
    {
        if (entry != null)
            entry.BoxedValue = newValue;
        else
            this.OnValueChanged(currentValue, newValue);
    }
    internal override void OnDestroy()
    {
        this.entry?.OnEntryValueChangedUntyped.Unsubscribe(onEntryValueChangedUntyped);
    }
    private void OnValueChanged(object oldValue, object newValue)
    {
        try
        {
            int newInt = Convert.ToInt32(newValue);
            this.currentValue = newInt;
            this.OnValueChangedUntyped?.Invoke(oldValue, newValue);
            this.OnValueChangedOption?.Invoke(this);
        }
        catch (Exception e)
        {
            Log.LogOutput($"{this.GetType()}.OnValueChanged: Could not parse '{Name}': {e}", Log.LogLevel.Warning);
        }
    }
}
public class FloatOption : BaseOption
{
    public new Action<FloatOption>? OnValueChangedOption;
    internal bool slider;
    internal float currentValue, min, max;

    public FloatOption(MelonPreferences_Entry? entry = null, bool slider = false, float currentValue = 0, float min = 0, float max = 1, float step = 0.1f) : base(entry)
    {
        this.entry = entry;
        this.min = min;
        this.max = max;
        this.slider = slider;
        this.currentValue = currentValue;

        if (this.entry != null)
        {
            this.onEntryValueChangedUntyped = new LemonAction<object, object>(OnValueChanged);
            this.entry.OnEntryValueChangedUntyped.Subscribe(onEntryValueChangedUntyped);

            try
            {
                this.currentValue = (float)this.entry.BoxedValue;
            }
            catch (Exception e)
            {
                Log.LogOutput($"{this.GetType()}: Could not parse '{Name}': {e}", Log.LogLevel.Warning);
            }
        }
    }

    public float GetValue()
    {
        if (entry != null)
            currentValue = (float)entry.BoxedValue;

        return currentValue;
    }
    public override void SetValue(object newValue)
    {
        if (entry != null)
            entry.BoxedValue = newValue;
        else
            this.OnValueChanged(currentValue, newValue);
    }
    internal override void OnDestroy()
    {
        this.entry?.OnEntryValueChangedUntyped.Unsubscribe(onEntryValueChangedUntyped);
    }
    private void OnValueChanged(object oldValue, object newValue)
    {
        float newFloat;

        if (!float.TryParse(newValue.ToString(), System.Globalization.NumberStyles.Float, null, out newFloat))
        {
            Log.LogOutput($"{this.GetType()}.OnValueChanged: Could not parse '{Name}':", Log.LogLevel.Warning);
            return;
        }

        this.currentValue = newFloat;
        this.OnValueChangedUntyped?.Invoke(oldValue, newValue);
        this.OnValueChangedOption?.Invoke(this);
    }
}

public class BoolOption : BaseOption
{
    public new Action<BoolOption>? OnValueChangedOption;
    internal bool currentValue;

    public BoolOption(MelonPreferences_Entry? entry = null, bool currentValue = false) : base(entry)
    {
        this.entry = entry;
        this.currentValue = currentValue;

        if (this.entry != null)
        {
            this.onEntryValueChangedUntyped = new LemonAction<object, object>(OnValueChanged);
            this.entry.OnEntryValueChangedUntyped.Subscribe(onEntryValueChangedUntyped);

            if (!bool.TryParse(this.entry.BoxedValue.ToString(), out this.currentValue))
            {
                Log.LogOutput($"{this.GetType()}: Could not parse '{Name}':", Log.LogLevel.Warning);
            }
        }
    }

    public bool GetValue()
    {
        return currentValue;
    }
    public override void SetValue(object newValue)
    {
        if (entry != null)
            entry.BoxedValue = newValue;
        else
            this.OnValueChanged(currentValue, newValue);
    }
    internal override void OnDestroy()
    {
        this.entry?.OnEntryValueChangedUntyped.Unsubscribe(onEntryValueChangedUntyped);
    }
    private void OnValueChanged(object oldValue, object newValue)
    {
        if (this.entry != null)
        {
            if (!bool.TryParse(this.entry.BoxedValue.ToString(), out currentValue))
            {
                Log.LogOutput($"{this.GetType()}.OnValueChanged: Could not parse '{Name}':", Log.LogLevel.Warning);
                return;
            }
        }
        else
        {
            try
            {
                currentValue = (bool)newValue;
            }
            catch (Exception e)
            {
                Log.LogOutput($"{this.GetType()}.OnValueChanged: Could not parse '{Name}': {e}", Log.LogLevel.Warning);
                return;
            }
        }

        this.OnValueChangedUntyped?.Invoke(oldValue, newValue);
        this.OnValueChangedOption?.Invoke(this);
    }
}
public class StringOption : BaseOption
{
    public new Action<StringOption>? OnValueChangedOption;
    internal string currentValue;

    public StringOption(MelonPreferences_Entry? entry = null, string currentValue = "") : base(entry)
    {
        this.entry = entry;
        this.currentValue = currentValue;

        if (this.entry != null)
        {
            this.onEntryValueChangedUntyped = new LemonAction<object, object>(OnValueChanged);
            this.entry.OnEntryValueChangedUntyped.Subscribe(onEntryValueChangedUntyped);

            var boxedString = this.entry.BoxedValue.ToString();
            this.currentValue = boxedString == null ? currentValue : boxedString;
        }
    }
    public string GetValue()
    {
        if (entry != null)
        {
            var boxedString = this.entry.BoxedValue.ToString();

            if (boxedString == null)
            {
                Log.LogOutput($"Could not parse {this.GetType()}'{Name}'", Log.LogLevel.Warning);
                return currentValue;
            }

            this.currentValue = boxedString;
        }

        return currentValue == null ? "" : currentValue;
    }
    public override void SetValue(object newValue)
    {
        if (newValue == null)
            return;

        if (entry != null)
            entry.BoxedValue = newValue;
        else
            this.OnValueChanged(currentValue == null ? "" : currentValue, newValue);
    }
    internal override void OnDestroy()
    {
        this.entry?.OnEntryValueChangedUntyped.Unsubscribe(onEntryValueChangedUntyped);
    }
    private void OnValueChanged(object oldValue, object newValue)
    {
        if (newValue == null)
            return;

        var newValueString = newValue.ToString();

        if (newValueString == null)
            return;

        currentValue = newValueString;

        this.OnValueChangedUntyped?.Invoke(oldValue, newValue);
        this.OnValueChangedOption?.Invoke(this);
    }
}
public class ButtonOption : BaseOption
{
    public Action<ButtonOption>? OnClick;

    public string ButtonName
    {
        get
        {
            return buttonName;
        }
        set
        {
            buttonName = (value == null || value.Length == 0) ? defaultButtonName : value;

            OnInfoUpdated?.Invoke(this);
        }
    }

    protected string buttonName = "";

    public ButtonOption(string buttonName = "") : base(null)
    {
        ButtonName = buttonName;
    }

    public virtual void Click()
    {
        OnClick?.Invoke(this);
    }
}