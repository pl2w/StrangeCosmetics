using HarmonyLib;

namespace StrangeCosmetics.Patches
{
    [HarmonyPatch(typeof(GorillaTagger), "Awake")]
    internal class OnGameInit
    {
        public static void Postfix()
        {
            Plugin.LoadMod();
        }
    }
}
