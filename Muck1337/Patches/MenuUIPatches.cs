using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Muck1337.Patches
{
    [HarmonyPatch(typeof(MenuUI))]
    public class MenuUIPatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start_Postfix(MenuUI __instance)
        {
            /*
            var buttonsParent = __instance.mainUi.GetComponentInChildren<GameObject>();
            var buttons = buttonsParent.GetComponentsInChildren<GameObject>();
            
            foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>())
            {
                if (gameObject.name == "Buttons")
                {
                    gameObject.
                }
            }
            */

            __instance.version.text = __instance.version.text + "\nMuck1337 " + Muck1337Plugin.PluginVersion;
        }
    }
}