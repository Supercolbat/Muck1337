using System;
using System.Linq;
using HarmonyLib;
using Muck1337.Utils;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(Commands))]
    class CommandsPatches
    {
        private static void SetSuggestedText(object instance, string command, string input, string completion)
        {
            PrivateFinder.SetValue(instance, "suggestedText", "/" + command + " " + input + completion.Substring(input.Length));
        }
        
        [HarmonyPatch("PredictCommands")]
        [HarmonyPostfix]
        static void PredictCommands_Postfix(Commands __instance)
        {
            string text = __instance.inputField.text;
            if (text.Length < 2 || text.Count(char.IsWhiteSpace) < 1)
                return;
            
            string[] split = text.Substring(1).Split(' ');
            string joinedCmd = String.Join(" ", split.Skip(1));

            // Get the true command name (used in CommandManager.commands) for the entered command
            bool commandExists = CommandManager.CommandNames.TryGetValue(split[0], out string trueCommandName);
			
            // If the entered command doesn't match a Muck1337Command, then stop
            if (!commandExists)
                return;
			
            // Call the prediction method and set it as the suggested text
            SetSuggestedText(__instance, split[0], joinedCmd, CommandManager.Commands[trueCommandName].Prediction(joinedCmd.ToLower()));
        }
    }
}