using UnityEngine;

namespace Core.TimeLine
{
    public class TimeLineManager : MonoBehaviour
    {
        public TimeLineDataSO timeLineData;
        private TimeLine timeLine = new TimeLine();
        
        private void Start()
        {
            if (timeLineData != null)
            {
                var nodes = new System.Collections.Generic.List<Node>();
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
            }
        }
        
        private void Update()
        {
            timeLine.Update(Time.deltaTime);
        }
        
        public void Play()
        {
            timeLine.Play();
        }
        
        public void Pause()
        {
            timeLine.Pause();
        }
        
        public void Stop()
        {
            timeLine.Stop();
        }
        
        public void AddNode(Node node)
        {
            timeLine.AddNode(node);
        }
        
        public void RemoveNode(Node node)
        {
            timeLine.RemoveNode(node);
        }
        
        public float CurrentTime => timeLine.CurrentTime;
        public bool IsPlaying => timeLine.IsPlaying;
        public int NodeCount => timeLine.NodeCount;
        public int ActiveNodeCount => timeLine.ActiveNodeCount;
    }
}
