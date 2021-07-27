using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Muck1337
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Muck1337Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "me.supercolbat.muck1337";
        private const string PluginName = "Muck1337";
        private const string PluginVersion = "0.3.0";

        public static Muck1337Plugin Instance;
        private Harmony harmony;
        public ManualLogSource log;

        public void Awake()
        {
            log = Logger;
            harmony = new Harmony(PluginGuid);

            // ConfigManager.LoadConfig();
            
            harmony.PatchAll();
            log.LogInfo("[Muck1337] Mod initialized!!111one");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
    
}