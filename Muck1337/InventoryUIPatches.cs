using HarmonyLib;

namespace Muck1337
{
    [HarmonyPatch(typeof(InventoryUI), "GetMoney")]
    class GetMoneyPatch
    {
        static bool Prefix(ref int __result)
        {
            __result = int.MaxValue;
            return false;
        }
    }
}