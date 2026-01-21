/*
 * 中药游戏项目 - Player.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 玩家角色类，管理玩家的核心状态
 * - 存储当前场景、拥有物品、游戏脚本进度、任务列表等信息
 * - 提供保存和加载玩家数据的方法
 */
using ITEMS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int scene; // ��������ĳ�������
    public string[] items; // ���ӵ�е���Ʒ���������飩
    public int gameScriptIndex; // ��������ľ������
    public bool inGal; // ����Ƿ���gal״̬
    public int timesPlayedGame; // ��ҽ�����Ϸ����
    public string[] tasks;

    public PlayerData SavePlayerData()
    {
        // ������ҵĵ�ǰ״̬
        return new PlayerData
        {
            scene = SceneManager.GetActiveScene().buildIndex,
            items = ItemWarehouse.Instance.GetAllItems().ToArray(),
            storyProgress = gameScriptIndex,
            inGal = inGal,
            tasks = TaskManager.Instance.GetActiveTaskIDs().ToArray(),
        };
    }

    public void LoadPlayerData(PlayerData data)
    {
        // ������ҵ�״̬
        scene = data.scene;
        items = data.items;
        gameScriptIndex = data.storyProgress;
        inGal = data.inGal;
        tasks = data.tasks;
    }
}

[System.Serializable]
public class PlayerData
{
    public int scene; // ��������ĳ�������
    public string[] items; // ���ӵ�е���Ʒ
    public int storyProgress; // ��������ľ������
    public bool inGal; // ����Ƿ���gal״̬
    public string[] tasks;
}