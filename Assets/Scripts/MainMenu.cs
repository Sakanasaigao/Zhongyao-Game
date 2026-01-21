/*
 * 中药游戏项目 - MainMenu.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 主菜单界面逻辑控制器
 * - 处理游戏开始、继续、退出等功能按钮的点击事件
 * - 与存档系统交互，检查是否存在存档文件
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using ARCHIVE;

public class MainMenu : MonoBehaviour
{
    public Button startGameButton;
    public Button continueGameButton;
    public Button exitGameButton;

    public SceneLoad sceneLoad;

    [SerializeField]
    ArchivingManager archivingManager => ArchivingManager.Instance;
    Player player;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        continueGameButton.onClick.AddListener(ContinueGame);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        sceneLoad.LoadSceneByIndex(1);
    }

    public void ContinueGame()
    {
        if (ArchivingManager.Instance.HaveArchive())
        {
            // 加载存档，ArchivingManager.Load()会自动根据存档中的场景ID加载相应场景
            ArchivingManager.Instance.Load();
        }
        else
        {
            Debug.LogWarning("No save file found! Starting new game.");
            StartGame();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}