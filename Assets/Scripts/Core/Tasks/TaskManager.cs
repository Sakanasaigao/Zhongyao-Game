/*
 * 中药游戏项目 - TaskManager.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 任务系统的核心管理器，采用单例模式
 * - 负责任务的添加、管理和完成状态检查
 * - 支持通过名称添加任务和获取活跃任务列表
 * - 提供任务进度检查和完成验证功能
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [SerializeField] List<Task> activeTasks = new List<Task>();
    private Dictionary<string, Task> _allTasks = new Dictionary<string, Task>();
    public Dictionary<string, Task> allTasks => _allTasks;
    List<ScriptableObject> allObjects => new List<ScriptableObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTaskByName(string taskName)
    {
        TaskData taskDataAsset = Resources.Load<TaskData>(FilePaths.GetPathToResource(FilePaths.resources_task, taskName));

        if (taskDataAsset != null)
        {
            Task task = taskDataAsset.GetTask();
            AddTaskByTask(task);
        }
        else
        {
            Debug.LogWarning($"Task with name {taskName} not found in resources.");
        }
    }

    public void RemoveTaskByName(string taskName)
    {
        if (allTasks.TryGetValue(taskName, out Task task))
        {
            activeTasks.Remove(task);
            allTasks.Remove(taskName);

            task.Cleanup();
            Debug.Log($"Task {taskName} removed successfully.");
        }
        else
        {
            Debug.LogWarning($"Task with name {taskName} not found.");
        }
    }

    public List<string> GetActiveTaskIDs()
    {
        List<string> activeIDs = new List<string>();
        foreach (var task in allTasks.Values)
        {
            if (task.IsActive)
            {
                activeIDs.Add(task.ID);
            }
        }
        return activeIDs;
    }

    private void AddTaskByTask(Task task)
    {
        if (!allTasks.ContainsKey(task.ID))
        {
            activeTasks.Add(task);
            allTasks.Add(task.ID, task);
            task.Initialize();
        }
    }

    private void RemoveTaskByTask(Task task)
    {
        if (allTasks.ContainsKey(task.ID))
        {
            activeTasks.Remove(task);
            allTasks.Remove(task.ID);
        }
    }

    public void CheckTaskProgress()
    {
        foreach (var task in activeTasks.ToArray())
        {
            if (!task.AreDependenciesMet())
            {
                Debug.Log(task.ID + " not met");

                continue;
            }

            if (task.CheckCompletion())
            {
                Debug.Log(task.ID + " complete");
                task.Complete();

            }
            else
            {
                Debug.Log(task.ID + " not complete");
            }
        }
    }

    public void SaveTasks()
    {
        // �浵
    }

    public void LoadTasks()
    {
        // ����
    }
}