using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace AccessibleTerminal.Patches
{
    [HarmonyPatch]
    internal static class TerminalPatches
    {
        private static CursorLockMode prevLock; // Previous mouse lock mode.
        private static bool wasVisible = false; // Whether mouse was previously visible or not.

        [HarmonyPatch(typeof(Terminal), "Awake")]
        [HarmonyPostfix]
        private static void OnInitTerminal()
        {
            // Create script holder game object.
            GameObject scriptHolder = new("TerminalGUI");
            Object.DontDestroyOnLoad(scriptHolder);
            scriptHolder.hideFlags = HideFlags.HideAndDontSave;
            // ...

            // Add 'TerminalGUI' script to script holder.
            TerminalGUI terminalGui = scriptHolder.AddComponent<TerminalGUI>();
            terminalGui.enabled = false;
            // ...
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.BeginUsingTerminal))]
        [HarmonyPostfix]
        private static void OnOpenTerminal()
        {
            // Toggle clickable terminal HUD visibility.
            TerminalGUI.Instance.ToMainMenu();
            TerminalGUI.Instance.ToggleVisibility();

            // Unlock mouse cursor after opening terminal.
            prevLock = Cursor.lockState;
            wasVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // ...

            // Disable camera movement when in the terminal.
            if (StartOfRound.Instance.localPlayerController != null)
            {
                StartOfRound.Instance.localPlayerController.disableLookInput = true;
            }
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.QuitTerminal))]
        [HarmonyPrefix]
        private static void OnCloseTerminal()
        {
            // Toggle clickable terminal HUD visibility.
            TerminalGUI.Instance.ToggleVisibility();

            // Set mouse cursor to previous state.
            Cursor.lockState = prevLock;
            Cursor.visible = wasVisible;
            // ...

            // Enable camera movement after closing terminal.
            if (StartOfRound.Instance.localPlayerController != null)
            {
                StartOfRound.Instance.localPlayerController.disableLookInput = false;
            }
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.TextChanged), [typeof(string)])]
        [HarmonyPrefix]
        private static void OnTextChanged(string newText, bool ___modifyingText)
        {
            TerminalGUI.Instance.terminalReady = !___modifyingText;
        }

        [HarmonyPatch(typeof(KillLocalPlayer), nameof(KillLocalPlayer.KillPlayer), [typeof(PlayerControllerB)])]
        [HarmonyPostfix]
        private static void OnPlayerDeath(PlayerControllerB playerWhoTriggered)
        {
            // Check if client was the player who died.
            if (playerWhoTriggered == null || !playerWhoTriggered.IsLocalPlayer)
            {
                return;
            }

            // Set mouse cursor to previous state.
            Cursor.lockState = prevLock;
            Cursor.visible = wasVisible;
            // ...

            // Enable camera movement upon dying.
            playerWhoTriggered.disableLookInput = false;
        }

        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
        [HarmonyPostfix]
        private static void OnDisconnect()
        {
            // Destroy script holder game object when exiting to title screen.
            if (TerminalGUI.Instance != null)
            {
                Object.Destroy(TerminalGUI.Instance.gameObject);
                TerminalGUI.Instance = null;
            }
        }
    }
}