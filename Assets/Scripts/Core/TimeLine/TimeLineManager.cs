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
            string uniqueKey = GetKey(timeLineName, context);
            
            if (timeLines.ContainsKey(uniqueKey))
            {
                return (TimeLine<T>)timeLines[uniqueKey];
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
                timeLines[uniqueKey] = timeLine;
                return timeLine;
            }
            
            return null;
        }
        
        public void UnloadTimeLine<T>(string timeLineName, T context = default)
        {
            string uniqueKey = GetKey(timeLineName, context);
            if (timeLines.TryGetValue(uniqueKey, out var timeLine))
            {
                timeLine.Stop();
                timeLines.Remove(uniqueKey);
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
        
        public TimeLine<T> GetTimeLine<T>(string timeLineName, T context = default)
        {
            string uniqueKey = GetKey(timeLineName, context);
            timeLines.TryGetValue(uniqueKey, out var timeLine);
            return timeLine as TimeLine<T>;
        }
        
        public bool HasTimeLine<T>(string timeLineName, T context = default)
        {
            string uniqueKey = GetKey(timeLineName, context);
            return timeLines.ContainsKey(uniqueKey);
        }
        
        private string GetKey<T>(string timeLineName, T context)
        {
            string contextId = context != null ? context.GetHashCode().ToString() : "null";
            return $"{timeLineName}_{typeof(T).FullName}_{contextId}";
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
