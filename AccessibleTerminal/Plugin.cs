using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

namespace AccessibleTerminal
{
    // [BepInDependency(DependencyGUID: "LethalAPI.LibTerminal", MinimumDependencyVersion: "1.0.1")]
    [BepInPlugin(GUID, PLUGIN_NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string GUID = "pacoito.AccessibleTerminal", PLUGIN_NAME = "AccessibleTerminal", VERSION = "1.0.0";
        internal static ManualLogSource StaticLogger;

        private void Awake()
        {
            StaticLogger = Logger;

            try
            {
                Harmony harmony = new(PLUGIN_NAME);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                StaticLogger.LogInfo($"{PLUGIN_NAME} loaded!");
            }
            catch (Exception e)
            {
                StaticLogger.LogError($"Error while initializing {PLUGIN_NAME}: {e}");
            }
        }
    }
}
