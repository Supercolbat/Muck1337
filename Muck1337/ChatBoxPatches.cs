using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Muck1337.Utils;

namespace Muck1337
{
    [HarmonyPatch(typeof(ChatBox))]
    class ChatBox_Patches
    {
	    /*
	     * =================================
	     *  Add commands to prediction list
	     * =================================
	     */
	    [HarmonyPatch("Awake")]
	    [HarmonyPostfix]
	    static void Awake_Postfix(ChatBox __instance)
	    {
		    __instance.commands = __instance.commands.Concat(new string[]
		    {
			    "dupe",
			    "tp",
			    "pickup",
			    "chests",
			    "item",
			    "powerup",
			    "sail",
			    "hell",
			    "killmobs",
		    }).ToArray();
	    }
	    
	    /*
	     * ===============
	     *  Chat Commands
	     * ===============
         */
        [HarmonyPatch("ChatCommand")]
        [HarmonyPrefix]
		static bool ChatCommand_Prefix(ChatBox __instance, string message)
		{
			if (message.Length <= 0)
			{
				return false;
			}
			
			string colorConsole = "#" + ColorUtility.ToHtmlStringRGB(PrivateFinder.GetValue<Color>(__instance, "console"));;
			List<string> cmd = new List<string>(message.Substring(1).Split(' '));
			
			PrivateFinder.CallMethod(__instance, "ClearMessage");
			
			switch (cmd[0])
			{
				/*
				 * usage: /dupe
				 * duplicate the player's whole inventory
				 */
				case "dupe":
				case "d":
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
					string targetUsernameForTp = string.Join(" ", cmd.GetRange( 1, cmd.Count - 1)).ToLower();
					
					// loop through all the players connected to the server until one matches targetUsernameForTP
					foreach (Client client in Server.clients.Values)
					{
						if (client == null || client.player == null)
							continue;
						
						if (client.player.username.ToLower() == targetUsernameForTp)
						{
							__instance.AppendMessage(-1, "<color=" + colorConsole + ">Teleporting to " + client.player.username + "<color=white>", "Muck1337");
							// set the player position as the matched player's position
							PlayerInput.Instance.transform.position = client.player.pos;
							return false;
						}
					}
					__instance.AppendMessage(-1, "<color=red>Could not find a player by the username of '" + targetUsernameForTp + "'<color=white>", "Muck1337");
					return false;
				
				/*
				 * usage: /pickup ([items, powerups])
				 * picks up all interactable items
				 */
				case "pickup":
				case "pick":
					foreach (PickupInteract pickupInteract in Object.FindObjectsOfType<PickupInteract>())
					{
						pickupInteract.Interact();
					}
					
					foreach (Item item in Object.FindObjectsOfType<Item>())
					{
						if (cmd[1] == "items" && item.powerup == null)
							item.transform.position = PlayerInput.Instance.transform.position;
						else if (cmd[1] == "powerups" && item.powerup != null)
						    item.transform.position = PlayerInput.Instance.transform.position; 
						else
							item.transform.position = PlayerInput.Instance.transform.position;
					}
					return false;
				
				/*
				 * usage: /chests
				 * opens all chests
				 */
				case "chests":
					foreach (LootContainerInteract lootContainerInteract in Object.FindObjectsOfType<LootContainerInteract>())
					{
						lootContainerInteract.Interact();
					}
					return false;
				
				/*
				 * usage: /item <item name> (amount)
				 * spawns item in player's inventory or drops it if it's full
				 */
				case "item":
				case "i":
					// if the last element in cmd is an integer, then the count for the GetRange will be 'cmd.Count - 2'
					// if not, the count will be 'cmd.Count - 1'
					bool itemAmountIsInt = int.TryParse(cmd[cmd.Count - 1], out int itemAmount);
					if (!itemAmountIsInt) itemAmount = 1;
					
					string itemQuery = string.Join(" ", cmd.GetRange( 1, cmd.Count - (itemAmountIsInt ? 2 : 1))).ToLower();

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
				 * usage: /powerup <powerup name> (amount)
				 * increments the stated powerup for the player by the given amount
				 */
				case "powerup":
				case "pow":
					bool powerupAmountIsInt = int.TryParse(cmd[cmd.Count - 1], out int powerupAmount);
					if (!powerupAmountIsInt) itemAmount = 1;
					
					string powerupArg = string.Join(" ", cmd.GetRange( 1, cmd.Count - (powerupAmountIsInt ? 2 : 1))).ToLower();

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
								PlayerStatus.Instance.UpdateStats();
								PowerupUI.Instance.AddPowerup(powerupPair.Value);
							}
						}
					}
					return false;
				
				/*
				 * usage: /sail
				 * starts the boat rising & final boss event
				 */
				case "sail":
				case "sl":
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
				case "mobs":
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
				
				/*
				 * usage: /test
				 * yes
				 */
				
				case "test":
					return false;
			}

			return true;
		}
	}
}