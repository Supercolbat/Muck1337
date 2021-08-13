using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Muck1337.Patches;
using Muck1337.Utils;

namespace Muck1337
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Muck1337Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "me.supercolbat.muck1337";
        public const string PluginName = "Muck1337";
        public const string PluginVersion = "0.3.0";

        public static Muck1337Plugin Instance;
        private Harmony harmony;
        public ManualLogSource log;

        public void Awake()
        {
            Instance = this;
            harmony = new Harmony(PluginGuid);
            log = Logger;

            // ConfigManager.LoadConfig();
            
            /*
             * Check for new version
             */
            // string fetchedVersion = VersionUtil.GetVersion();
            // // String.Compare returns -1 if the fetched version is smaller, 0 if they are equal, and 1 if fetched version is greater
            // if (String.Compare(fetchedVersion, PluginVersion, StringComparison.Ordinal) > 0)
            //     StatusMessage.Instance.DisplayMessage($"Muck1337 can be updated to version {fetchedVersion}!");
            
            /*
             * Patch harmony
             */
            try
            {
                ChatBoxPatches.InitCommands();
            }
            catch(Exception e)
            {
                log.LogError(e);
            }
            
            harmony.PatchAll();
            log.LogMessage("Mod initialized!!111one");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
    
}