using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Muck1337.Utils;

namespace Muck1337
{
    [HarmonyPatch(typeof(Commands))]
    class CommandsPatches
    {
        private static void SetSuggestedText(object instance, string command, string input, string completion)
        {
            PrivateFinder.SetValue<string>(instance, "suggestedText", "/" + command + " " + input + completion.Substring(input.Length));
        }
        
        [HarmonyPatch("PredictCommands")]
        [HarmonyPostfix]
        static void PredictCommands_Postfix(Commands __instance)
        {
            string text = __instance.inputField.text;
            string[] cmds = text.Substring(1).Split(' ');
            string joinedCmdRegular = string.Join(" ", cmds.Skip(1));
            string joinedCmd = joinedCmdRegular.ToLower();
            
            if (cmds.Length < 2)
                return;
            
            switch (cmds[0])
            {
                /*
                 * /tp <username> 
                 */
                case "tp":
                    foreach (Client client in Server.clients.Values)
                    {
                        if (client != null && client.player != null && client.player.username.ToLower()
                            .StartsWith(joinedCmd))
                        {
                            SetSuggestedText(__instance, cmds[0], joinedCmd, client.player.username);
                            break;
                        }
                    }

                    break;

                /*
                 * /pickup [items, powerups] 
                 */
                case "pickup":
                case "pick":
                    foreach (string pickupOption in new [] {"items", "powerups"})
                        if (pickupOption.StartsWith(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], joinedCmd, pickupOption);
                    
                    break;
				
				/*
				 * /item <item name>
				 */
				case "item":
                case "i":
                    foreach (InventoryItem inventoryItem in ItemManager.Instance.allItems.Values)
                        if (inventoryItem.name.ToLower().StartsWith(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], joinedCmd, inventoryItem.name);

                    break;
				
                /*
                 * /powerup <powerup name>
                 */
                case "powerup":
                case "pow":
                    foreach (KeyValuePair<string, int> powerupPair in ItemManager.Instance.stringToPowerupId)
                        if (powerupPair.Key.ToLower().StartsWith(joinedCmd))
                            SetSuggestedText(__instance, cmds[0], joinedCmd, powerupPair.Key);

                    break;
            }
        }
    }
}