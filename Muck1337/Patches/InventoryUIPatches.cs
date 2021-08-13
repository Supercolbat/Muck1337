using HarmonyLib;
using UnityEngine.EventSystems;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(InventoryUI))]
    class InventoryUIPatches
    {
        /*
         * =================
         *  Unlimited coins
         * =================
         *
         * 2021 free no virus no human verification no scam no survey
         */
        [HarmonyPatch("GetMoney")]
        [HarmonyPrefix]
        static bool GetMoney_Prefix(ref int __result)
        {
            __result = int.MaxValue;
            return false;
        }
        
        /*
         * =====================
         *  Middle click duping
         * =====================
         */
        [HarmonyPatch("DropItem")]
        [HarmonyPrefix]
        static bool DropItem_Prefix(InventoryUI __instance, PointerEventData eventData)
        {
            if (__instance.currentMouseItem == null)
                return false;

            if (eventData != null && eventData.button == PointerEventData.InputButton.Middle)
            {
                __instance.DropItemIntoWorld(__instance.currentMouseItem);
                return false;
            }

            return true;
        }
    }
}