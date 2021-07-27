using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Muck1337.Utils;

namespace Muck1337
{
    [HarmonyPatch(typeof(Commands))]
    class CommandsPatches
    {
        private static void SetSuggestedText(object instance, string command, string text)
        {
            PrivateFinder.SetValue<string>(instance, "suggestedText", command + " " + text);
        }
        
        [HarmonyPatch("PredictCommands")]
        [HarmonyPostfix]
        static void PredictCommands_Postfix(Commands __instance)
        {
            string text = __instance.inputField.text;
            string[] cmds = text.Split(' ');
            string joinedCmdRegular = string.Join(" ", cmds.Skip(1));
            string joinedCmd = joinedCmdRegular.ToLower();
            
            if (cmds.Length < 2)
                return;
            
            switch (cmds[0])
            {
                /*
                 * /tp <username> 
                 */
                case "/tp":
                    foreach (Client client in Server.clients.Values)
                    {
                        if (client != null && client.player != null && client.player.username.ToLower()
                            .Contains(joinedCmd))
                        {
                            SetSuggestedText(__instance, cmds[0], client.player.username);
                            break;
                        }
                    }

                    break;

                /*
                 * /pickup [items, powerups] 
                 */
                case "/pickup":
                    foreach (string pickupOption in new string[] {"items", "powerups"})
                        if (pickupOption.Contains(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], pickupOption);
                    
                    break;
				
				/*
				 * /item <item name>
				 */
				case "item":
                    foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
                        if (inventoryItem.name.ToLower().Contains(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], inventoryItem.name);

                    break;
				
                /*
                 * /powerup <powerup name>
                 */
                case "powerup":
                    foreach (KeyValuePair<string, int> powerupPair in ItemManager.Instance.stringToPowerupId)
                        if (powerupPair.Key.ToLower().Contains(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], powerupPair.Key);

                    break;
            }
        }
    }
}