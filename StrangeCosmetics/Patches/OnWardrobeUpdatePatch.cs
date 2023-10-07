using GorillaNetworking;
using HarmonyLib;

namespace StrangeCosmetics.Patches
{
    [HarmonyPatch(typeof(CosmeticsController), "UpdateWardrobeModelsAndButtons")]
    internal class OnWardrobeUpdatePatch
    {
        public static void Postfix()
        {
            Plugin.RefreshStrangeCounts();
        }
    }
}
