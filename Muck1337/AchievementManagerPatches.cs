using HarmonyLib;

namespace Muck1337
{
    [HarmonyPatch(typeof(AchievementManager))]
    public class AchievementsManagerPatches
    {
        /*
         * ======================
         *  Disable achievements
         * ======================
         *
         * Although I do enjoy cheating, I don't like doing it if it means that I can no longer
         * have the thrill of grinding for those achievements.
         *
         * The CheckAllBossesKilled method doesn't check if the player CanUseAchievements or not.
         * 
         * TODO: make this read a config (or gui soon™) because i know some people want to cheat the achievements
         */
        [HarmonyPatch("CanUseAchievements")]
        [HarmonyPostfix]
        static bool CanUseAchievements_Postfix(ref bool __return)
        {
            __return = false;
            return false;
        }

        [HarmonyPatch("CheckAllBossesKilled")]
        [HarmonyPrefix]
        static bool CheckAllBossesKilled_Prefix()
        {
            return false;
        }
    }
}