using LethalLib.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace Spood.LethalCompany.SOLC
{
    public static class CustomItems
    {
        public static List<CustomItem> GetShopItems()
        {
            return new List<CustomItem>()
            {
                new CustomItem (
                    "Assets/Spood.LethalCompany.SOLC/Cube/CubeItem.asset",
                    "a cube.\n\n",
                    1,
                    10),
                new CustomItem (
                    "Assets/Spood.LethalCompany.SOLC/Pipe/PipeItem.asset",
                    "a pipe.\n\n",
                    1,
                    10),
                new CustomItem(
                    "Assets/Spood.LethalCompany.SOLC/ScrapMetal/ScrapMetal.asset",
                    "you could easily carry a lot of these.\n\n",
                    1,
                    10)
            };
        }

        public static void RegisterCustomItems(AssetBundle bundle)
        {
            foreach(var item in GetShopItems())
            {
                RegisterCustomItem(bundle, item);
            }
        }

        public static void RegisterCustomItem(AssetBundle bundle, CustomItem customItem)
        {
            Item ItemAsset = bundle.LoadAsset<Item>(customItem.AssetPath);
            Utilities.FixMixerGroups(ItemAsset.spawnPrefab);
            NetworkPrefabs.RegisterNetworkPrefab(ItemAsset.spawnPrefab);

            if (customItem.Price > 0)
            {
                var node = ScriptableObject.CreateInstance<TerminalNode>();
                node.clearPreviousText = true;
                node.displayText = customItem.TerminalDisplayText;

                Items.RegisterShopItem(ItemAsset, null, null, node, 1);
            }
            
            if(customItem.ScrapChance > 0)
            {
                Items.RegisterScrap(ItemAsset, customItem.ScrapChance, Levels.LevelTypes.All);
            }
        }
    }

    public class CustomItem
    {
        public string AssetPath { get; }
        public string TerminalDisplayText { get; }
        public int Price { get; }
        public int ScrapChance { get; }

        public CustomItem(string assetPath, string terminalText, int price, int scrapChance)
        {
            AssetPath = assetPath;
            TerminalDisplayText = terminalText;
            Price = price;
            ScrapChance = scrapChance;
        }
    }
}
