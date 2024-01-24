using HarmonyLib;
using System;
using UnityEngine;

namespace Spood.LethalCompany.ExpressDelivery.Patches
{
    [HarmonyPatch(typeof(ItemDropship), "Update")]
    internal class ItemDropshipPatchUpdateMethod
    {
        private static int expressDeliveryIndex = -1;
        private static float timerTarget = 0f;

        private static bool Prefix(
            ref ItemDropship __instance,
            ref bool ___deliveringOrder,
            ref Terminal ___terminalScript,
            ref StartOfRound ___playersManager,
            ref float ___shipTimer,
            ref bool ___playersFirstOrder,
            ref bool ___shipLanded,
            ref float ___noiseInterval,
            ref int ___timesPlayedWithoutTurningOff,
            ref bool ___shipDoorsOpened)
        {
            if (!__instance.IsServer)
            {
                return false;
            }

            if (!___deliveringOrder)
            {
                var hasOrderedItems = ___terminalScript.orderedItemsFromTerminal.Count > 0;
                if (hasOrderedItems)
                {
                    ___shipTimer += Time.deltaTime;
                    Debug.Log("ship timer:" + ___shipTimer);
                    // cache the express delivery index
                    if (expressDeliveryIndex == -1)
                    {
                        expressDeliveryIndex = FindExpressDeliveryIndex(___terminalScript);
                    }

                    // the BuyExpress and BuyRegular methods will be called repeatedly but it should always
                    // be under the same conditions. This is not elegant but it's the best way i could think of
                    // that was still somewhat readable
                    if (___terminalScript.orderedItemsFromTerminal.Contains(expressDeliveryIndex) || ___playersFirstOrder)
                    {
                        BuyExpress(___playersManager, ref __instance);
                    }
                    else
                    {
                        BuyRegular(___playersManager, ref __instance);
                    }
                    
                    if (___shipTimer > timerTarget)
                    {
                        Debug.Log("target timer:" + timerTarget);
                        ___shipTimer = 0f;
                        var landShipOnServerMethod = AccessTools.Method(typeof(ItemDropship), "LandShipOnServer");
                        landShipOnServerMethod.Invoke(__instance, null);
                    }
                }
            }
            else if (___shipLanded)
            {
                ___shipTimer += Time.deltaTime;
                if (___shipTimer > 30f)
                {
                    ___timesPlayedWithoutTurningOff = 0;
                    ___shipLanded = false;
                    var shipLeaveClientRpc = AccessTools.Method(typeof(ItemDropship), "ShipLeaveClientRpc");
                    shipLeaveClientRpc.Invoke(__instance, null);
                }
                if (___noiseInterval <= 0f)
                {
                    ___noiseInterval = 1f;
                    ___timesPlayedWithoutTurningOff++;
                    RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 60f, 1.3f, ___timesPlayedWithoutTurningOff, noiseIsInsideClosedShip: false, 94);
                }
                else
                {
                    ___noiseInterval -= Time.deltaTime;
                }
            }

            return false;
        }

        private static int FindExpressDeliveryIndex(Terminal terminal)
        {
            return Array.FindIndex(terminal.buyableItemsList, x => x.name == "ExpressDeliveryItem");
        }

        private static void BuyExpress(StartOfRound playersManager, ref ItemDropship instance)
        {
            if (!playersManager.shipHasLanded)
            {
                timerTarget = 12f;
            }
            else
            {
                timerTarget = 5f;
            }
        }

        private static void BuyRegular(StartOfRound playersManager, ref ItemDropship instance)
        {
            if (!playersManager.shipHasLanded)
            {
                timerTarget = 60f;
            }
            else
            {
                timerTarget = 40f;
            }
        }
    }
}
