using HarmonyLib;

namespace AccessibleTerminal.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal static class StartOfRoundPatches
    {
        [HarmonyPatch(nameof(StartOfRound.OnClientConnect), [typeof(ulong)])]
        [HarmonyPostfix]
        private static void OnPlayerJoin(ulong clientId)
        {
            // Refresh player button list whenever a player joins the lobby.
            TerminalGUI.Instance?.RefreshPlayers();
        }

        [HarmonyPatch(nameof(StartOfRound.OnPlayerDC), [typeof(int), typeof(ulong)])]
        [HarmonyPostfix]
        private static void OnPlayerLeave(int playerObjectNumber, ulong clientId)
        {
            // Refresh player button list whenever a player leaves the lobby.
            TerminalGUI.Instance?.RefreshPlayers();
        }
    }
}