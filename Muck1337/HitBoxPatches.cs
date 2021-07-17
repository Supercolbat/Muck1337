using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using HarmonyLib;
using Muck1337.Utils;
using UnityEngine;

namespace Muck1337
{
    [HarmonyPatch(typeof(HitBox))]
    public class HitBoxPatches
    {
	    /*
	     * ==========================
	     *    One shot everything
	     *  minus dragon and players
	     * ==========================
	     * 
	     * There has to be a better way of doing this than completely overriding the whole method...
	     */
        [HarmonyPatch("UseHitbox")]
        [HarmonyPrefix]
        static bool UseHitbox_Prefix(HitBox __instance)
        {
	        PrivateFinder.GetValue<List<Hitable>>(__instance, "alreadyHit").Clear();
	        PrivateFinder.SetValue<List<Hitable>>(__instance, "alreadyHit", new List<Hitable>());
	        if (Hotbar.Instance.currentItem == null)
	        {
		        return false;
	        }
	        float maxDistance = 1.2f + PlayerStatus.Instance.currentChunkArmorMultiplier;
	        RaycastHit[] array = Physics.SphereCastAll(__instance.playerCam.position + __instance.playerCam.forward * 0.1f, 3f, __instance.playerCam.forward, maxDistance, __instance.whatIsHittable);
	        Array.Sort<RaycastHit>(array, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
	        if (array.Length < 1)
	        {
	        	return false;
	        }
	        InventoryItem currentItem = Hotbar.Instance.currentItem;
	        bool falling = !PlayerMovement.Instance.grounded && PlayerMovement.Instance.GetVelocity().y < 0f;
	        PowerupCalculations.DamageResult damageMultiplier = PowerupCalculations.Instance.GetDamageMultiplier(falling, -1f);
	        float damageMultiplier2 = damageMultiplier.damageMultiplier;
	        bool flag = damageMultiplier.crit;
	        float lifesteal = damageMultiplier.lifesteal;
	        float sharpness = currentItem.sharpness;
	        bool flag2 = false;
	        int num = 0;
	        float num2 = 1f;
	        float num3 = 1f;
	        if (flag)
	        {
	        	num3 = 2f;
	        }
	        Vector3 pos = Vector3.zero;
	        bool flag3 = array[0].transform.CompareTag("Build");
	        int hitWeaponType = 0;
	        if (currentItem.name == "Rock")
	        {
	        	hitWeaponType = 2;
	        }
	        foreach (RaycastHit raycastHit in array)
	        {
	        	Collider collider = raycastHit.collider;
	        	Hitable component = collider.transform.root.GetComponent<Hitable>();
	        	if (!(component == null) && (collider.gameObject.layer != LayerMask.NameToLayer("Player") || component.GetId() != LocalClient.instance.myId))
	        	{
	        		if (!flag3 && raycastHit.transform.CompareTag("Build"))
	        		{
	        			return false;
	        		}
	        		if (!PrivateFinder.GetValue<List<Hitable>>(__instance, "alreadyHit").Contains(component))
	        		{
	        			if (!component.canHitMoreThanOnce)
	        			{
		                    // this may or may not work
		                    // PrivateFinder.GetValue<List<Hitable>>(__instance, "alreadyHit").Add(component);
		                    List<Hitable> alreadyHit = PrivateFinder.GetValue<List<Hitable>>(__instance, "alreadyHit");
		                    alreadyHit.Add(component);
		                    PrivateFinder.SetValue<List<Hitable>>(__instance, "alreadyHit", alreadyHit);
	        			}
	        			int num4 = 0;
	        			if (collider.gameObject.layer == LayerMask.NameToLayer("Object"))
	        			{
	        				HitableResource hitableResource = (HitableResource)component;
	        				if ((currentItem.type == hitableResource.compatibleItem && currentItem.tier >= hitableResource.minTier) || hitableResource.compatibleItem == InventoryItem.ItemType.Item)
	        				{
	        					float resourceMultiplier = PowerupInventory.Instance.GetResourceMultiplier(null);
	        					num4 = (int)((float)currentItem.resourceDamage * damageMultiplier2 * resourceMultiplier * num2);
	        					CameraShaker.Instance.DamageShake(0.1f * num3);
	        				}
	        			}
	        			else
	        			{
	        				CameraShaker.Instance.DamageShake(0.4f);
	        				int num5 = currentItem.attackDamage;
	        				if (currentItem.tag == InventoryItem.ItemTag.Arrow)
	        				{
	        					num5 = 1;
	        				}
	        				num4 = (int)((float)num5 * damageMultiplier2 * num2);
	        				Mob component2 = component.GetComponent<Mob>();
	        				if (component2 && currentItem.attackTypes != null && component2.mobType.weaknesses != null)
	        				{
	        					foreach (MobType.Weakness weakness in component2.mobType.weaknesses)
	        					{
	        						MobType.Weakness[] attackTypes = currentItem.attackTypes;
	        						for (int k = 0; k < attackTypes.Length; k++)
	        						{
	        							if (attackTypes[k] == weakness)
	        							{
	        								flag = true;
	        								num4 *= 2;
	        							}
	        						}
	        					}
	        				}
	        			}
	        			HitEffect hitEffect = HitEffect.Normal;
	        			if (damageMultiplier.sniped)
	        			{
	        				hitEffect = HitEffect.Big;
	        			}
	        			else if (flag)
	        			{
	        				hitEffect = HitEffect.Crit;
	        			}
	        			else if (damageMultiplier.falling)
	        			{
	        				hitEffect = HitEffect.Falling;
	        			}
	                    // all of this just for this ONE small change
	        			component.Hit(int.MaxValue, sharpness, (int)hitEffect, raycastHit.collider.ClosestPoint(PlayerMovement.Instance.playerCam.position), hitWeaponType);
	        			num2 *= 0.5f;
	        			PlayerStatus.Instance.Heal(Mathf.CeilToInt((float)num4 * lifesteal));
	        			if (flag)
	        			{
	        				PowerupInventory.Instance.StartJuice();
	        			}
	        			if (!flag2)
	        			{
	        				pos = raycastHit.collider.ClosestPoint(PlayerMovement.Instance.playerCam.position);
	        				num = num4;
	        			}
	        			flag2 = true;
	        		}
	        	}
	        }
	        if (flag2)
	        {
	        	if (damageMultiplier.sniped)
	        	{
	        		PowerupCalculations.Instance.HitEffect(PowerupCalculations.Instance.sniperSfx);
	        	}
	        	if (damageMultiplier2 > 0f && damageMultiplier.hammerMultiplier > 0f)
	        	{
	        		int num6 = 0;
	        		PowerupCalculations.Instance.SpawnOnHitEffect(num6, true, pos, (int)((float)num * damageMultiplier.hammerMultiplier));
	        		ClientSend.SpawnEffect(num6, pos);
	        	}
	        }
	        
	        return false;
        }
    }
}