# AccessibleTerminal

[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/pacoito/AccessibleTerminal?style=for-the-badge&logo=thunderstore&color=mediumseagreen
)](https://thunderstore.io/c/lethal-company/p/pacoito/AccessibleTerminal/)
[![GitHub Releases](https://img.shields.io/github/v/release/pacoito123/AccessibleTerminal?display_name=tag&style=for-the-badge&logo=github&color=steelblue
)](https://github.com/pacoito123/LC_AccessibleTerminal/releases)
[![License](https://img.shields.io/github/license/pacoito123/AccessibleTerminal?style=for-the-badge&logo=github&color=teal
)](https://github.com/pacoito123/LC_AccessibleTerminal/blob/main/LICENSE)

> Execute just about any terminal command, without the use of a keyboard.

## Description

Adds a clickable GUI to the ship terminal, which allows the player to 'build' full commands without a keyboard by inserting words into the player input.

While this mod was made primarily for accessibility purposes, it might be useful to anyone who simply doesn't like typing commands at the terminal, since it could be faster to click through terminal options.

Originally a fork of [`TerminalGamepad`](https://thunderstore.io/c/lethal-company/p/secrecthide/TerminalGamepad) by [secrecthide](https://github.com/secrecthide), but focused on mouse input rather than gamepad input.

![alt](https://files.catbox.moe/1wa90t.gif "Showcase of the clickable terminal being used.")

## Configuration

For the moment, there is no configuration file available, but the following configuration options are planned and should not take too long to implement:

- The list of commands to exclude from the **Actions** category (currently `confirm`, `deny`, and `reset credits` are not shown).
- The number of rows and columns in each button page (currently **2 rows**, **8 columns**).
- Overall menu width and height (currently **1200 pixels** by **600 pixels**).
- The space between each button (currently **4 pixels** horizontally and **8 pixels** vertically).
- Whether custom commands found in the `LethalAPI.Terminal` registry should be added to the **Actions** category (currently **disabled**, though custom commands directly added to `TerminalNodesList.allKeywords` *will* show up).
- Whether to show only previously scanned enemies in the **Enemies** category (**all enemies** are currently shown).
- Whether to show only items currently available in the rotating shop in the **Decor** category (**all rotating store items** are currently shown).
- Whether to show only discovered story logs in the **Story Logs** category (**every Sigurd log** is currently shown).
- The range of numbers to display in the **Numbers** category (currently **0** to **48**, but only the digits from 0-9 are really needed, the rest are for convenience).
- The amount to scroll when pressing the up/down buttons (currently **0.5**, referring to scrolbar position).
- Color and transparency styling for menu elements and text (currently using the default style of [`TerminalGamepad`](https://thunderstore.io/c/lethal-company/p/secrecthide/TerminalGamepad)).

## Compatibility

Any modded content that falls under the standard 'categories' (e.g. items, moons, enemies) should appear as a clickable option within the button lists, as long as it is added to the respective lists used by the game (e.g. `Terminal.buyableItemsList` for store items).

Custom terminal commands directly added to `TerminalNodesList.allKeywords`, ~~as well as commands registered through the [`LethalAPI.Terminal`](https://thunderstore.io/c/lethal-company/p/LethalAPI/LethalAPI_Terminal) library~~ (currently **disabled**, will be added soon as a soft dependency), should also show up within the **Actions** category.

Mods such as [`Lategame Upgrades`](https://thunderstore.io/c/lethal-company/p/malco/Lategame_Upgrades) or [`TooManyEmotes`](https://thunderstore.io/c/lethal-company/p/FlipMods/TooManyEmotes), which add their own 'category' of items (upgrades/perks separate from ship upgrades and emotes, respectively), will need to be manually supported to show up in the list of buttons, though the 'main' command _should_ show up within the **Actions** category.

If you would like any specific mod to be supported, find any compatibility issue with another mod, or are simply having an issue, please let me know in the [relevant thread](https://discord.com/invite/lcmod) in the Lethal Company Modding Discord server, or [open an issue on GitHub](https://github.com/pacoito123/LC_AccessibleTerminal/issues). Feedback is also welcome!

## Credits

- [secrecthide](https://github.com/secrecthide) — For [`TerminalGamepad`](https://thunderstore.io/c/lethal-company/p/secrecthide/TerminalGamepad), the original mod this was initially forked from.
- [Azim](https://github.com/Azim) — For [`TerminalMouse`](https://thunderstore.io/c/lethal-company/p/Azim/TerminalMouse), to allow mouse input in the terminal, integrated into this mod to remove the `TerminalAPI` dependency.
- [Shaosil](https://github.com/Shaosil) — For the tweak in [`GeneralImprovements`](https://thunderstore.io/c/lethal-company/p/ShaosilGaming/GeneralImprovements) to lock the camera while using the terminal, added to this mod to avoid nausea when moving the mouse to click buttons. The [`GeneralImprovements`](https://thunderstore.io/c/lethal-company/p/ShaosilGaming/GeneralImprovements) repository does not have a license file (as far as I can tell), so if there's any problem with me having done this, please let me know.
- [NotAtomicBomb](https://github.com/NotAtomicBomb) — For [`TerminalAPI`](https://thunderstore.io/c/lethal-company/p/NotAtomicBomb/TerminalApi), from which I largely based the `TerminalGUI.GetTerminalInput()` and `TerminalGUI.SetTerminalInput()` methods off of.
- [Hazikara](https://www.twitch.tv/kitsumodo) — For the [original request](https://discord.com/channels/1168655651455639582/1214149145909010463) in the Lethal Company Modding Discord server, which gave me the idea of adapting [`TerminalGamepad`](https://thunderstore.io/c/lethal-company/p/secrecthide/TerminalGamepad) for mouse input. You are very inspiring!
- _You!_ — ![alt](https://cdn.betterttv.net/emote/642f4905a3c841a2f9ef2a94/1x.webp "pepeSTARE")