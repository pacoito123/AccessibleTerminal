### [v1.0.0]

Initial release.
- Updated `README.md` file.
- Added `CHANGELOG.md` file.
- Added `icon.png` and `manifest.json` files.
- Placed Harmony patching inside a try-catch block (just in case).

---

### Changelog from [`TerminalGamepad` v1.2.3](https://github.com/secrecthide/LC-TerminalGamepad/tree/1.2.3)

This mod started as a fork of [`TerminalGamepad`](https://thunderstore.io/c/lethal-company/p/secrecthide/TerminalGamepad) by [secrecthide](https://github.com/secrecthide), shown below are the list of changes made to it:

### General
- Rebranded to `AccessibleTerminal`.
- Removed [`TerminalAPI`](https://thunderstore.io/c/lethal-company/p/NotAtomicBomb/TerminalApi) and [`LethalCompany_InputUtils`](https://thunderstore.io/c/lethal-company/p/Rune580/LethalCompany_InputUtils) dependencies.
- Updated project license (still MIT).

### Config
- Removed mod configuration options temporarily (will be added back soon).

### Input
- Added mouse support for terminal.
- Removed gamepad support (might implement this back, but for now the mod is focused on mouse input only).
- Integrated [`TerminalMouse`](https://thunderstore.io/c/lethal-company/p/Azim/TerminalMouse) by [Azim](https://github.com/Azim) to unlock the mouse cursor when using the terminal (mostly done to get rid of the `TerminalAPI` dependency).
- Integrated tweak from [`GeneralImprovements`](https://thunderstore.io/c/lethal-company/p/ShaosilGaming/GeneralImprovements) by [Shaosil](https://github.com/Shaosil) to lock the camera while the terminal is being used (this project lacks an open-source license, so if there's an issue with me doing this please let me know).

### Terminal GUI
- Made GUI button creation fully dynamic in order to include just about everything in the game, including modded content. This should also make the codebase substantially easier to maintain, as button actions were previously being added manually.
- Used lambda expressions (arrow functions) for just about every list iteration (mostly to fill the button lists).
- Clickable terminal buttons are now separated by categories (e.g. moons, items, decor).
- Button and menu scaling has been simplified (no more mysterious constants to determine menu and button positions, width, and height).
- Submit and back buttons now act as `CONFIRM` and `DENY`, respectively, when on a confirmation screen.
- Added buttons for pagination (next page, previous page), as well as buttons for scrolling up and down in the terminal screen.
- Added checks and fixes for just about everything that can go wrong (and DID go wrong during testing).
- Created a helper method to make button creation somewhat more concise.
- Created method to remake the player button list, to be run every time a player joins or leaves.
- Created method to toggle terminal GUI visibility (might change to an explicit `setVisibility()` instead of a toggle).
- Created methods to switch between main menu and selected categories.
- Added methods to obtain and replace player input in the terminal (largely based off of the implementation in `TerminalAPI`).
- Created method to check if the current node displayed in the terminal is a confirmation screen. I'm not sure if it works if the game language is changed.

### Patches
- Replaced `TerminalAPI` events (`OnTerminalX()`) with Harmony patches.
- Singleton `TerminalGUI` instance is now destroyed upon exiting to main menu.