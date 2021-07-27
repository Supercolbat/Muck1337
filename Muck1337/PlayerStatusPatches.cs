using HarmonyLib;    

namespace Muck1337
{
    [HarmonyPatch(typeof(PlayerStatus))]
    class PlayerStatusPatches
    {
        /*
         * =================
         *  Invulnerability
         * =================
         * 
         * The HandleDamage method, quite obviously, handles the damage the player receives. We ignore
         * all the calculations for how much damage the player takes and only play the effects.
         * Methods such as Damage and DealDamage also calculate the amount of damage a player takes but
         * still rely on HandleDamage to actually make the player take damage.
         *
         * While you can set __instance.invulnerable as true, the DealDamage method does not check if the
         * invulnerable or not.
         * 
         * TODO: Later when settings are implemented, this can be replaced with a prefix that only returns false when the godmode/invulnerable setting is enabled 
         */
        
        [HarmonyPatch("HandleDamage")]
        [HarmonyPrefix]
        static bool HandleDamage_Prefix(PlayerStatus __instance, int damageTaken)
        {
            float shakeRatio = (float)damageTaken / (float)__instance.MaxHpAndShield();
            CameraShaker.Instance.DamageShake(shakeRatio);
            DamageVignette.Instance.VignetteHit();
        
            return false;
        }
    
        /*
         * ====================
         *  Rapid health regen
         * ====================
         *
         * In a hypothetical case, if you were to take any damage (despite being invulnerable), your health
         * will immediately go back to max when the heal method is called. Is this overkill? Yes.
         */
        [HarmonyPatch("Heal")]
        [HarmonyPrefix]
        static bool Heal_Prefix(PlayerStatus __instance)
        {
            __instance.hp = __instance.maxHp;
            return false;
        }
    
        /*
         * ====================
         *  Rapid hunger regen
         * ====================
         */
        [HarmonyPatch("Hunger")]
        [HarmonyPrefix]
        static bool Hunger_Prefix(PlayerStatus __instance)
        {
            __instance.hunger = __instance.maxHunger;
            return false;
        }
    
        /*
         * =====================
         *  Rapid stamina regen
         * =====================
         */
        [HarmonyPatch("Stamina")]
        [HarmonyPrefix]
        static bool Stamina_Prefix(PlayerStatus __instance)
        {
            __instance.stamina = __instance.maxStamina;
            return false;
        }
    
        /*
         * ==============================
         *  No stamina loss from jumping
         * ==============================
         */
        [HarmonyPatch("Jump")]
        [HarmonyPrefix]
        static bool Jump_Prefix(PlayerStatus __instance)
        {
            return false;
        }
    }
}