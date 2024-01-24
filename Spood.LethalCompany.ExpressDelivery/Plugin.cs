using BepInEx;
using HarmonyLib;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Spood.LethalCompany.ExpressDelivery
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle;

        private void Awake()
        {
            var assetDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "spood.lethalcompany.expressdelivery");
            bundle = AssetBundle.LoadFromFile(assetDirectory);

            var harmony = new Harmony("Spood.LethalCompany.ExpressDelivery");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            RegisterExpressDelivery();
        }

        private void RegisterExpressDelivery()
        {
            Item expressDelivery = bundle.LoadAsset<Item>("Assets/Spood.LethalCompany.ExpressDelivery/ExpressDelivery/ExpressDeliveryItem.asset");

            Utilities.FixMixerGroups(expressDelivery.spawnPrefab);

            var node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Receive items faster!\n\n";

            NetworkPrefabs.RegisterNetworkPrefab(expressDelivery.spawnPrefab);
            Items.RegisterShopItem(expressDelivery, null, null, node, 50);
        }
    }
}
