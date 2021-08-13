using HarmonyLib;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(PingController))]
    class PingControllerPatches
    {
        /*
         * Disables pinging if the player is holding item
         * in inventory (duping).
         */
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static bool Update_Prefix()
        {
            // In Harmony, if you return true, the original method will execute.
            // If false is returned, then the original method will not execute.
            //
            // Here, we are checking if the inventory is open and no item is held.
            // If both of these conditions are true, then we'll inverse it and return it, thus skipping the original method.
            // Otherwise, true will be returned and the original method will run.
            return !(InventoryUI.Instance.backDrop.activeInHierarchy && InventoryUI.Instance.currentMouseItem == null);
        }
    }
}