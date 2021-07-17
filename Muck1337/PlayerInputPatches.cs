using HarmonyLib;
using Muck1337.Utils;
using UnityEngine;

namespace Muck1337
{
    [HarmonyPatch(typeof(PlayerInput))]
    public class PlayerInputPatches
    {
        private static Vector3 flightDirection;
        private static float flightSpeed;
    
        /*
         * =============
         *  Keybindings
         * =============
         */
        [HarmonyPatch("MyInput")]
        [HarmonyPostfix]
        static void MyInput_Postfix(PlayerInput __instance)
        {
            /*
             * For those who want to quickly dupe lots of items
             */
            if (Input.GetKeyDown(KeyCode.Tilde))
            {
                foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
                {
                    if (inventoryCell.currentItem != null)
                    {
                        InventoryUI.Instance.DropItemIntoWorld(inventoryCell.currentItem);
                    }
                }
            }

            /*
             * Noclip + fly
             */
            if (Input.GetKey(KeyCode.F))
            {
                Transform camera = PrivateFinder.GetValue<Transform>(__instance, "playerCam").gameObject.transform;
                flightDirection = Vector3.zero;
                flightSpeed = 30f;
                
                // Prevent the player from slowly falling downwards
                PrivateFinder.GetValue<PlayerMovement>(__instance, "playerMovement").GetRb().velocity =
                    new Vector3(0f, 1f, 0f);

                if (Input.GetKey(KeyCode.W))
                    flightDirection += camera.forward;
                if (Input.GetKey(KeyCode.A))
                    flightDirection += -camera.right;
                if (Input.GetKey(KeyCode.S))
                    flightDirection += -camera.forward;
                if (Input.GetKey(KeyCode.D))
                    flightDirection += camera.right;
                if (Input.GetKey(KeyCode.LeftShift))
                    flightSpeed = 60f;

                __instance.gameObject.transform.position += flightDirection * Time.deltaTime * flightSpeed;
            }
        }
    }
}