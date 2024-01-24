using HarmonyLib;

namespace Spood.LethalCompany.ExpressDelivery.Patches
{
    /*
     sets the ship timer to 25 seconds when we open the door
     the original code waits for the ship timer to get to 30 to make the ship take off
     by setting it to 25 we make the ship take off 5 seconds after the door opens and will keep the
     30 seconds timer for when the doors are never opened
     */
    [HarmonyPatch(typeof(ItemDropship), "OpenShipDoorsOnServer")]
    internal class ItemDropshipPatchOpenShipDoorOnServerMethod
    {
        private static bool Prefix(ref ItemDropship __instance, ref bool ___shipDoorsOpened, ref float ___shipTimer)
        {
            ___shipTimer = 25f;
            return true;
        }
    }
}
