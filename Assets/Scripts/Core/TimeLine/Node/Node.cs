using UnityEngine;

namespace Core.TIMELINE
{
    public abstract class Node
    {
        public float startTime;
        public float duration;
        
        public abstract void OnEnter();
        public abstract void OnUpdate(float elapsedTime);
        public abstract void OnExit();
    }
}
