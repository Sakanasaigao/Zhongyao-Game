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
                DontDestroyOnLoad(gameObject);
            }
        }

        public bool AddItem(string itemName)
        {
            if (!_obtainedItems.Contains(itemName))
            {
                _obtainedItems.Add(itemName);
                OnItemAdded?.Invoke(itemName);
                Debug.Log($"ItemWH add item {itemName} list: {_obtainedItems}");
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

        public void _test_LogAllItems()
        {
            foreach (var item in _obtainedItems)
            {
                Debug.Log($"IWH has {item}");
            }
            if (_obtainedItems.Count == 0)
            {
                Debug.Log("IWH is empty");
            }
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