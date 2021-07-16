using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Muck1337.Utils;

namespace Muck1337
{
	[HarmonyPatch(typeof(ChatBox), "ChatCommand")]
	class ChatCommandPatch
	{
		static bool Prefix(ChatBox __instance, string message)
		{
			string color = "#" + ColorUtility.ToHtmlStringRGB(PrivateFind.GetValue<Color>(__instance, "console"));
			string text = message.Substring(1);
			List<string> cmd = new List<string>(text.Split(' '));
			
			PrivateFind.CallMethod(__instance, "ClearMessage");
			
			switch (cmd[0])
			{
				/*
				 * Modded commands
				 */
				case "dupe":
					foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
					{
						if (!(inventoryCell.currentItem == null))
						{
							InventoryUI.Instance.DropItemIntoWorld(inventoryCell.currentItem);
						}
					}
					break;
				
				case "tp":
					PlayerInput.Instance.transform.position =
						Object.FindObjectsOfType<OnlinePlayer>()[0].transform.position;
					break;
				
				case "pickup":
					PickupInteract[] array = Object.FindObjectsOfType<PickupInteract>();
					for (int j = 0; j < array.Length; j++)
					{
						array[j].Interact();
					}
					break;
				
				case "id":
					__instance.AppendMessage(-1, string.Concat(new object[]
					{
						"<color=",
						color,
						">ID: ",
						Hotbar.Instance.currentItem.id,
						"<color=white>"
					}), "");
					break;
				
				case "item":
					int itemAmount;
					string itemArg = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out itemAmount) ? 2 : 1))).ToLower();

					foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
					{
						if (inventoryItem.name.ToLower() == itemArg)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								color,
								">Spawning item: ",
								inventoryItem.name,
								"<color=white>"
							}), "");
							
							if (InventoryUI.Instance.IsInventoryFull())
							{
								InventoryUI.Instance.DropItemIntoWorld(inventoryItem);
								break;
							}

							foreach (InventoryCell inventoryCell in InventoryUI.Instance.allCells)
							{
								if (inventoryCell.currentItem == null)
								{
									inventoryCell.currentItem = inventoryItem;
									inventoryCell.UpdateCell();
									break;
								}
							}
						}
					}
					break;
				
				case "powerup":
					string powerupArg = string.Join(" ", cmd.GetRange( 1, cmd.Count - (int.TryParse(cmd[cmd.Count - 1], out var powerupAmount) ? 2 : 1))).ToLower();

					foreach (KeyValuePair<string, int> keyValuePair in ItemManager.Instance.stringToPowerupId)
					{
						if (keyValuePair.Key.ToLower() == powerupArg)
						{
							__instance.AppendMessage(-1, string.Concat(new string[]
							{
								"<color=",
								color,
								">Spawning item: ",
								keyValuePair.Key,
								"<color=white>"
							}), "");

							int[] powerups = PrivateFind.GetValue<int[]>(PowerupInventory.Instance, "powerups");
							
							for (int i = 0; i < powerupAmount; i++)
							{
								powerups[keyValuePair.Value]++;
								PrivateFind.SetValue<int[]>(PowerupInventory.Instance, "powerups", powerups);
								UiEvents.Instance.AddPowerup(ItemManager.Instance.allPowerups[keyValuePair.Value]);
								PlayerStatus.Instance.UpdateStats();
								PowerupUI.Instance.AddPowerup(keyValuePair.Value);
							}
						}
					}
					break;
				
				case "nick":
					Server.clients[LocalClient.instance.myId].player.username =
						string.Join(" ", cmd.GetRange(1, cmd.Count - 1));
					break;
				
				case "leaveisland":
					Boat.Instance.LeaveIsland();
					break;
				
				case "clip":
					if (cmd[1] == "off")
					{
						PlayerMovement.Instance.GetPlayerCollider().enabled = false;
						break;
					}

					if (cmd[1] == "on")
					{
						PlayerMovement.Instance.GetPlayerCollider().enabled = true;
					}
					break;
				
				case "hell":
					ShrineBoss[] bossShrines = Object.FindObjectsOfType<ShrineBoss>();
					ShrineGuardian[] guardianShrines = Object.FindObjectsOfType<ShrineGuardian>();
					ShrineInteractable[] combatShrines = Object.FindObjectsOfType<ShrineInteractable>();
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

					__instance.AppendMessage(-1, "<color=red>Welcome to hell...<color=white>", "");
					break;
				
				/*
				 * Default commands
				 */
				
				case "seed":
					int seed = GameManager.gameSettings.Seed;
					__instance.AppendMessage(-1, string.Concat(new object[]
					{
						"<color=",
						color,
						">Seed: ",
						seed,
						" (copied to clipboard)<color=white>"
					}), "");
					GUIUtility.systemCopyBuffer = string.Concat(seed);
					break;
				
				case "ping":
					__instance.AppendMessage(-1, "<color=" + color + ">pong<color=white>", "");
					break;
				
				case "debug":
					DebugNet.Instance.ToggleConsole();
					break;
				
				case "kill":
					PlayerStatus.Instance.Damage(0, 0, true);
					break;
				
				/*
				 * Testing command
				 * at the bottom for easy access
				 */
				case "test":
					foreach (Mob mob in MobManager.Instance.mobs.Values)
					{
						MobManager.Instance.RemoveMob(mob.id);
					}
					break;
			}

			return true;
		}
	}
}