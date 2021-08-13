using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Muck1337.Utils;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(ChatBox))]
    class ChatBoxPatches
    {
	    private static string _colorConsole;
	    
	    /*
	     * =================================
	     *  Add commands to prediction list
	     * =================================
	     */
	    [HarmonyPatch("Awake")]
	    [HarmonyPostfix]
	    static void Awake_Postfix(ChatBox __instance)
	    {
		    _colorConsole =
			    "#" + ColorUtility.ToHtmlStringRGB(PrivateFinder.GetValue<Color>(ChatBox.Instance, "console"));
		    
		    __instance.commands = __instance.commands.Concat(CommandManager.Commands.Keys).ToArray();
		    Muck1337Plugin.Instance.log.LogInfo("Initialized commands");
	    }
	    
	    /*
	     * ===============
	     *  Chat Commands
	     * ===============
         */
        [HarmonyPatch("ChatCommand")]
        [HarmonyPostfix]
		static void ChatCommand_Postfix(ChatBox __instance, string message)
		{
			// original code
			if (message.Length <= 0)
				return;
			
			var cmd = new List<string>(message.Substring(1).Split(' '));
			
			PrivateFinder.CallMethod(__instance, "ClearMessage");

			// Get the true command name (used in CommandManager.commands) for the entered command
			bool commandExists = CommandManager.CommandNames.TryGetValue(cmd[0], out string trueCommandName);
			
			// If the entered command doesn't match a Muck1337 command, run the original ChatCommand method
			if (!commandExists)
				return;
			
			// Call the command and skip the original method
			CommandManager.Commands[trueCommandName].Callback(cmd.GetRange(1, cmd.Count - 1).ToArray());
		}

		public static void InitCommands()
		{
			/*
		     * usage: /dupe
		     * duplicate the player's whole inventory
		     */
			CommandManager.AddCommand("dupe", delegate(string[] args)
			{
				// loops over every inventory cell in player and drops it without updating the cell
				foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
				{
					if (inventoryCell.currentItem != null)
					{
						InventoryUI.Instance.DropItemIntoWorld(inventoryCell.currentItem);
					}
				}
			});
			
			/*
			 * usage: /tp <username>
			 * teleport to a player
			 */
			CommandManager.AddCommand("tp", delegate(string[] args)
			{
				string targetUsername = String.Join(" ", args).ToLower();
					
				// loop through all the players connected to the server until one matches targetUsername
				foreach (Client client in Server.clients.Values)
				{
					if (client?.player == null)
						continue;
						
					if (client.player.username.ToLower() == targetUsername)
					{
						ChatBox.Instance.AppendMessage(-1, "<color=" + _colorConsole + ">Teleporting to " + client.player.username + "<color=white>", "Muck1337");
						
						// set the player position as the matched player's position
						PlayerInput.Instance.transform.position = client.player.pos;
						return;
					}
				}
				ChatBox.Instance.AppendMessage(-1, "<color=red>Could not find a player by the username of '" + targetUsername + "'<color=white>", "Muck1337");
			}, prediction: delegate(string text)
			{
				foreach (Client client in Server.clients.Values)
				{
					if (client?.player != null && client.player.username.ToLower()
						.StartsWith(text))
					{
						return client.player.username;
					}
				}

				return "";
			});
			
			/*
		     * usage: /pickup ([items, powerups])
		     * picks up all interactable items
		     */
			CommandManager.AddCommand("pickup", delegate(string[] args)
			{
				foreach (PickupInteract pickupInteract in UnityEngine.Object.FindObjectsOfType<PickupInteract>())
				{
					pickupInteract.Interact();
				}
					
				foreach (Item item in UnityEngine.Object.FindObjectsOfType<Item>())
				{
					switch (args[0])
					{
						case "items":
							if (item.powerup == null)
								item.transform.position = PlayerInput.Instance.transform.position;
							break;
						
						case "powerups":
							if (item.powerup != null)
								item.transform.position = PlayerInput.Instance.transform.position;
							break;
						
						default:
							item.transform.position = PlayerInput.Instance.transform.position;
							break;
							
					}
				}
			}, new []{"pick"}, delegate(string text)
			{
				foreach (string pickupOption in new [] {"items", "powerups"})
				{
					if (pickupOption.StartsWith(text))
					{
						return pickupOption;
					}
				}
				
				return "";
			});
			
			/*
			 * usage: /chests
			 * opens all chests
			 */
			CommandManager.AddCommand("chests", delegate(string[] args)
			{
				foreach (LootContainerInteract lootContainerInteract in UnityEngine.Object.FindObjectsOfType<LootContainerInteract>())
				{
					lootContainerInteract.Interact();
				}
			});
			
			/*
		     * usage: /item <item name> (amount)
		     * spawns item in player's inventory or drops it if it's full
		     */
			CommandManager.AddCommand("item", delegate(string[] args)
			{
				// Check if the last element in args is an int
				bool isInt = int.TryParse(args.Last(), out int itemAmount);
				if (!isInt) itemAmount = 1;
				
				// Join args (excluding last one if isInt is true) to form itemQuery
				string itemQuery = String.Join(" ", args.Take(args.Length - (isInt ? 1 : 0))).ToLower();

				// Cycle through all the defined items until one matches the query
				foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
				{
					if (inventoryItem.name.ToLower() == itemQuery)
					{
						ChatBox.Instance.AppendMessage(-1, string.Concat(
							"<color=",
							_colorConsole,
							">Spawning item: ",
							inventoryItem.name,
							"<color=white>"
						), "Muck1337");
						
						// Drop the item if the player's inventory is full
						if (InventoryUI.Instance.IsInventoryFull())
						{
							InventoryUI.Instance.DropItemIntoWorld(inventoryItem);
							return;
						}
							
						// Set the first empty inventory cell to the chosen item
						foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
						{
							if (inventoryCell.currentItem == null)
							{
								inventoryCell.currentItem = inventoryItem;
								if (inventoryCell.currentItem.max > itemAmount)
									inventoryCell.currentItem.max = itemAmount;
								inventoryCell.currentItem.amount = itemAmount;
								inventoryCell.UpdateCell();
								return;
							}
						}
							
						// Update the hotbar so the item shows if in hotbar
						InventoryUI.Instance.hotbar.UpdateHotbar();
					}
				}
			}, new []{"i"}, delegate(string text)
			{
				foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
				{
					if (inventoryItem.name.ToLower().StartsWith(text))
					{
						return inventoryItem.name;
					}
				}
				return "";
			});
			
			/*
		     * usage: /powerup <powerup name> (amount)
		     * increments the stated powerup for the player by the given amount
		     */
			CommandManager.AddCommand("powerup", delegate(string[] args)
			{
				// Check if the last element in args is an int
				bool isInt = int.TryParse(args.Last(), out int powerupAmount);
				if (!isInt) powerupAmount = 1;
				
				// Join args (excluding last one if isInt is true) to form itemQuery
				string powerupArg = String.Join(" ", args.Take(args.Length - (isInt ? 1 : 0))).ToLower();

				// Cycle through all the defined items until one matches the query
				foreach (var powerupPair in ItemManager.Instance.stringToPowerupId)
				{
					if (powerupPair.Key.ToLower() == powerupArg)
					{
						ChatBox.Instance.AppendMessage(-1, string.Concat(new string[]
						{
							"<color=",
							_colorConsole,
							">Spawning powerup: ",
							powerupPair.Key,
							"<color=white>"
						}), "Muck1337");
						
						// Add powerups using in-game methods
						int[] powerups = PrivateFinder.GetValue<int[]>(PowerupInventory.Instance, "powerups");
							
						for (int i = 0; i < powerupAmount; i++)
						{
							powerups[powerupPair.Value]++;
							PrivateFinder.SetValue(PowerupInventory.Instance, "powerups", powerups);
							PlayerStatus.Instance.UpdateStats();
							PowerupUI.Instance.AddPowerup(powerupPair.Value);
						}
					}
				}
			}, new []{"pow"}, delegate(string text)
			{
				foreach (var powerupPair in ItemManager.Instance.stringToPowerupId)
				{
					if (powerupPair.Key.ToLower().StartsWith(text))
					{
						return powerupPair.Key;
					}
				}
				return "";
			});
			
			/*
			 * usage: /sail
			 * starts the boat rising & final boss event
			 */
			CommandManager.AddCommand("sail", delegate(string[] args)
			{
				// Boat.Instance.LeaveIsland();
				PrivateFinder.CallMethod(PrivateFinder.GetValue<FinishGameInteract>(Boat.Instance, "wheelInteract"), "Interact");
			});
			
			/*
			 * usage: /hell
			 * activates all the boss, guardian, and combat shrines
			 */
			CommandManager.AddCommand("hell", delegate(string[] args)
			{
				var bossShrines = UnityEngine.Object.FindObjectsOfType<ShrineBoss>();
				var guardianShrines = UnityEngine.Object.FindObjectsOfType<ShrineGuardian>();
				var combatShrines = UnityEngine.Object.FindObjectsOfType<ShrineInteractable>();

				if (bossShrines.Length == 0 && guardianShrines.Length == 0 && combatShrines.Length == 0)
				{
					ChatBox.Instance.AppendMessage(-1, "<color=red>Hell has already been unleashed :(<color=white>", "Muck1337");
					return;
				}
				
				// Kind of ugly. Can be compressed
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

				ChatBox.Instance.AppendMessage(-1, "<color=red>HELL HAS BEEN UNLEASHED!<color=white>", "Muck1337");
			});

			/*
			 * usage: /killmobs
			 * kills all mobs
			 */
			CommandManager.AddCommand("killmobs", delegate(string[] args)
			{
				foreach (HitableMob hitableMob in UnityEngine.Object.FindObjectsOfType<HitableMob>())
				{
					hitableMob.Hit(int.MaxValue, int.MaxValue, (int)HitEffect.Normal, hitableMob.mob.transform.position);
				}
			}, new []{"mobs"});

			/*
			 * usage: /destroy
			 * destroys all materials
			 */
			CommandManager.AddCommand("destroy", delegate(string[] args)
			{
				object[] hitableResources = {UnityEngine.Object.FindObjectsOfType<HitableRock>(), UnityEngine.Object.FindObjectsOfType<HitableTree>()};
				foreach (HitableResource hitableResource in hitableResources)
				{
					hitableResource.Hit(int.MaxValue, int.MaxValue, (int)HitEffect.Normal, hitableResource.transform.position, 1);
				}
			}, new []{"des"});
			
			
			/*********************************
			 * Development commands          *
			 * at the bottom for easy access *
			 *********************************/
			
			/*
		     * usage: /id
		     * returns the id of the held item
		     */
			CommandManager.AddCommand("id", delegate(string[] args)
			{
				if (Hotbar.Instance.currentItem != null)
				{
					ChatBox.Instance.AppendMessage(-1, string.Concat(new object[]
					{
						"<color=",
						_colorConsole,
						">ID: ",
						Hotbar.Instance.currentItem.id,
						"<color=white>"
					}), "Muck1337");
					return;
				}
				
				ChatBox.Instance.AppendMessage(-1, string.Concat(new object[]
				{
					"<color=",
					_colorConsole,
					">ID: null<color=white>"
				}), "Muck1337");
			});

			/*
			 * usage: /test
			 * yes
			 */
			CommandManager.AddCommand("test", delegate(string[] args)
			{
				ChatBox.Instance.AppendMessage(-1, string.Concat(new object[]
				{
					"<color=",
					_colorConsole,
					">This is a test!<color=white>"
				}), "Muck1337");
				// yes
			});
		}
	}
}