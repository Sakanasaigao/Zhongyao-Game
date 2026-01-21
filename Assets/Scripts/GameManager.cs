/*
 * 中药游戏项目 - GameManager.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 游戏的核心管理器，采用单例模式
 * - 负责统筹各个游戏模块的初始化和运行
 * - 管理玩家对象和游戏全局状态
 */
using UnityEngine;
using System.IO; // �����ļ�����
using System.Runtime.Serialization.Formatters.Binary; // ���ڶ��������л�

public class GameManager : MonoBehaviour
{
    public Player player;
    private string saveFilePath;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}