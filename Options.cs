namespace QList;

using System.Reflection;
using MelonLoader;
using QList.OptionTypes;

public static class Options
{
    internal static Dictionary<string, ModOptionContainer> CurrentModOptions = new();
    private static List<BaseOption> OnUpdateListeners = new();
    private static bool initialized;

    public static void RegisterMod(MelonMod mod)
    {
        if (mod == null)
        {
            Log.LogOutput($"Unable to register mod: ensure mod is not null", Log.ELevel.Error);
            return;
        }

        var key = $"{mod.Info.Author}.{mod.Info.Name}";

        if (CurrentModOptions.ContainsKey(key))
        {
            Log.LogOutput(
                $"Unable to register '{mod.Info.Name}': mod already registered",
                Log.ELevel.Warning
            );
            return;
        }

        CurrentModOptions.Add(
            key,
            new ModOptionContainer(mod, System.Reflection.Assembly.GetCallingAssembly())
        );

        if (initialized) // Only sort after LateInitialize
            SortModList();

        Log.LogOutput($"Registered mod: '{mod.Info.Name}'", Log.ELevel.Message);
    }

    public static void DeregisterMod(MelonMod mod)
    {
        if (mod == null)
        {
            Log.LogOutput($"Unable to deregister mod: ensure mod is not null", Log.ELevel.Error);
            return;
        }

        var key = $"{mod.Info.Author}.{mod.Info.Name}";

        if (!CurrentModOptions.ContainsKey(key))
        {
            Log.LogOutput(
                $"Unable to deregister '{mod.Info.Name}': mod not registered",
                Log.ELevel.Warning
            );
            return;
        }

        CurrentModOptions.Remove(key);

        SortModList();

        Log.LogOutput($"Deregistered mod: '{mod.Info.Name}'", Log.ELevel.Message);
    }

    /// <summary>
    /// Adds an option for a registered mod.
    /// </summary>
    /// <param name="name">Used if no MelonPreference exists in the given option</param>
    /// <param name="description">Used if no MelonPreference exists in the given option</param>
    /// <param name="category">Used if no MelonPreference exists in the given option</param>
    /// <returns>False if unable to add option</returns>
    public static bool AddOption(
        BaseOption option,
        string? name = "",
        string? description = "",
        string? category = ""
    )
    {
        if (option == null)
        {
            Log.LogOutput($"Unable to add option: option is null", Log.ELevel.Warning);
            return false;
        }

        var assembly = System.Reflection.Assembly.GetCallingAssembly();

        var search = CurrentModOptions.Where(x => x.Value.assembly.Equals(assembly));

        if (search == null || search.Count() == 0)
        {
            Log.LogOutput(
                $"Unable to add option '{option.name}': Mod {assembly.GetName()} not registered. Call Options.RegisterMod first. ({assembly})",
                Log.ELevel.Warning
            );
            return false;
        }

        var container = search?.First();

        if (container == null || !container.HasValue)
        {
            Log.LogOutput(
                $"Unable to add option '{option.name}': Mod {assembly.GetName()} not registered. Call Options.RegisterMod first. ({assembly}",
                Log.ELevel.Warning
            );
            return false;
        }

        container.Value.Value.AddOption(option);

        if (category != null && category.Length > 0)
            option.category = category;
        if (name != null && name.Length > 0)
            option.name = name;
        if (description != null && description.Length > 0)
            option.description = description;

        return true;
    }

    internal static void SortModList()
    {
        var sortedKeys = CurrentModOptions.Keys.ToList();
        sortedKeys.Sort();

        var sortedDictionary = new Dictionary<string, ModOptionContainer>();

        foreach (var key in sortedKeys)
            sortedDictionary.Add(key, CurrentModOptions[key]);

        CurrentModOptions = sortedDictionary;

        initialized = true;
    }

    internal static void SubscribeOnUpdate(BaseOption option)
    {
        if (OnUpdateListeners.Contains(option))
            return;

        OnUpdateListeners.Add(option);
    }

    public static void UnsubscribeOnUpdate(BaseOption option)
    {
        if (!OnUpdateListeners.Contains(option))
            return;

        OnUpdateListeners.Remove(option);
    }

    internal static void OnUpdate()
    {
        foreach (var option in OnUpdateListeners)
            if (option != null)
                option.OnUpdate();
    }

    internal static void OnLateUpdate()
    {
        
    }
}

internal class ModOptionContainer
{
    #region Variables & Constructor
    public bool IsDirty
    {
        get { return dirty; }
    }

    public Assembly assembly;
    public MelonMod mod;

    private bool dirty;
    private List<BaseOption> options = new();

    public ModOptionContainer(MelonMod mod, Assembly assembly)
    {
        this.mod = mod;
        this.assembly = assembly;
    }
    #endregion

    #region Options
    public void AddOption(BaseOption option)
    {
        options.Add(option);
        dirty = true;
    }

    public System.Collections.ObjectModel.ReadOnlyCollection<BaseOption> GetOptions()
    {
        dirty = false;
        return options.AsReadOnly();
    }
    #endregion
}
