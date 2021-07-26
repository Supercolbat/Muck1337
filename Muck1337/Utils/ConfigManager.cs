using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace Muck1337.Utils
{
    /*
     * Will get around improving this sometime
     * TODO: make better
     */
    public class ConfigManager
    {
        public static class ToggleMods
        {
            public static string SectionName = "ToggleMods"; 
                
            public static ConfigEntry<bool> Achievements;
        }
        
        
        private const string ConfigFilename = "Muck1337.cfg";
        private static ConfigFile configFile;

        public static bool LoadConfig()
        {
            try
            {
                configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ConfigFilename), true);
                ToggleMods.Achievements = configFile.Bind("ToggleMods", "Achievements", false, "");
            }
            catch (Exception e)
            {
                Muck1337Plugin.Instance.log.LogError($"Failed to load configuration file {ConfigFilename}\n{e}");
                return false;
            }

            return true;
        }

        public static bool SaveConfig()
        {
            try
            {
                List<PropertyInfo> toggleModsProperties = AccessTools.GetDeclaredProperties(typeof(ToggleMods));
                foreach (PropertyInfo propertyInfo in toggleModsProperties)
                {
                    ConfigDefinition key = new ConfigDefinition(ToggleMods.SectionName, propertyInfo.Name);
                    // configFile[key].BoxedValue = propertyInfo.GetValue(ToggleMods);
                }
                configFile.Save();
            }
            catch (Exception e)
            {
                Muck1337Plugin.Instance.log.LogError($"Failed to save configuration file {ConfigFilename}\n{e}");
                return false;
            }

            return true;
        }
    }
}