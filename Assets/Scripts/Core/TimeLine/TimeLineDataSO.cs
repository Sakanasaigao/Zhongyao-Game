using System.Collections.Generic;
using UnityEngine;

namespace Core.TIMELINE
{
    [CreateAssetMenu(fileName = "TimeLineData", menuName = "Core/TIMELINE/TimeLineData")]
    public class TimeLineDataSO : ScriptableObject
    {
        public List<NodeData> nodes = new List<NodeData>();
    }
    
    [System.Serializable]
    public class NodeData
    {
        public string nodeName;
        public float startTime;
        public float duration;
        public string nodeTypeName;
    }
}
