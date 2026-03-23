using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.TIMELINE
{
    public class TimeLineManager : MonoBehaviour
    {
        [Inject] private DiContainer container;
        public string timeLineDataFolder = "TimeLineData";
        private Dictionary<string, ITimeLine> timeLines = new Dictionary<string, ITimeLine>();
        
        public TimeLine<T> LoadTimeLine<T>(string timeLineName, T context = default)
        {
            string key = $"{timeLineName}_{typeof(T).FullName}";
            if (timeLines.ContainsKey(key))
            {
                return (TimeLine<T>)timeLines[key];
            }
            
            string path = $"{timeLineDataFolder}/{timeLineName}";
            TimeLineDataSO timeLineData = Resources.Load<TimeLineDataSO>(path);
            
            if (timeLineData != null)
            {
                TimeLine<T> timeLine = new TimeLine<T>();
                timeLine.Context = context;
                var nodes = new List<Node>();
                
                foreach (var nodeData in timeLineData.nodes)
                {
                    if (!string.IsNullOrEmpty(nodeData.nodeTypeName))
                    {
                        var nodeType = System.Type.GetType(nodeData.nodeTypeName);
                        if (nodeType != null)
                        {
                            var node = System.Activator.CreateInstance(nodeType) as Node;
                            if (node != null)
                            {
                                node.startTime = nodeData.startTime;
                                node.duration = nodeData.duration;
                                if (node is Node<T> typedNode)
                                {
                                    typedNode.Context = context;
                                }
                                container.Inject(node);
                                nodes.Add(node);
                            }
                        }
                    }
                }
                
                timeLine.Initialize(nodes);
                timeLines[key] = timeLine;
                return timeLine;
            }
            
            return null;
        }
        
        public void UnloadTimeLine<T>(string timeLineName)
        {
            string key = $"{timeLineName}_{typeof(T).FullName}";
            if (timeLines.TryGetValue(key, out var timeLine))
            {
                timeLine.Stop();
                timeLines.Remove(key);
            }
        }
        
        public void UnloadTimeLine(ITimeLine timeLine)
        {
            string keyToRemove = null;
            foreach (var kv in timeLines)
            {
                if (kv.Value == timeLine)
                {
                    keyToRemove = kv.Key;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(keyToRemove))
            {
                timeLine.Stop();
                timeLines.Remove(keyToRemove);
            }
        }
        
        public TimeLine<T> GetTimeLine<T>(string timeLineName)
        {
            string key = $"{timeLineName}_{typeof(T).FullName}";
            timeLines.TryGetValue(key, out var timeLine);
            return timeLine as TimeLine<T>;
        }
        
        private void Update()
        {
            foreach (var timeLine in timeLines.Values)
            {
                timeLine.Update(Time.deltaTime);
            }
        }
        
        public void PlayAll()
        {
            foreach (var timeLine in timeLines.Values)
            {
                timeLine.Play();
            }
        }
        
        public void PauseAll()
        {
            foreach (var timeLine in timeLines.Values)
            {
                timeLine.Pause();
            }
        }
        
        public void StopAll()
        {
            foreach (var timeLine in timeLines.Values)
            {
                timeLine.Stop();
            }
        }
        
        public int TimeLineCount => timeLines.Count;
    }
}
