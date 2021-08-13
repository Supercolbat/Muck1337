/*
using System.Collections.Generic;
using HarmonyLib;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(HitBox))]
    class HitBoxPatches
    {
        /*
         * ==========================
         *    One shot everything
         *  minus dragon and players
         * ==========================
         * 
         * There has to be a better way of doing this than completely overriding the whole method...
         * EDIT: Transpilers!! 
         * /
        [HarmonyPatch("UseHitbox")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // do something
        }
        static bool UseHitbox_Prefix(HitBox __instance)
        {
            return false;
        }
    }
}
*/