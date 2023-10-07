using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static GorillaNetworking.CosmeticsController;

namespace StrangeCosmetics
{
    [BepInPlugin("pl2w.strangecosmetics", "StrangeCosmetics", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
        static string path = Path.Combine(Application.streamingAssetsPath, "strangeCosmeticsSave.json");
        public static Text itemSlotOne, itemSlotTwo, itemSlotThree;

        Plugin()
        {
            new Harmony("pl2w.strangecosmetics").PatchAll(Assembly.GetExecutingAssembly());

            if(File.Exists(path))
            {
                string json = File.ReadAllText(path);
                StrangeCosmeticDict dict = JsonUtility.FromJson<StrangeCosmeticDict>(json);

                keyValuePairs = Enumerable.Range(0, dict.itemIds.Count).ToDictionary(i => dict.itemIds[i], i => dict.itemTags[i]);
                Debug.Log(json);
            }
            else
            {
                keyValuePairs = new Dictionary<string, int>();
            }
        }

        public static void OnTagPlayer()
        {
            List<string> wornItems = new List<string>();
            foreach (CosmeticsController.CosmeticItem item in CosmeticsController.instance.currentWornSet.items)
            {
                if(item.isNullItem) continue;

                wornItems.Add(item.itemName);
                Debug.Log(item.itemName);
            }

            for (int i = 0; i < wornItems.Count; i++)
            {
                if (!keyValuePairs.ContainsKey(wornItems[i]))
                {
                    keyValuePairs.Add(wornItems[i], 1);
                    continue;
                }
                keyValuePairs[wornItems[i]] += 1;
            }

            StrangeCosmeticDict dict = new StrangeCosmeticDict();
            dict.itemIds = keyValuePairs.Keys.ToList();
            dict.itemTags = keyValuePairs.Values.ToList();

            string json = JsonUtility.ToJson(dict);
            File.WriteAllText(path, json);

            RefreshStrangeCounts();
        }

        public static int GetStrangeCountForID(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                return keyValuePairs[id];
            }

            return 0;
        }

        public static void LoadMod()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StrangeCosmetics.Resources.strangecosmetics");
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();

            GameObject canvas = Instantiate(bundle.LoadAsset<GameObject>("ItemSlotsCanvas"));
            itemSlotOne = canvas.transform.Find("Slot1").gameObject.GetComponent<Text>();
            itemSlotTwo = canvas.transform.Find("Slot2").gameObject.GetComponent<Text>();
            itemSlotThree = canvas.transform.Find("Slot3").gameObject.GetComponent<Text>();

            bundle.Unload(false);

            RefreshStrangeCounts();
        }

        public static void RefreshStrangeCounts()
        {
            Wardrobe robe = CosmeticsController.instance.wardrobes[0];
            itemSlotOne.text = GetStrangeCountForID(robe.wardrobeItemButtons[0].currentCosmeticItem.itemName).ToString();
            itemSlotTwo.text = GetStrangeCountForID(robe.wardrobeItemButtons[1].currentCosmeticItem.itemName).ToString();
            itemSlotThree.text = GetStrangeCountForID(robe.wardrobeItemButtons[2].currentCosmeticItem.itemName).ToString();
        }
    }

    [Serializable]
    public class StrangeCosmeticDict
    {
        public List<string> itemIds = new List<string>();
        public List<int> itemTags = new List<int>();
    }
}
