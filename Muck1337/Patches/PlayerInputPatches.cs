using HarmonyLib;
using Muck1337.Utils;
using UnityEngine;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(PlayerInput))]
    class PlayerInputPatches
    {
        private static Vector3 _flightDirection;
        private static float _flightSpeed;

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
            if (Input.GetKeyDown(KeyCode.F))
                PlayerMovement.Instance.GetPlayerCollider().enabled = false;
            if (Input.GetKeyUp(KeyCode.F))
                PlayerMovement.Instance.GetPlayerCollider().enabled = true;
            if (Input.GetKey(KeyCode.F))
            {
                Transform camera = PrivateFinder.GetValue<Transform>(__instance, "playerCam").gameObject.transform;
                _flightDirection = Vector3.zero;
                _flightSpeed = 30f;
                
                // Prevent the player from slowly falling downwards
                PrivateFinder.GetValue<PlayerMovement>(__instance, "playerMovement").GetRb().velocity =
                    new Vector3(0f, 1f, 0f);

                if (Input.GetKey(KeyCode.W))         _flightDirection +=  camera.forward;
                if (Input.GetKey(KeyCode.A))         _flightDirection += -camera.right;
                if (Input.GetKey(KeyCode.S))         _flightDirection += -camera.forward;
                if (Input.GetKey(KeyCode.D))         _flightDirection +=  camera.right;
                if (Input.GetKey(KeyCode.LeftShift)) _flightSpeed *= 2;

                __instance.gameObject.transform.position += _flightDirection * Time.deltaTime * _flightSpeed;
            }
            
            /*
             * Toggle GUI
             */
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                // Muck1337GUI.Toggled = !Muck1337GUI.Toggled;
            }
        }
    }
}