using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCModReverseGrief.Patches
{
    internal class PlayerControllerBPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayerFromOtherClientClientRpc")]
        public static bool PlayerDamaged(ref int damageAmount, ref Vector3 hitDirection, ref int playerWhoHit, PlayerControllerB __instance)
        {
            // If the other player is hitting is to save us from a clinged MOB, accept the damages
            CentipedeAI[] array = Object.FindObjectsByType<CentipedeAI>(0);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].clingingToPlayer == __instance)
                {
                    ModeBase.Instance.mls.LogInfo("REVERSE_GRIEF: Accepting the hit from player (" + playerWhoHit + ")");
                    return true;
                }
            }

            // At this point, we assume it's grief
            ModeBase.Instance.mls.LogInfo("REVERSE_GRIEF: Detected grief from another player (" + playerWhoHit + ")");

            // Find the player who caused the damage
            PlayerControllerB griefingPlayer = FindPlayerById(playerWhoHit);

            // Hit the other player back instantly
            if (griefingPlayer != null)
            {
                ModeBase.Instance.mls.LogInfo("REVERSE_GRIEF: Identified griefing player (" + playerWhoHit + "). Hitting back other player instantly.");
                griefingPlayer.DamagePlayerFromOtherClientServerRpc(damageAmount, -hitDirection, (int)__instance.playerClientId);
            } else {
                ModeBase.Instance.mls.LogInfo("REVERSE_GRIEF: Unable to identify griefing player. Skipping revenge.");
            }

            // For our own client, set the damage to 0
            damageAmount = 0;

            // Still run the original function to maximize compatibility with other mods or future version upgrades
            return true;
        }

        private static PlayerControllerB FindPlayerById(int playerId)
        {
            PlayerControllerB[] players = Object.FindObjectsOfType<PlayerControllerB>();
            foreach (var player in players)
            {
                if ((int) player.playerClientId == playerId)
                {
                    return player;
                }
            }
            return null;
        }
    }
}
