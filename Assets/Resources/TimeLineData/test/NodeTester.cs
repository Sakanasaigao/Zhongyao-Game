using Core.TIMELINE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NodeTester : MonoBehaviour
{
    public string testTimeLine;
    [Inject] TimeLineManager timeLineManager;

    public void StartTestTimeLine()
    {
        if (timeLineManager == null)
        {
            Debug.Log("manager is null");
        }
        var timeLine = timeLineManager.LoadTimeLine<NodeTester>(testTimeLine, this);
        timeLine.Play();
    }

    public void NodeIn()
    {
        Debug.Log("Node In");
    }

    public void NodeOut()
    {
        Debug.Log("Node Out");
    }

    public void NodeUpdate(float time)
    {
        Debug.Log($"Node Update {time}s");
    }
}
