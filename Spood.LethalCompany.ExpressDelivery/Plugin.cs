using BepInEx;
using HarmonyLib;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Spood.LethalCompany.ExpressDelivery
{ 
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Spood.LethalCompany.ExpressDelivery";
        public const string PLUGIN_NAME = "Spood's Express Delivery";
        public const string PLUGIN_VERSION = "1.0.1";
        // hello
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
