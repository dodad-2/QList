namespace QList;

using QList.OptionTypes;

using MelonLoader;
using System.Reflection;

public static class Options
{
    internal static Dictionary<string, ModOptionContainer> CurrentModOptions = new();

    public static void RegisterMod(MelonMod mod)
    {
        if (mod == null)
        {
            Log.LogOutput($"Unable to register mod: ensure mod is not null", Log.LogLevel.Error);
            return;
        }

        var key = $"{mod.Info.Author}.{mod.Info.Name}";

        if (CurrentModOptions.ContainsKey(key))
        {
            Log.LogOutput($"Unable to register mod: mod already registered", Log.LogLevel.Warning);
            return;
        }

        CurrentModOptions.Add(key, new ModOptionContainer(mod, System.Reflection.Assembly.GetCallingAssembly()));

        Log.LogOutput($"Registered mod: '{mod.Info.Name}'", Log.LogLevel.Message);
    }

    public static void DeregisterMod(MelonMod mod)
    {
        if (mod == null)
        {
            Log.LogOutput($"Unable to deregister mod: ensure mod is not null", Log.LogLevel.Error);
            return;
        }

        var key = $"{mod.Info.Author}.{mod.Info.Name}";

        if (!CurrentModOptions.ContainsKey(key))
        {
            Log.LogOutput($"Unable to deregister mod: mod not registered", Log.LogLevel.Warning);
            return;
        }

        CurrentModOptions.Remove(key);

        Log.LogOutput($"Deregistered mod: '{mod.Info.Name}'", Log.LogLevel.Message);
    }
    /// <summary>
    /// Adds an option for a registered mod.
    /// </summary>
    /// <param name="name">Used if no MelonPreference exists in the given option</param>
    /// <param name="description">Used if no MelonPreference exists in the given option</param>
    /// <param name="category">Used if no MelonPreference exists in the given option</param>
    /// <returns>False if unable to add option</returns>
    public static bool AddOption(BaseOption option, string? name = "", string? description = "", string? category = "")
    {
        if (option == null)
        {
            Log.LogOutput($"Unable to add option: option is null", Log.LogLevel.Warning);
            return false;
        }

        var assembly = System.Reflection.Assembly.GetCallingAssembly();

        var search = CurrentModOptions.Where(x => x.Value.assembly.Equals(assembly));

        if (search == null || search.Count() == 0)
        {
            Log.LogOutput($"Unable to add option: Mod not registered. Call Options.RegisterMod first. ({assembly})", Log.LogLevel.Warning);
            return false;
        }

        var container = search?.First();

        if (container == null || !container.HasValue)
        {
            Log.LogOutput($"Unable to add option: Mod not registered. Call Options.RegisterMod first. ({assembly}", Log.LogLevel.Warning);
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
}

internal class ModOptionContainer
{
    #region Variables & Constructor
    public bool IsDirty
    {
        get
        {
            return dirty;
        }
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