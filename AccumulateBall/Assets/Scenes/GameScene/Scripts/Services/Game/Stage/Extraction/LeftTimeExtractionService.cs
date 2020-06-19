using System;
using GameScene.Services.Content;
using GameScene.Services.Game.Parameters;

namespace GameScene.Services.Game
{
    public class LeftTimeExtractionService : BaseContentWithChangeablyParameterizedNonDefaultExtractionService<TimeSpan, LeftTimeChangeableExtractionParameter, TimeSpan>
    {
        protected override TimeSpan ExtractDefaultContent(TimeSpan constantDefaultExtractionParameter)
        {
            return constantDefaultExtractionParameter;
        }

        protected override TimeSpan ExtractNonDefaultContent(LeftTimeChangeableExtractionParameter nonDefaultExtractionParameter)
        {
            TimeSpan GetDecrement()
            {
                long decrementTicks = (long)(TimeSpan.TicksPerSecond * nonDefaultExtractionParameter.DecrementSeconds);

                return TimeSpan.FromTicks(decrementTicks);
            }

            TimeSpan decrementedTime = nonDefaultExtractionParameter.Time - GetDecrement();

            return (decrementedTime < TimeSpan.Zero) ? TimeSpan.Zero : decrementedTime;
        }
    }
}

namespace GameScene.Services.Game.Parameters
{
    public struct LeftTimeChangeableExtractionParameter
    {
        public LeftTimeChangeableExtractionParameter(TimeSpan time, float decrementSeconds)
        {
            Time = time;
            DecrementSeconds = decrementSeconds;
        }

        public float DecrementSeconds { get; private set; }

        public TimeSpan Time { get; private set; }
    }
}