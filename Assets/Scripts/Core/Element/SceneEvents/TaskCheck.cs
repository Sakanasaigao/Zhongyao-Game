using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCheck : MonoBehaviour
{
    void Start()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.CheckTaskProgress();
        }
        else
        {
            Debug.LogWarning("TaskManager instance not found, skipping task progress check");
        }
    }
}
