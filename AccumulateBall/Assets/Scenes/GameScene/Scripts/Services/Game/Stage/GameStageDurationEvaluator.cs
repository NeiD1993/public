using System;
using GameScene.Managers.Game.Settings;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private class GameStageDurationEvaluator
        {
            private static TimeSpan GetStepDuration(GameStageStepDurationSettings stepDurationSettings)
            {
                return new TimeSpan(0, stepDurationSettings.Minutes, stepDurationSettings.Seconds);
            }

            public TimeSpan EvaluateStageDuration(int pathLength, GameStageStepDurationSettings stepDurationSettings)
            {
                TimeSpan stepDuration = GetStepDuration(stepDurationSettings);

                return TimeSpan.FromTicks(pathLength * stepDuration.Ticks);
            }
        }
    }
}