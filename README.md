# Description

Easily configure mods for the game Silica through the main menu settings screen or the pause menu

![Mod Options Window](image.png)

# Instructions - Users

1. Install Silica from Steam

2. Install [.NET 6.0 Runtime x64](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

3. Download and run the [MelonLoader.Installer.exe](https://github.com/LavaGang/MelonLoader/releases/tag/v0.6.2)

4. Using the Automated tab, hit the <b>SELECT</b> button for Unity Game and specify `C:\Program Files (x86)\Steam\steamapps\common\Silica\Silica.exe` or your custom path to the Silica.exe in your game installation directory

5. Ensure that the <b>Version</b> is set to v0.6.2 or later

6. Ensure that the <b>Game Arch</b> is set to x64

7. Select <b>Install</b>

8. Place QList.dll into `Silica\Mods` and launch the game

A new `Mod Options` button will be visible in the main menu settings or the ingame pause menu. Mods that have added custom options will appear in the list on the left-hand side. Click a mod name to show available options in the right-hand pane.

# Instructions - Developers

1. Register your mod by calling `QList.Options.RegisterMod(myMelonMod)`

2. Create an option then optionally link it to a Melon Preference or subscribe to `OnValueChangedUntyped`

3. Call `QList.Options.AddOption(myOption)`. Note that any values not provided here will be pulled from the Preference in the Option. Description must be set here or in the preference to appear in the menu.

4. Optionally review the <a href="https://github.com/dodad-2/SilicaTemplate">Template Mod</a> for a few examples

## Melon Preferences

To link a MelonPreference to an option you only need to pass a `MelonPreferences_Entry` reference to an appropriate option's constructor. Passing a null reference will create a non-persistent option.

On a value change `OnValueChangedUntyped` will be invoked whether a MelonPreference reference exists or not.

## Notes

- Sliders are supported for IntOption and FloatOption

## Option Types

Option types are located at `QList.OptionTypes`. Currently available:

- IntOption
- FloatOption
- BoolOption
- StringOption
- ButtonOption
- KeybindOption
- DropdownOption

# Known Issues

- Slider step value doesn't work

# Special thanks

- databomb for his pioneering efforts in modding Silica
- MelonLoader Discord for support
- The <a href="https://discord.gg/5SHQxFaess">Silica Modders Discord</a>

# Changelog

**0.3.0**

- KeyBindOption: Added `IsKeyDown`
- KeyBindOption: Added `KeyPressedThisFrame`
- KeyBindOption: Added `KeyReleasedThisFrame`

**0.2.0**

- Options: User editing of options can now be toggled via `BaseOption.AllowUserEdits` and listeners are notified of this change via `BaseOption.OnAllowUserEditsUpdated`. This field does not affect `BaseOption.SetValue` and manual changes are always applied
- Options: Added `DropdownOption`
- Options: `KeybindOption` properly updates when the MelonPreference value is changed
- Options: Sliders without a MelonEntry properly load and save values

**0.1.1**

- Mod Options Window: Reopens to last mod while ingame
- Keybinds: Changed the cancel change keybind hotkey from Capslock to Backspace
- Keybinds: UI fixes

**0.1.0**

- Options: Added `KeybindOption`

**0.0.7**

- Logging more clear
- Cleanup

**0.0.6**

- Mod List: List is now sorted alphabetically

**0.0.5**

- Options: Added `Action<BaseOption>? OnValueChangedOption`

**0.0.4**

- Fixed ButtonType issues

**0.0.3**

- Fixed OptionType issues

**0.0.2**

- Fixed various UI issues
- Fixed a bug in the game where sometimes the main menu would disappear

**0.0.1**

- First release
