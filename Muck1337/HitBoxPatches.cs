using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using HarmonyLib;
using Muck1337.Utils;
using UnityEngine;

namespace Muck1337
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
         *
        [HarmonyPatch("UseHitbox")]
        [HarmonyPrefix]
        static bool UseHitbox_Prefix(HitBox __instance)
        {
            return false;
        }
        */
    }
}