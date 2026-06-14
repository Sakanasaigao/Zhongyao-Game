using System;
using System.Collections.Generic;
using System.Linq;
using COMMANDS;
using COUNTER;
using DIALOGUE;
using ITEMS;
using UnityEngine;

namespace MEDICINE
{
    public class CMD_DatabaseExtension_Medicine : CMD_DatabaseExtension
    {
        private static string[] PARAM_COUNT => new string[] { "c", "count" };
        private static string[] PARAM_NECESSARY => new string[] { "n", "necessary" };
        private static string[] PARAM_ITEMS => new string[] { "i", "items" };
        private static string[] PARAM_TARGETS => new string[] { "t", "targets" };
        private static string[] PARAM_POSITION => new string[] { "p", "position" };
        private static string[] PARAM_FILE => new string[] { "f", "file" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("putmedicineoncounter", new Action<string[]>(PutMedicineOnCounter));
            database.AddCommand("addmedicinetowarehouse", new Action<string[]>(AddMedicineToWarehouse));
            database.AddCommand("createmedicinepot", new Action<string[]>(CreateMedicinePot));
        }

        private static void CreateMedicinePot(string[] data)
        {
            // 旧药罐系统已弃用，改用Decoct场景的煎药小游戏
            Debug.Log("旧药罐系统已弃用，CreateMedicinePot 命令跳过");
        }

        private static (string[] IndicesFormatted, string[] ShuffledArray) MergeAndShuffle(string[] a, string[] b)
        {
            var random = new System.Random();
            var combined = a.Concat(b).ToArray();
            var indices = Enumerable.Range(0, combined.Length).ToList();
            var shuffledIndices = indices.OrderBy(_ => random.Next()).ToList();
            var shuffledArray = shuffledIndices.Select(i => combined[i]).ToArray();

            string[] result = new string[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                int indexInShuffled = Array.IndexOf(shuffledArray, a[i]);
                result[i] = (indexInShuffled + 1).ToString("D2");
            }

            return (result, shuffledArray);
        }


        private static string[] ParseStringArray(string input)
        {
            return input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void AddMedicineToWarehouse(string[] data)
        {
            CommandParameters parameters = new CommandParameters(data);

            if (!parameters.TryGetValue(PARAM_ITEMS, out string itemsStr, ""))
            {
                Debug.LogError("AddMedicineToWarehouse 需要 -items 参数");
                return;
            }

            string[] items = itemsStr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (ItemWarehouse.Instance == null)
            {
                Debug.LogError("ItemWarehouse 实例未找到");
                return;
            }

            foreach (string item in items)
            {
                string trimmedItem = item.Trim();
                if (!string.IsNullOrEmpty(trimmedItem))
                {
                    bool success = ItemWarehouse.Instance.AddItem(trimmedItem);
                    if (!success)
                        Debug.LogWarning($"药品 '{trimmedItem}' 已存在，添加失败");
                }
            }
        }

        private static void PutMedicineOnCounter(string[] data)
        {
            CommandParameters parameters = new CommandParameters(data);

            if (!parameters.TryGetValue(PARAM_COUNT, out int count, 0))
            {
                Debug.LogError("PutMedicineOnCounter 需要 -count 参数");
                return;
            }

            List<string> necessaryMeds = new List<string>();
            if (parameters.TryGetValue(PARAM_NECESSARY, out string medsStr, ""))
            {
                string[] medArray = medsStr.Split(' ');
                foreach (string med in medArray)
                {
                    if (!string.IsNullOrEmpty(med))
                        necessaryMeds.Add(med.Trim());
                }
            }

            if (CounterManager.instance != null)
                CounterManager.instance.PlaceAllMedicine(count, necessaryMeds);
            else
                Debug.LogError("CounterManager 实例未找到");
        }
    }
}