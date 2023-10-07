using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrangeCosmetics.Patches
{
    [HarmonyPatch(typeof(GorillaTagManager), "ReportTag")]
    internal class OnTagPatch 
    {
        static Player prevPlayer;
        static float timeSinceLastTry;

        public static void Postfix(GorillaTagManager __instance, Player taggedPlayer, Player taggingPlayer)
        {
            if (taggingPlayer.IsLocal && __instance.LocalCanTag(PhotonNetwork.LocalPlayer, taggedPlayer) && !__instance.currentInfected.Contains(taggedPlayer))
            {
                if (prevPlayer == taggedPlayer && Time.time < timeSinceLastTry + 0.5f) return;
                timeSinceLastTry = Time.time;
                prevPlayer = taggedPlayer;

                Plugin.OnTagPlayer();
            }
        }
    }
}
