using HarmonyLib;

namespace Spood.LethalCompany.ExpressDelivery.Patches
{
    [HarmonyPatch(typeof(ItemDropship), "ShipLeave")]
    internal class ItemDropshipPatchShipLeaveMethod
    {
        public static bool Prefix(ref float ___shipTimer)
        {
            ___shipTimer = 0f;

            return true;
        }
    }
}
