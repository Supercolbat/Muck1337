using System;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace Muck1337.Utils
{
    
    /*
     * Will get around improving this sometime
     * TODO: make better
     */
    public class SettingsManager
    {
        private const string ConfigFilename = "Muck1337.cfg";
        private static ConfigFile _configFile;

        public static bool LoadSettings()
        {
            try
            {
                /*
                _configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ConfigFilename), true);
                Muck1337Settings obj = new Muck1337Settings();

                foreach(PropertyInfo objProp in obj.GetType().GetProperties()) {
                    if (objProp.CanRead) {
                        object[] indexer = new object[0];
                        object nestedObj = objProp.GetValue(obj, indexer);

                        foreach(PropertyInfo nestedProp in nestedObj.GetType().GetProperties()) {
                            if (nestedProp.CanRead && nestedProp.DeclaringType != null) {
                                ConfigDefinition key = new ConfigDefinition(nestedProp.DeclaringType.Name, nestedProp.Name);
                                _configFile[key].BoxedValue = nestedProp.GetValue(nestedObj, indexer).ToString();  // Some variables are not strings so the .ToString() is required
                            }
                        }
                    }
                }

                List<PropertyInfo> properties = AccessTools.GetDeclaredProperties(typeof(Muck1337Settings));
                MethodInfo configBindMethod = typeof(ConfigFile)
                    .GetFields()
                    .Where(m => m.Name == nameof(ConfigFile.Bind))
                    .First(m => m.IsGenericMethod && m.GetParameters().Length == 4);

                foreach (PropertyInfo prop in properties)
                {
                    object entry = configBindMethod.MakeGenericMethod(prop.PropertyType).Invoke(configFile,
                        new [] { SECTION_NAME, prop.Name, prop.GetValue(Options), null });

                    Type entryType = typeof(ConfigEntry<>).MakeGenericType(prop.PropertyType);
                    prop.SetValue(Options, AccessTools.Property(entryType, "Value").GetValue(entry));
                }
                */
            }
            catch (Exception e)
            {
                Muck1337Plugin.Instance.log.LogError($"Failed to load configuration file {ConfigFilename}\n{e}");
                return false;
            }

            return true;
        }

        public static bool SaveSettings()
        {
            try
            {
                Muck1337Settings obj = new Muck1337Settings();

                foreach(PropertyInfo objProp in obj.GetType().GetProperties()) {
                    if (objProp.CanRead) {
                        object[] indexer = new object[0];
                        object nestedObj = objProp.GetValue(obj, indexer);

                        foreach(PropertyInfo nestedProp in nestedObj.GetType().GetProperties()) {
                            if (nestedProp.CanRead && nestedProp.DeclaringType != null) {
                                ConfigDefinition key = new ConfigDefinition(nestedProp.DeclaringType.Name, nestedProp.Name);
                                _configFile[key].BoxedValue = nestedProp.GetValue(nestedObj, indexer).ToString();  // Some variables are not strings so the .ToString() is required
                            }
                        }
                    }
                }
                _configFile.Save();
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