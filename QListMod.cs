﻿using MelonLoader;
using QList;

[assembly: MelonInfo(typeof(QListMod), "QList", "0.3.0", "dodad")]
[assembly: MelonGame("Bohemia Interactive", "Silica")]
[assembly: MelonPriority(-99)]
[assembly: MelonColor(100, 255, 180, 100)]

namespace QList;

public class QListMod : MelonMod
{
    internal static readonly string BundleKey = "QList.qlist_bundle";

    //internal static readonly string bundleKey = "QList.qlist_bundle";

    internal static QListMod? Instance;

    public override void OnInitializeMelon()
    {
        Instance = this;
        Config.SetFilePath(this);

        Options.RegisterMod(this);

        if (Log.Enable(this))
        {
            /*
            var firstCategory = MelonPreferences.CreateCategory("Example Category");
            firstCategory.SetFilePath(PreferencesConfig.filePath);

            var firstIntPreference = MelonPreferences.CreateEntry<int>(firstCategory.Identifier, "FIRST_INT", 1, "First Integer", "This option is linked to a Melon Preference and Category and will automatically save changes made by users.");
            var firstIntOptionPreference = new OptionTypes.IntOption(firstIntPreference);
            Options.AddOption(firstIntOptionPreference);

            var firstFloatPreference = MelonPreferences.CreateEntry<float>(firstCategory.Identifier, "FIRST_FLOAT", 0.5f, "First Float", "This option is linked to a Melon Preference and Category and will automatically save changes made by users.");
            var firstFloatOptionPreference = new OptionTypes.FloatOption(firstFloatPreference, true, 0, -10, 10, 0.25f); // step doesn't work
            Options.AddOption(firstFloatOptionPreference);

            var firstBoolPreference = MelonPreferences.CreateEntry<bool>(firstCategory.Identifier, "FIRST_BOOL", true, "First Bool", "This option is linked to a Melon Preference and Category and will automatically save changes made by users.");
            var firstBoolOptionPreference = new OptionTypes.BoolOption(firstBoolPreference);
            Options.AddOption(firstBoolOptionPreference);

            var firstStringPreference = MelonPreferences.CreateEntry<string>(firstCategory.Identifier, "FIRST_STRING", "Lorem ipsum", "First String", "This option is linked to a Melon Preference and Category and will automatically save changes made by users.");
            var firstStringOptionPreference = new OptionTypes.StringOption(firstStringPreference);
            Options.AddOption(firstStringOptionPreference);

            var secondBoolOption = new OptionTypes.BoolOption(null, true);
            Options.AddOption(secondBoolOption, "Second Bool");

            buttonOption = new OptionTypes.ButtonOption();
            //buttonOption.OnClick += ButtonTest;
            Options.AddOption(buttonOption, "First Button", "Button description here"); // If you don't create a description here then updates to Description will do nothing
            
            */

            Log.LogOutput($"{Info.Name} {Info.Version} initialized");
        }
    }

    public override void OnLateInitializeMelon()
    {
        Resources.Initialize();
        QList.UI.Manager.Initialize();
        Options.SortModList();
    }

    public override void OnDeinitializeMelon()
    {
        Options.DeregisterMod(this);
    }

    public override void OnUpdate()
    {
        Options.OnUpdate();
    }
    /*
    private static void ButtonTest(OptionTypes.ButtonOption button) // Option info can be updated at any time
    {
        button.Name = "New Name";

        if (button.Description.Equals("A"))
            button.Description = "B";
        else
            button.Description = "A";

        if (button.ButtonName.Equals("Activate"))
            button.ButtonName = "Deactivate";
        else
            button.ButtonName = "Activate";
    }*/
}
