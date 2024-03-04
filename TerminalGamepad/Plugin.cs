using BepInEx;
using HarmonyLib;
using static TerminalApi.Events.Events;
using System.Reflection;
using TerminalGamepad.GUILoader;
using UnityEngine;
using BepInEx.Configuration;
using TerminalGamepad.Data;

namespace TerminalGamepad
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency("atomic.terminalapi", MinimumDependencyVersion: "1.5.0")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", MinimumDependencyVersion: "0.6.3")]
    public class ModBase : BaseUnityPlugin
    {
        private const string GUID = "Secrecthide.TerminalGamepad";
        private const string Name = "TerminalGamepad";
        private const string Version = "1.2.3";

        private readonly Harmony harmony = new Harmony(GUID);
        private static ModBase instance;

        private TerminalGUI myGUI;
        private KeyBinds keybinds = new KeyBinds();

        public static ConfigEntry<string> BoxBackgroundColor;
        public static ConfigEntry<string> AmountBoxbackgroundColor;
        public static ConfigEntry<string> ButtonsColor;
        public static ConfigEntry<string> HighlightedButtonColor;

        public static ConfigEntry<string> ButtonTextColor;
        public static ConfigEntry<string> HighlightedButtonTextColor;
        public static ConfigEntry<string> AmountButtonTextColor;
        public static ConfigEntry<string> BoxTextColor;

        public static ConfigEntry<string> CustomCommads;
        public static ConfigEntry<int> Rows;
        public static ConfigEntry<int> Columns;
        public static ConfigEntry<PagesScrollType> PagesMode;

        private void Awake()
        {
            instance = this;
            Logger.LogInfo("TermialGamepad has launched successfully! ENJOY :)");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            TerminalStarted += OnTerminalBeginUsing;

            Rows = Config.Bind("Customization", "Rows", 2, "Adjust number of Rows");
            Columns = Config.Bind("Customization", "Columns", 8, "Adjust number of columns");
            PagesMode = Config.Bind("Customization", "Pages mode", PagesScrollType.Dynamic, "The way to switch between pages.\n\n" + "Options:\n" +
                                    "[Dynamic]: It checks the bigger count of rows and columns, then chooses automatically between LeftRight and UpDown.\n" +
                                    "[UpDown]: It will change between pages by going up or down.\n" +
                                    "[LeftRight]: It will change between pages by going left or right.\n" +
                                    "[Both]: It is a combination of UpDown and LeftRight.");
            CustomCommads = Config.Bind("Customization", "Custom Commands", "", "You can add other modds commands to shows up in the panel (e.g. \"door, light\")");

            BoxBackgroundColor = Config.Bind("Background Colors", "Box backcground color", "0, 0, 15, 130", "The color of the Box background");
            ButtonsColor = Config.Bind("Background Colors", "Buttons color", "0, 0, 20, 150", "The color of the buttons");
            HighlightedButtonColor = Config.Bind("Background Colors", "Highlighted Button color", "0, 0, 50, 150", "The color of the highlighted button");
            AmountBoxbackgroundColor = Config.Bind("Background Colors", "Amount box color", "0, 0, 50, 255", "The color of the amount box");

            BoxTextColor = Config.Bind("Text Colors", "Box text color", "30, 255, 0, 255", "The color of the box text");
            ButtonTextColor = Config.Bind("Text Colors", "Buttons text color", "20, 142, 0, 255", "The color of the buttons text");
            HighlightedButtonTextColor = Config.Bind("Text Colors", "Highlighted button text color", "36, 255, 0, 255", "The color of the highlighted button text");
            AmountButtonTextColor = Config.Bind("Text Colors", "Amount box text color", "24, 203, 0, 255", "The color of the amount box text");


            BoxTextColor.SettingChanged += (s,e) => TerminalGUI.UpdateTextColor();
            ButtonTextColor.SettingChanged += (s,e) => TerminalGUI.UpdateTextColor();
            HighlightedButtonTextColor.SettingChanged += (s,e) => TerminalGUI.UpdateTextColor();
            AmountButtonTextColor.SettingChanged += (s,e) => TerminalGUI.UpdateTextColor();
            CustomCommads.SettingChanged += (s,e) => TerminalGUI.UpdateMainButtonNames();
            PagesMode.SettingChanged += (s,e) => TerminalGUI.UpdatePagesMode();
        }

        private void OnTerminalBeginUsing(object sender, TerminalEventArgs e)
        {
            if (!GameObject.Find("TerminalGUI"))
            {
                var gameObject = new UnityEngine.GameObject("TerminalGUI");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
                gameObject.AddComponent<TerminalGUI>();
                myGUI = (TerminalGUI)gameObject.GetComponent("TerminalGUI");
                myGUI.keybinds = keybinds;
                Logger.LogMessage("GUI has been loaded succesfuly");
            }
        }
    }
}
