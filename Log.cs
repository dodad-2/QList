using MelonLoader;

namespace QList;

internal static class Log // TODO rewrite this
{
    internal static LogLevel logLevel = LogLevel.None;
    internal static MelonMod? mod;

    internal static bool SetMod(MelonMod newMod, LogLevel logLevel = LogLevel.None)
    {
        if (newMod == null)
        {
            newMod?.LoggerInstance.Error($"Unable to initialize null mod.");
            return false;
        }

        mod = newMod;

        var category = MelonPreferences.GetCategory("General");
        MelonPreferences_Entry? entry = null;

        if (category == null)
        {
            category = MelonPreferences.CreateCategory("General");
            category.SetFilePath(PreferencesConfig.filePath);
            entry = category.CreateEntry<int>("LOG_LEVEL", 5, "Log Level");
            category.SaveToFile();
        }

        entry ??= category.GetEntry("LOG_LEVEL");

        string[] valueNames = new string[8];
        valueNames[0] = "None";
        valueNames[1] = "Message";
        valueNames[2] = "Info";
        valueNames[3] = "Warning";
        valueNames[4] = "Error";
        valueNames[5] = "Fatal";
        valueNames[6] = "Debug";
        valueNames[7] = "All";

        var logLevelOption = new OptionTypes.DropdownOption(entry, 7, valueNames);
        logLevelOption.OnValueChangedUntyped += OnValueUpdatedUntyped;
        Options.AddOption(logLevelOption);

        Log.logLevel = logLevel == LogLevel.None ? (LogLevel)(int)entry.BoxedValue : logLevel;

        return true;
    }

    internal static void LogOutput(object data, LogLevel level = LogLevel.Debug)
    {
        if (level > logLevel || logLevel == LogLevel.None || mod == null)
            return;

        switch (level)
        {
            case LogLevel.Message:
                mod.LoggerInstance.Msg($"{data}");
                break;
            case LogLevel.Info:
                mod.LoggerInstance.Msg($"{data}");
                break;
            case LogLevel.Warning:
                mod.LoggerInstance.Warning($"{data}");
                break;
            case LogLevel.Error:
                mod.LoggerInstance.Error($"{data}");
                break;
            case LogLevel.Fatal:
                mod.LoggerInstance.BigError($"{data}");
                break;
            case LogLevel.Debug:
                mod.LoggerInstance.Msg($"{data}");
                break;
        }
    }

    internal static void OnValueUpdatedUntyped(object oldValue, object newValue)
    {
        logLevel = (LogLevel)Convert.ToInt32(newValue);
    }

    public enum LogLevel
    {
        None = 0,
        Message = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
        Debug = 6,
        All = 7
    }
}
