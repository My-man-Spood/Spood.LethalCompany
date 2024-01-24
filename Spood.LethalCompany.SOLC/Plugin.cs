using BepInEx;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Spood.LethalCompany.SOLC
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle;

        private void Awake()
        {
            var assetDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "spood.lethalcompany.solc");
            bundle = AssetBundle.LoadFromFile(assetDirectory);

            CustomItems.RegisterCustomItems(bundle);
            RegisterScrewdriver(bundle);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void RegisterScrewdriver(AssetBundle bundle)
        {
            Item screwDriver = bundle.LoadAsset<Item>("Assets/Spood.LethalCompany.SOLC/Screwdriver/ScrewdriverItem.asset");
            Screwdriver screwdriverScript = screwDriver.spawnPrefab.AddComponent<Screwdriver>();
            screwdriverScript.grabbable = true;
            screwdriverScript.grabbableToEnemies = true;
            screwdriverScript.itemProperties = screwDriver;

            Utilities.FixMixerGroups(screwDriver.spawnPrefab);

            var node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Unscrew pesky bolts! Can also dismantle objects.\n\n";

            NetworkPrefabs.RegisterNetworkPrefab(screwDriver.spawnPrefab);
            Items.RegisterShopItem(screwDriver, null, null, node, 1);
        }
    }
}
