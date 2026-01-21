/*
 * 中药游戏项目 - ItemWarehouse.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 物品仓库管理类，采用单例模式
 * - 负责管理玩家拥有的所有物品
 * - 提供添加、移除、检查和获取物品的方法
 * - 支持默认物品初始化和随机获取物品功能
 */
using System.Collections.Generic;
using UnityEngine;

namespace ITEMS
{
    public class ItemWarehouse : MonoBehaviour
    {
        private static ItemWarehouse _instance;
        public static ItemWarehouse Instance => _instance;

        public int ItemCount => _obtainedItems.Count;

        [SerializeField]
        private List<string> _obtainedItems = new List<string>();
        public event System.Action<string> OnItemAdded;
        public event System.Action<string> OnItemRemoved;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public bool AddItem(string itemName)
        {
            if (!_obtainedItems.Contains(itemName))
            {
                _obtainedItems.Add(itemName);
                OnItemAdded?.Invoke(itemName);
                return true;
            }
            return false;
        }

        public bool AddItem(Item item)
        {
            return AddItem(item.name);
        }

        public bool AddItem(ItemConfigData config)
        {
            return AddItemByConfig(config);
        }

        public bool RemoveItem(string itemName)
        {
            if (_obtainedItems.Remove(itemName))
            {
                OnItemRemoved?.Invoke(itemName);
                return true;
            }
            return false;
        }

        public bool HasItem(string itemName)
        {
            return _obtainedItems.Contains(itemName);
        }

        public List<string> GetAllItems()
        {
            return new List<string>(_obtainedItems);
        }

        public void ClearWarehouse()
        {
            _obtainedItems.Clear();
            Debug.Log("Warehouse has been cleared");
        }

        public void InitializeWithDefaultItems(params string[] defaultItems)
        {
            foreach (var item in defaultItems)
            {
                AddItem(item);
            }
        }

        public bool AddItemByConfig(ItemConfigData config)
        {
            if (config != null)
            {
                return AddItem(config.name);
            }
            return false;
        }

        public string[] GetRandomItemsExcluding(int count, List<string> excludedItems = null)
        {
            List<string> availableItems = new List<string>();
            foreach (var item in _obtainedItems)
            {
                if (excludedItems == null || !excludedItems.Contains(item))
                {
                    availableItems.Add(item);
                }
            }

            if (count <= 0 || availableItems.Count == 0)
            {
                return new string[0];
            }

            int actualCount = Mathf.Min(count, availableItems.Count);
            string[] result = new string[actualCount];

            for (int i = 0; i < actualCount; i++)
            {
                int randomIndex = Random.Range(0, availableItems.Count);
                result[i] = availableItems[randomIndex];
                availableItems.RemoveAt(randomIndex);
            }

            return result;
        }

        public string[] GetRandomItemsExcluding(int count, params string[] excludedItems)
        {
            return GetRandomItemsExcluding(count,
                excludedItems != null ? new List<string>(excludedItems) : null);
        }
    }
}