using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewTask", menuName = "TaskSystem/Task")]
public class TaskData : ScriptableObject
{
    [Header("Setting")]
    public string taskID;
    public string taskName;
    [TextArea] public string description;

    [Header("Task Conditions")]
    public TaskCondition[] conditions;
    public TaskDependency[] taskDependency;

    //[Header("Callback Function")]
    //public UnityEvent onComplete;

    [Header("Callback Commands")]
    [TextArea] public string onCompleteCommandNames;

    public Task GetTask()
    {
        return new Task
        {
            _id = taskID,
            _conditions = conditions,
            _dependencies = taskDependency,
            _onCompleteCommandNames = onCompleteCommandNames
        };
    }
}