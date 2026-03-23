using System.Collections.Generic;

namespace Core.TIMELINE
{
    public interface ITimeLine
    {
        void Play();
        void Pause();
        void Stop();
        void Update(float deltaTime);
        float CurrentTime { get; }
        bool IsPlaying { get; }
        int NodeCount { get; }
        int ActiveNodeCount { get; }
    }
}
