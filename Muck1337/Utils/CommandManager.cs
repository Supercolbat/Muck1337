using System;
using System.Collections.Generic;

namespace Muck1337.Utils
{
    public readonly struct Muck1337Command
    {
        public readonly Action<string[]> Callback;
        public readonly Func<string, string> Prediction;
        public Muck1337Command(Action<string[]> callback, Func<string, string> prediction)
        {
            Callback = callback;
            Prediction = prediction;
        }
    }
    
    public static class CommandManager
    {
        public static readonly Dictionary<string, Muck1337Command> Commands = new Dictionary<string, Muck1337Command>();
        public static readonly Dictionary<string, string> CommandNames = new Dictionary<string, string>();

        public static void AddCommand(string name, Action<string[]> callback, string[] aliases = null, Func<string, string> prediction = null)
        {
            // Assign aliases that point to the original name 
            CommandNames[name] = name;

            if (aliases != null)
            {
                foreach (string alias in aliases)
                {
                    CommandNames[alias] = name;
                }
            }
            
            // Create prediction command if null
            if (prediction == null)
                // create a function that returns name
                prediction = s => name;
            
            // Create a new Command object and add to Commands
            Commands[name] = new Muck1337Command(callback, prediction);
        }
    }
}