using System;
using System.Linq;
using HarmonyLib;
using Muck1337.Utils;

namespace Muck1337
{
    [HarmonyPatch(typeof(Commands))]
    class CommandsPatches
    {
        [HarmonyPatch("PredictCommands")]
        [HarmonyPostfix]
        static void PredictCommands_Postfix(Commands __instance)
        {
            string text = __instance.inputField.text;
            string[] cmds = text.Split(' ');
            
            if (cmds.Length < 2)
            {
                return;
            }
            
            if (cmds[0] == "/tp")
            {
                foreach (Client client in Server.clients.Values)
                {
                    if (client != null && client.player != null && client.player.username.ToLower().Contains(string.Join(" ", cmds.Skip(1)).ToLower()))
                    {
                        PrivateFinder.SetValue<string>(__instance, "suggestedText", cmds[0] + " ");
                        PrivateFinder.SetValue<string>(__instance, "suggestedText", PrivateFinder.GetValue<string>(__instance, "suggestedText") + client.player.username);
                        break;
                    }
                }
            }
        }
    }
}