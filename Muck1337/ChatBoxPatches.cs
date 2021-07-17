using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Muck1337.Utils;

namespace Muck1337
{
    [HarmonyPatch(typeof(ChatBox))]
    class ChatBox_Patches
    {
	    /*
	     * ===============
	     *  Chat Commands
	     * ===============
         */
        [HarmonyPatch("ChatCommand")]
        [HarmonyPrefix]
		static bool ChatCommand_Prefix(ChatBox __instance, string message)
		{		     
			string colorConsole = "#" + ColorUtility.ToHtmlStringRGB(PrivateFinder.GetValue<Color>(__instance, "console"));
			string text = message.Substring(1);
			List<string> cmd = new List<string>(text.Split(' '));
			
			PrivateFinder.CallMethod(__instance, "ClearMessage");
			
			switch (cmd[0])
			{
				/*******************
				 * Modded commands *
				 *******************/
				
				/*
				 * usage: /dupe
				 * duplicate the player's whole inventory
				 */
				case "dupe":
					// loops over every inventory cell in player and drops it without updating the cell
					foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
					{
						if (!(inventoryCell.currentItem == null))
						{
							InventoryUI.Instance.DropItemIntoWorld(inventoryCell.currentItem);
						}
					}
					return false;
				
				/*
				 * usage: /tp <username>
				 * teleport to a player
				 */
				case "tp":
					string targetUsernameForTP = string.Join(" ", cmd.GetRange( 1, cmd.Count - 1)).ToLower();
					
					// loop through all the players connected to the server until one matches targetUsernameForTP
					foreach (Client c in Server.clients.Values)
					{
						if (c.player.username == targetUsernameForTP)
						{
							__instance.AppendMessage(-1, "<color=" + colorConsole + ">Teleporting to" + c.player.username + "<color=white>", "Muck1337");
							// set the player position as the matched player's position
							PlayerInput.Instance.transform.position = c.player.pos;
							return false;
						}
					}
					__instance.AppendMessage(-1, "<color=red>Could not find a player by the username of '" + targetUsernameForTP + "'<color=white>", "Muck1337");
					return false;
				
				/*
				 * usage: /pickup
				 * picks up all interactable items
				 */
				case "pickup":
					foreach (PickupInteract pickupInteract in Object.FindObjectsOfType<PickupInteract>())
					{
						pickupInteract.Interact();
					}

					// wont teleport items to us :((
					foreach (Item item in Object.FindObjectsOfType<Item>())
					{
						item.item.prefab.transform.position = PlayerInput.Instance.transform.position;
					}
					return false;
				
				/*
				 * usage: /openchests
				 * opens all chests
				 */
				case "openchests":
					foreach (ChestInteract chestInteract in Object.FindObjectsOfType<ChestInteract>())
					{
						chestInteract.Interact();
					}
					return false;
				
				/*
				 * usage: /item <item name> (amount)
				 * spawns item in player's inventory or drops it if it's full
				 */
				case "item":
					int itemAmount = 1;
					
					// if the last element in cmd is an integer, then the count for the GetRange will be 'cmd.Count - 2'
					// if not, the count will be 'cmd.Count - 1'
					string itemQuery = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out itemAmount) ? 2 : 1))).ToLower();

					// cycle through all the defined items until one matches our query
					foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
					{
						if (inventoryItem.name.ToLower() == itemQuery)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								colorConsole,
								">Spawning item: ",
								inventoryItem.name,
								"<color=white>"
							}), "Muck1337");
							
							// if the player's inventory is full, drop the item into the world and return false
							if (InventoryUI.Instance.IsInventoryFull())
							{
								InventoryUI.Instance.DropItemIntoWorld(inventoryItem);
								return false;
							}
							
							// sets the first empty inventory cell to the chosen item
							foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
							{
								if (inventoryCell.currentItem == null)
								{
									inventoryCell.currentItem = inventoryItem;
									if (inventoryCell.currentItem.max > itemAmount)
									    inventoryCell.currentItem.max = itemAmount;
									inventoryCell.currentItem.amount = itemAmount;
									inventoryCell.UpdateCell();
									return false;
								}
							}
						}
					}
					return false;
				
				/*
				 * usage: /itemuser <username> <item name> (amount)
				 * Spawns item at a player's location
				 */
				case "itemuser":
					int itemuserAmount = 1;
					string itemuserQuery = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out itemuserAmount) ? 2 : 1))).ToLower();
					Player targetPlayerForItem = null;

					foreach (Client c in Server.clients.Values)
					{
						if (c.player.username == cmd[1])
						{
							targetPlayerForItem = c.player;
							break;
						}
					}

					if (targetPlayerForItem == null)
					{
						__instance.AppendMessage(-1, string.Concat(new string[]
						{
							"<color=red>Could not find player by the username of '",
							cmd[1],
							"'<color=white>"
						}), "Muck1337");
						return false;
					}

					foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
					{
						if (inventoryItem.name.ToLower() == itemuserQuery)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								colorConsole,
								">Spawning item '",
								inventoryItem.name,
								"' at user '",
								targetPlayerForItem.username,
								"'<color=white>"
							}), "Muck1337");
							
							ItemManager.Instance.DropItemAtPosition(inventoryItem.id, itemuserAmount, targetPlayerForItem.pos, ItemManager.Instance.GetNextId());
							return false;
						}
					}
					return false;
				
				/*
				 * usage: /powerup <powerup name> (amount)
				 * increments the stated powerup for the player by the given amount
				 */
				case "powerup":
					int powerupAmount = 1;
					string powerupArg = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out powerupAmount) ? 2 : 1))).ToLower();

					foreach (KeyValuePair<string, int> powerupPair in ItemManager.Instance.stringToPowerupId)
					{
						if (powerupPair.Key.ToLower() == powerupArg)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								colorConsole,
								">Spawning powerup: ",
								powerupPair.Key,
								"<color=white>"
							}), "Muck1337");

							int[] powerups = PrivateFinder.GetValue<int[]>(PowerupInventory.Instance, "powerups");
							
							for (int i = 0; i < powerupAmount; i++)
							{
								powerups[powerupPair.Value]++;
								PrivateFinder.SetValue<int[]>(PowerupInventory.Instance, "powerups", powerups);
								UiEvents.Instance.AddPowerup(ItemManager.Instance.allPowerups[powerupPair.Value]);
								PlayerStatus.Instance.UpdateStats();
								PowerupUI.Instance.AddPowerup(powerupPair.Value);
							}
						}
					}
					return false;
				
				/*
				 * usage: /powerupuser <username> <item name> (amount)
				 * Spawns powerup at a player's location
				 */
				case "powerupuser":
					int powerupuserAmount = 1;
					string powerupuserArg = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out powerupuserAmount) ? 2 : 1))).ToLower();

					Player targetPlayerForPowerup = null;

					foreach (Client c in Server.clients.Values)
					{
						if (c.player.username == cmd[1])
						{
							targetPlayerForPowerup = c.player;
							break;
						}
					}

					if (targetPlayerForPowerup == null)
					{
						__instance.AppendMessage(-1, string.Concat(new string[]
						{
							"<color=red>Could not find player by the username of '",
							cmd[1],
							"'<color=white>"
						}), "Muck1337");
						return false;
					}
					
					foreach (KeyValuePair<string, int> powerupPair in ItemManager.Instance.stringToPowerupId)
					{
						if (powerupPair.Key.ToLower() == powerupuserArg)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								colorConsole,
								">Spawning powerup: ",
								powerupPair.Key,
								"<color=white>"
							}), "Muck1337");

							for (int i = 0; i < powerupuserAmount; i++)
							{
								ItemManager.Instance.DropPowerupAtPosition(powerupPair.Value, targetPlayerForPowerup.pos, ItemManager.Instance.GetNextId());
							}
						}
					}
					return false;
				
				/*
				 * usage: /sail
				 * starts the boat rising & final boss event
				 */
				case "sail":
					Boat.Instance.LeaveIsland();
					return false;
				
				/*
				 * usage: /hell
				 * activates all the boss, guardian, and combat shrines
				 */
				case "hell":
					ShrineBoss[] bossShrines = Object.FindObjectsOfType<ShrineBoss>();
					ShrineGuardian[] guardianShrines = Object.FindObjectsOfType<ShrineGuardian>();
					ShrineInteractable[] combatShrines = Object.FindObjectsOfType<ShrineInteractable>();

					if (bossShrines.Length == 0 && guardianShrines.Length == 0 && combatShrines.Length == 0)
					{
						__instance.AppendMessage(-1, "<color=red>Hell has already been unleashed!<color=white>", "Muck1337");
						return false;
					}
					foreach (var s in bossShrines)
					{
						s.Interact();
					}

					foreach (var s in guardianShrines)
					{
						s.Interact();
					}

					foreach (var s in combatShrines)
					{
						s.Interact();
					}

					__instance.AppendMessage(-1, "<color=red>HELL HAS BEEN UNLEASHED!<color=white>", "Muck1337");
					return false;
				
				/*
				 * usage: /killmobs
				 * kills all mobs
				 */
				case "killmobs":
					foreach (HitableMob hitableMob in Object.FindObjectsOfType<HitableMob>())
					{
						hitableMob.Hit(int.MaxValue, int.MaxValue, (int)HitEffect.Normal, hitableMob.mob.transform.position);
					}
					return false;
				
				
				/*********************************
				 * Development commands          *
				 * at the bottom for easy access *
				 *********************************/
				
				/*
				 * usage: /id
				 * returns the id of the held item
				 */
				case "id":
					if (Hotbar.Instance.currentItem != null)
					{
						__instance.AppendMessage(-1, string.Concat(new object[]
						{
							"<color=",
							colorConsole,
							">ID: ",
							Hotbar.Instance.currentItem.id,
							"<color=white>"
						}), "Muck1337");
						return false;
					}
					__instance.AppendMessage(-1, string.Concat(new object[]
					{
						"<color=",
						colorConsole,
						">ID: null<color=white>"
					}), "Muck1337");
					return false;
				
				case "test":
					return false;
			}

			return true;
		}
	}
}