
using System;
using System.Collections.Generic;
using System.IO;
using ITEMS;
using UnityEngine;

namespace ARCHIVE
{
    public class ArchivingManager : MonoBehaviour
    {
        public static ArchivingManager Instance { get; private set; }

        [SerializeField] private GameObject playerObject;

        private string saveFileName = "archive.txt";

        private string savePath
        {
            get
            {
                string folderPath = Path.Combine(Application.persistentDataPath, "Archive");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return Path.Combine(folderPath, saveFileName);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Save()
        {
            try
            {
                string jsonToSave = GetDataToArchive();
                File.WriteAllText(savePath, jsonToSave);
                Debug.Log("Save successful!" + savePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Save failed: " + ex.Message);
            }
        }

        public void Load()
        {
            PlayerData playerData = GetDataToLoad();
            if (playerData == null)
            {
                Debug.LogWarning("No available save data");
                return;
            }

            Player player = playerObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player component not found");
                return;
            }

            // 更新玩家数据
            player.scene = playerData.playerScene;
            player.timesPlayedGame = playerData.timesPlayerGame + 1;
            player.gameScriptIndex = playerData.gameScriptIndex;

            // 加载物品和任务
            foreach (var item in playerData.items)
            {
                ItemWarehouse.Instance.AddItem(item);
            }

            foreach (var task in playerData.tasks)
            {
                TaskManager.Instance.AddTaskByName(task);
            }

            // 根据存档中的场景ID加载场景
            if (playerData.playerScene >= 0)
            {
                SceneLoad.Instance.LoadSceneByIndex(playerData.playerScene);
            }

            Debug.Log("Save data loaded successfully");
        }

        public bool HaveArchive()
        {
            return File.Exists(savePath);
        }

        private PlayerData GetDataToLoad()
        {
            try
            {
                if (!File.Exists(savePath)) return null;

                string jsonData = File.ReadAllText(savePath);
                return JsonUtility.FromJson<PlayerData>(jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read save data: " + ex.Message);
                return null;
            }
        }

        private string GetDataToArchive()
        {
            if (playerObject == null)
            {
                Debug.LogError("Player object not assigned!");
                return "{}";
            }

            Player player = playerObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player component missing!");
                return "{}";
            }

            List<string> itemList = ItemWarehouse.Instance.GetAllItems();
            List<string> taskList = TaskManager.Instance.GetActiveTaskIDs();

            // 获取当前活动场景的索引
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            PlayerData data = new PlayerData
            {
                playerScene = currentSceneIndex,
                items = itemList,
                timesPlayerGame = player.timesPlayedGame,
                tasks = taskList,
                gameScriptIndex = player.gameScriptIndex
            };

            return JsonUtility.ToJson(data, true);
        }

        [Serializable]
        private class PlayerData
        {
            public int playerScene;
            public List<string> items;
            public int timesPlayerGame;
            public List<string> tasks;
            public int gameScriptIndex;
        }
    }
}