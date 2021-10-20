namespace Common
{
    public class RoundModel
    {
        public uint RoundCount { get; }

        public float TotalTime { get; }
        
        public float SafeTime { get; }

        public RoundModel(uint roundCount, float totalTime, float safeTime)
        {
            RoundCount = roundCount;
            TotalTime = totalTime;
            SafeTime = safeTime;
        }
    }
}