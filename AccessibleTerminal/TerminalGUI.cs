using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibleTerminal
{
    internal class TerminalGUI : MonoBehaviour
    {
        // Singleton instance of this script.
        internal static TerminalGUI Instance;

        internal Terminal terminal; // Terminal instance.
        internal bool terminalReady = false; // Whether 'Terminal.modifyingText' is true or not.

        // GUI positions and scaling.
        private int currentPage = 1, pages, col, row, limit;
        private float menuX, menuY, menuWidth, menuHeight, buttonX, buttonY, buttonWidth, buttonHeight, betweenSpaceX, betweenSpaceY;
        // ...

        // Button and text styling.
        private GUIStyle boxStyle, buttonStyle, textFieldStyle;
        private bool stylesInitialized = false; // Whether styles have been initialized or not.

        // Stack used for deleting last-inserted word in player terminal input.
        private readonly Stack<string> undo = [];

        private List<string> buttonList = []; // List of buttons displayed in the HUD.
        private bool buttonPressed = false; // Whether a button has just been pressed.
        private int buttonCount = 0; // The number of buttons shown in the current page.

        // Lists of words for terminal input, separated by category.
        private List<string> actions, items, moons, enemies, players, decor, upgrades, numbers, codes, boosters, logs;

        // List of buttons shown in the main menu.
        private readonly List<string> mainMenu = ["Actions", "Items", "Moons", "Enemies", "Players", "Decor", "Upgrades", "Story Logs", "Codes", "Boosters", "Numbers"];
        private bool isOnMainMenu = true; // Whether the main menu is currently shown.

        // Terminal commands found in 'TerminalNodesList.allKeywords' that are either unnecessary or don't work at all.
        private readonly string[] unused = ["confirm", "deny", "reset credits"]; // TODO: Add configuration to decide which.

        private void Awake()
        {
            // Singleton class.
            if (Instance != null)
            {
                return;
            }
            Instance = this;

            // Obtain terminal instance.
            terminal = FindObjectOfType<Terminal>();
        }

        private void Start()
        {
            // GUI Scaling for HUD elements.
            col = 8; // TODO: Add configuration for some of these fields.
            row = 2;
            limit = col * row;

            menuWidth = 1200;
            menuHeight = 600;
            menuX = (Screen.currentResolution.width - menuWidth) * 0.5f;
            menuY = Screen.currentResolution.height - menuHeight;

            betweenSpaceX = 4;
            betweenSpaceY = 8;

            buttonWidth = (menuWidth / col) - betweenSpaceX;
            buttonHeight = menuHeight / (row * 2);
            buttonX = menuX + (betweenSpaceX * 0.5f);
            buttonY = menuY + (menuHeight / menuWidth * 100);
            // ...

            // List of all terminal keywords.
            List<TerminalKeyword> allKeywords = new(terminal.terminalNodes.allKeywords);

            /// Fill button lists according to each category:
            // Keywords or actions (generally the first word in terminal commands).
            actions = allKeywords.Where(action => (action.isVerb || action.specialKeywordResult) && !unused.Contains(action.word))
                .Select(action => action.word.ToUpper()[0].ToString() + action.word[1..]).ToList();
            actions.Add("Transmit"); // Signal translator upgrade (not found in 'TerminalNodesList.allKeywords' for some reason).

            // Add custom terminal commands found in the 'LethalAPI.Terminal' registry.
            // TODO: Add configuration for whether or not to do this.
            /* TerminalRegistry.EnumerateCommands().Select(command => command.Name.ToUpper()[0].ToString() + command.Name[1..])
                .Where(name => !actions.Contains(name)).Do(actions.Add); */

            // Purchasable items in the store that are always available.
            items = terminal.buyableItemsList.Select(item => item.name).ToList();

            // Moon names (with the number prefix removed, since route pricings are not correct otherwise).
            moons = terminal.moonsCatalogueList.Select(moon => moon.PlanetName.Trim())
                .Select(name => Regex.Match(name, @"(?!\d*\s).*").Value).ToList();

            // Enemy names (made singular by removing any 's' at the end, but it might not always be correct).
            // TODO: Add configuration to only show previously scanned enemies in the button list.
            enemies = terminal.enemyFiles.Select(enemy => enemy.creatureName.Trim())
                .Select(name => Regex.Match(name, @"^.*([^s]$|(?=s$))").Value).ToList();

            // List of players (updated every time someone joins or leaves the lobby).
            players = StartOfRound.Instance.allPlayerScripts.Select(player => player.playerUsername).ToList();

            List<UnlockableItem> allUnlockables = StartOfRound.Instance.unlockablesList.unlockables;

            // Ship decorations and suits.
            // TODO: Add configuration to only show the current rotating shop in the button list.
            decor = allUnlockables.Where(decor => decor.shopSelectionNode && !decor.alwaysInStock)
                .Select(decor => decor.unlockableName).ToList();

            // Ship upgrades.
            upgrades = allUnlockables.Where(upgrade => upgrade.IsPlaceable && upgrade.alwaysInStock)
                .Select(upgrade => upgrade.unlockableName).ToList();

            // Sigurd terminal logs.
            // TODO: Add configuration to only show obtained logs in the button list.
            logs = terminal.logEntryFiles.Select(node => node.creatureName).ToList();

            // Access codes for facility doors/mines/turrets.
            codes = allKeywords.Where(code => code.accessTerminalObjects).Select(code => code.word).ToList();

            // Names for radar boosters.
            boosters = [.. StartOfRound.Instance.randomNames];

            // A list of numbers in a specified range, mostly for convenience.
            // TODO: Add configuration to the range of numbers.
            numbers = Enumerable.Range(0, 48).Select(num => num.ToString()).ToList();
            /// ...
        }

        private void OnGUI()
        {
            // Initialize button and text styling (should run once).
            InitStyles();

            // Check if terminal is null (should never be needed, but just in case).
            if (terminal == null)
            {
                terminal = FindObjectOfType<Terminal>();
            }

            // Avoid doing this calculation every cycle.
            if (buttonList.Count != buttonCount)
            {
                pages = Mathf.CeilToInt(buttonList.Count / ((float)limit));
                buttonCount = buttonList.Count;
            }
            string boxText = $"Page {currentPage} of {pages}.";

            // TODO: Add configuration for background color.
            GUI.backgroundColor = new Color32(0, 0, 15, 130);
            GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), boxText, boxStyle);

            // Submit button.
            CreateButton(buttonX, Screen.currentResolution.height - (menuHeight * 0.125f) + 1,
                (menuWidth * 0.5f) - betweenSpaceX, menuHeight * 0.125f, "Submit command", buttonStyle,
                () =>
                {
                    // Check if currently on a confirmation node.
                    if (IsActualConfirmationNode())
                    {
                        // Send 'CONFIRM' on confirmation screen.
                        SetTerminalInput("CONFIRM");
                    }

                    // Check if input command is empty.
                    if (GetTerminalInput().Length > 0)
                    {
                        // Submit command.
                        terminal.OnSubmit();
                    }

                    // Clear undo stack.
                    undo.Clear();
                });

            // Back button.
            CreateButton(buttonX + (menuWidth * 0.5f), Screen.currentResolution.height - (menuHeight * 0.125f) + 1,
                (menuWidth * 0.5f) - betweenSpaceX, menuHeight * 0.125f, "Back / delete word", buttonStyle,
                () =>
                {
                    // Check if not on main menu.
                    if (!isOnMainMenu)
                    {
                        // Return to main menu.
                        ToMainMenu();
                    }
                    // Check if currently on a confirmation node.
                    else if (IsActualConfirmationNode())
                    {
                        // Send 'DENY' on confirmation screen.
                        SetTerminalInput("DENY");
                        terminal.OnSubmit();
                    }
                    // Check if undo stack is empty.
                    else if (undo.Count > 0)
                    {
                        // Delete newest word.
                        SetTerminalInput(undo.Pop());
                    }
                    else
                    {
                        // Exit terminal.
                        terminal.QuitTerminal();
                    }
                });

            // Previous page button.
            CreateButton(buttonX, Screen.currentResolution.height - (menuHeight * 0.25f) - betweenSpaceY + 1,
                (menuWidth * 0.5f) - betweenSpaceX, menuHeight * 0.125f, "◀ Previous Page", buttonStyle,
                () =>
                {
                    // Decrement current page if possible.
                    if (currentPage > 1)
                    {
                        currentPage--;
                    }
                });

            // Next page button.
            CreateButton(buttonX + (menuWidth * 0.5f), Screen.currentResolution.height - (menuHeight * 0.25f) - betweenSpaceY + 1,
                (menuWidth * 0.5f) - betweenSpaceX, menuHeight * 0.125f, "Next Page ▶", buttonStyle,
                () =>
                {
                    // Increment current page if possible.
                    if (currentPage < pages)
                    {
                        currentPage++;
                    }
                });

            // Scroll up button.
            CreateButton(buttonX - betweenSpaceX - (menuHeight * 0.0625f), menuY,
                menuHeight * 0.0625f, (menuHeight * 0.5f) - (betweenSpaceY * 0.25f), "▲", buttonStyle,
                () =>
                {
                    // Obtain terminal scroll bar.
                    Scrollbar scrollbar = terminal.scrollBarVertical;

                    // TODO: Scroll amount configuration.
                    scrollbar.value = Mathf.Clamp01(scrollbar.value + 0.5f);
                });

            // Scroll down button.
            CreateButton(buttonX - betweenSpaceX - (menuHeight * 0.0625f), menuY + (betweenSpaceY * 0.25f) + (menuHeight * 0.5f) + 1,
                menuHeight * 0.0625f, (menuHeight * 0.5f) - (betweenSpaceY * 0.25f), "▼", buttonStyle,
                () =>
                {
                    // Obtain terminal scroll bar.
                    Scrollbar scrollbar = terminal.scrollBarVertical;

                    // TODO: Scroll amount configuration.
                    scrollbar.value = Mathf.Clamp01(scrollbar.value - 0.5f);
                });

            // Return if button list is empty.
            if (buttonList.Count == 0)
            {
                return;
            }

            // Obtain index and limit relative to current page.
            int relativeIndex = limit * (currentPage - 1),
                relativeLimit = relativeIndex + ((currentPage == pages) ? ((buttonList.Count - 1) % limit) : limit - 1);

            // Iterate for every button to create for the current page.
            for (int i = relativeIndex; i <= relativeLimit; i++)
            {
                // Exit early if a button was pressed.
                if (buttonPressed)
                {
                    break;
                }

                // Create input button at relative position.
                CreateButton(buttonX + ((buttonWidth + betweenSpaceX) * (i % limit % col)),
                    buttonY + ((buttonHeight + betweenSpaceY) * Mathf.FloorToInt(i % limit / col)),
                    buttonWidth, buttonHeight, buttonList[i], buttonStyle,
                    () =>
                    {
                        // Check if currently on main menu.
                        if (isOnMainMenu)
                        {
                            // Handle main menu button press (change to respective menu).
                            switch (i)
                            {
                                case 0:
                                    ChangeMenu(actions);
                                    break;
                                case 1:
                                    ChangeMenu(items);
                                    break;
                                case 2:
                                    ChangeMenu(moons);
                                    break;
                                case 3:
                                    ChangeMenu(enemies);
                                    break;
                                case 4:
                                    ChangeMenu(players);
                                    break;
                                case 5:
                                    ChangeMenu(decor);
                                    break;
                                case 6:
                                    ChangeMenu(upgrades);
                                    break;
                                case 7:
                                    ChangeMenu(logs);
                                    break;
                                case 8:
                                    ChangeMenu(codes);
                                    break;
                                case 9:
                                    ChangeMenu(boosters);
                                    break;
                                case 10:
                                    ChangeMenu(numbers);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            // Obtain current terminal input and button text.
                            string currentText = GetTerminalInput(),
                                word = buttonList[i];

                            // Check if current terminal input is not empty.
                            if (currentText.Length > 0)
                            {
                                // Prefix button text with a space, and push current input to the undo stack.
                                word = " " + word;
                                undo.Push(currentText);
                            }
                            else
                            {
                                // Push a blank string to the undo stack (to clear the last word).
                                undo.Push("");
                            }

                            // Set terminal input to appended text and return to main menu.
                            SetTerminalInput(currentText + word);
                            ToMainMenu();
                        }
                    });
            }

            // Reset for next cycle.
            buttonPressed = false;
        }

        /// <summary>
        ///     Initialize button and text styles.
        /// </summary>
        private void InitStyles()
        {
            // Ensure this runs only once.
            if (stylesInitialized)
            {
                return;
            }
            stylesInitialized = true;

            /// Styling.
            // TODO: Add color configuration.
            Color[] pix = new Color[4];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = Color.white;
            }

            Texture2D background = new(2, 2);
            background.SetPixels(pix);
            background.Apply();

            // Box style
            boxStyle = new(GUI.skin.box);
            boxStyle.normal.textColor = new Color32(30, 255, 0, 255);
            boxStyle.normal.background = background;
            boxStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            boxStyle.fontSize = 12;
            boxStyle.fontStyle = FontStyle.Normal;
            boxStyle.alignment = TextAnchor.UpperCenter;

            // Button style
            buttonStyle = new(GUI.skin.button);
            buttonStyle.normal.textColor = new Color32(20, 142, 0, 255);
            buttonStyle.normal.background = background;
            buttonStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            buttonStyle.hover.textColor = new Color32(36, 255, 0, 255);
            buttonStyle.hover.background = background;
            buttonStyle.hover.background.hideFlags = HideFlags.HideAndDontSave;

            buttonStyle.fontSize = 18;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.wordWrap = true;

            // Text field style
            textFieldStyle = new(GUI.skin.button);
            textFieldStyle.normal.textColor = new Color32(20, 142, 0, 255);
            textFieldStyle.normal.background = background;
            textFieldStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

            textFieldStyle.fontSize = 9;
            textFieldStyle.fontStyle = FontStyle.Bold;
            /// ...
        }

        /// <summary>
        ///     Helper method for creating buttons.
        /// </summary>
        /// <param name="x">Position in 'x' of the button.</param>
        /// <param name="y">Position in 'y' of the button.</param>
        /// <param name="width">Width of the button.</param>
        /// <param name="height">Height of the button.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="style">GUIStyle of the button.</param>
        /// <param name="action">Arrow function to execute on button press.</param>
        private void CreateButton(float x, float y, float width, float height, string text, GUIStyle style, Action action)
        {
            bool button = GUI.Button(new Rect(x, y, width, height), text, style);
            if (button)
            {
                action.Invoke();
                buttonPressed = true;
            }
        }

        /// <summary>
        ///     Refreshes player button list (in case someone joins or leaves the game).
        /// </summary>
        internal void RefreshPlayers()
        {
            players = StartOfRound.Instance.allPlayerScripts.Select(player => player.playerUsername).ToList();
        }

        /// <summary>
        ///     Toggles visibility of clickable terminal HUD.
        /// </summary>
        public void ToggleVisibility()
        {
            enabled = !enabled;
        }

        /// <summary>
        ///     Changes the currently shown list of buttons to the given one.
        /// </summary>
        /// <param name="buttonList">List of buttons to display.</param>
        public void ChangeMenu(List<string> buttonList)
        {
            this.buttonList = buttonList;
            isOnMainMenu = false;
        }

        /// <summary>
        ///     Return to main menu.<para/>
        ///     Load main menu button list, toggle <c>isOnMainMenu</c>, and reset current page number.
        /// </summary>
        public void ToMainMenu()
        {
            buttonList = mainMenu;
            isOnMainMenu = true;
            currentPage = 1;
        }

        /// <summary>
        ///     Obtain substring of text added by player, or a blank string if no text has been written.
        /// </summary>
        /// <returns>Current player terminal input.</returns>
        public string GetTerminalInput()
        {
            return terminal.currentText[^terminal.textAdded..];
        }

        /// <summary>
        ///     Replace current player terminal input with the given text.
        /// </summary>
        /// <param name="newText">Text to replace terminal input with.</param>
        public void SetTerminalInput(string newText)
        {
            // Insert a blank string into the terminal to manually toggle the 'Terminal.modifyingText' field.
            if (!terminalReady)
            {
                // Without this check, the first button press after opening the terminal is not registered.
                terminal.TextChanged("");
                terminal.textAdded = 0;

                // Clear undo stack and push a blank string to it.
                undo.Clear();
                undo.Push("");
            }

            // Append new text to what's currently being shown in the terminal (without player input).
            terminal.TextChanged(terminal.currentText[..^terminal.textAdded] + newText);

            // Update screen text in terminal, and set the value of added text to the length of the new text.
            terminal.screenText.text = terminal.currentText;
            terminal.textAdded = newText.Length;
        }

        /// <summary>
        ///     Checks if the current terminal node is asking for a <c>CONFIRM</c> or <c>DENY</c> input.
        /// </summary>
        /// <remarks>
        ///     Not exactly elegant, and it probably doesn't work for different language versions of the game, but during testing I could not find
        ///     a more reliable way of checking for this. For now it's staying like this, but will probably be replaced eventually.<para/>
        ///     The <see cref="TerminalNode.isConfirmationNode"/> field isn't exactly always accurate, or else I'd simply use it.
        /// </remarks>
        /// <returns>Whether or not the current node is a confirmation node.</returns>
        public bool IsActualConfirmationNode()
        {
            return terminal.currentText.Contains("Please CONFIRM or DENY.")
                && !terminal.currentText.Contains("Cancelled order.");
        }
    }
}