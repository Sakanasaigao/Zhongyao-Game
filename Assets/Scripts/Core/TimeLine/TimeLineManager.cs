using System.Collections.Generic;
using UnityEngine;

namespace Core.TimeLine
{
    public class TimeLineManager : MonoBehaviour
    {
        public string timeLineDataFolder = "TimeLineData";
        private Dictionary<string, TimeLine> timeLines = new Dictionary<string, TimeLine>();
        
        public TimeLine LoadTimeLine(string timeLineName)
        {
            if (timeLines.ContainsKey(timeLineName))
            {
                return timeLines[timeLineName];
            }
            
            string path = $"{timeLineDataFolder}/{timeLineName}";
            TimeLineDataSO timeLineData = Resources.Load<TimeLineDataSO>(path);
            
            if (timeLineData != null)
            {
                TimeLine timeLine = new TimeLine();
                var nodes = new List<Node>();
                
                foreach (var nodeData in timeLineData.nodes)
                {
                    if (nodeData.nodePrefab != null)
                    {
                        var node = Object.Instantiate(nodeData.nodePrefab);
                        node.startTime = nodeData.startTime;
                        node.duration = nodeData.duration;
                        nodes.Add(node);
                    }
                }
                
                timeLine.Initialize(nodes);
                timeLines[timeLineName] = timeLine;
                return timeLine;
            }
            
            return null;
        }
        
        public void UnloadTimeLine(string timeLineName)
        {
            if (timeLines.TryGetValue(timeLineName, out var timeLine))
            {
                timeLine.Stop();
                timeLines.Remove(timeLineName);
            }
        }
        
        public TimeLine GetTimeLine(string timeLineName)
        {
            timeLines.TryGetValue(timeLineName, out var timeLine);
            return timeLine;
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
