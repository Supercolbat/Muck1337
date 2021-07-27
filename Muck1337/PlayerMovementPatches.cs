using HarmonyLib;
using Muck1337.Utils;
using UnityEngine;

namespace Muck1337
{
    [HarmonyPatch(typeof(PlayerMovement))]
    class PlayerMovementPatches
    {
        /*
         * ================
         *  Infinite jumps
         * ================
         * 
         * The original method has too many checks to see if you can jump.
         * I pressed the space bar so let me jump!
         */
        [HarmonyPatch("Jump")]
        [HarmonyPrefix]
        static bool Jump_Prefix(PlayerMovement __instance, ref Rigidbody ___rb, ref bool ___readyToJump, ref int ___resetJumpCounter, ref float ___jumpForce, ref Vector3 ___normalVector)
        {
            ___rb.isKinematic = false;
            ___readyToJump = false;
            __instance.CancelInvoke("JumpCooldown");
            __instance.Invoke("JumpCooldown", 0.25f);
            ___resetJumpCounter = 0;
            float d = ___jumpForce * PowerupInventory.Instance.GetJumpMultiplier();
            ___rb.AddForce(Vector3.up * d * 1.5f, ForceMode.Impulse);
            ___rb.AddForce(___normalVector * d * 0.5f, ForceMode.Impulse);
            Vector3 velocity = ___rb.velocity;
            if (___rb.velocity.y < 0.5f)
            {
                ___rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            }
            else if (___rb.velocity.y > 0f)
            {
                ___rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            }
            // NOTE: the '__instance.transform.position' in the line before was originally 'base.transform.position'
            //       may or may not work
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = Object.Instantiate<GameObject>(__instance.playerJumpSmokeFx, PlayerInput.Instance.transform.position, Quaternion.LookRotation(Vector3.up)).GetComponent<ParticleSystem>().velocityOverLifetime;
            velocityOverLifetime.x = ___rb.velocity.x * 2f;
            velocityOverLifetime.z = ___rb.velocity.z * 2f;
            PrivateFinder.CallMethod(PrivateFinder.GetValue<PlayerStatus>(__instance, "playerStatus"), "Jump");
            
            return false;
        }
    }
}