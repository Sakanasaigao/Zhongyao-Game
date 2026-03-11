using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.TIMELINE
{
    public class TimeLine
    {
        private List<Node> nodes = new List<Node>();
        private float currentTime = 0f;
        private bool isPlaying = false;
        private List<Node> activeNodes = new List<Node>();
        
        public void Initialize(List<Node> initialNodes)
        {
            nodes = initialNodes.OrderBy(n => n.startTime).ToList();
            currentTime = 0f;
            isPlaying = false;
            activeNodes.Clear();
        }
        
        public void Play()
        {
            isPlaying = true;
        }
        
        public void Pause()
        {
            isPlaying = false;
        }
        
        public void Stop()
        {
            isPlaying = false;
            currentTime = 0f;
            foreach (var node in activeNodes)
            {
                node.OnExit();
            }
            activeNodes.Clear();
        }
        
        public void Update(float deltaTime)
        {
            if (!isPlaying)
                return;
            
            currentTime += deltaTime;
            
            CheckNewNodes();
            UpdateActiveNodes();
            CheckExitNodes();
        }
        
        private void CheckNewNodes()
        {
            var newNodes = nodes.Where(n => n.startTime <= currentTime && !activeNodes.Contains(n)).ToList();
            foreach (var node in newNodes)
            {
                node.OnEnter();
                activeNodes.Add(node);
            }
        }
        
        private void UpdateActiveNodes()
        {
            foreach (var node in activeNodes)
            {
                float elapsedTime = currentTime - node.startTime;
                node.OnUpdate(elapsedTime);
            }
        }
        
        private void CheckExitNodes()
        {
            var nodesToRemove = activeNodes.Where(n => currentTime >= n.startTime + n.duration).ToList();
            foreach (var node in nodesToRemove)
            {
                node.OnExit();
                activeNodes.Remove(node);
            }
        }
        
        public void AddNode(Node node)
        {
            nodes.Add(node);
            nodes = nodes.OrderBy(n => n.startTime).ToList();
        }
        
        public void RemoveNode(Node node)
        {
            if (activeNodes.Contains(node))
            {
                node.OnExit();
                activeNodes.Remove(node);
            }
            nodes.Remove(node);
        }
        
        public float CurrentTime => currentTime;
        public bool IsPlaying => isPlaying;
        public int NodeCount => nodes.Count;
        public int ActiveNodeCount => activeNodes.Count;
    }
}
