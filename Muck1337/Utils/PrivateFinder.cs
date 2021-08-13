using System.Reflection;
using HarmonyLib;

namespace Muck1337.Utils
{
    public static class PrivateFinder
    {
        /*
         * Hippity-hoppity,
         * your code is now my property.
         * https://github.com/ugackMiner53/Mitch-Client/blob/main/PlayerInputPatch.cs
         */
        
        public static T GetValue<T>(object instance, string fieldName)
        {
            return (T)Traverse.Create(instance).Field(fieldName).GetValue();
        }

        public static void SetValue(object instance, string fieldName, object value)
        {
            // Thank you to @funnynumber#3171 for the good code
            Traverse.Create(instance).Field(fieldName).SetValue(value);
        }
        
        /*
         * More copied code?!?
         * Who would've known?
         * https://stackoverflow.com/a/135482
         */

        public static void CallMethod(object instance, string methodName, object[] parameters=null)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(instance, parameters);
        }
    }
}