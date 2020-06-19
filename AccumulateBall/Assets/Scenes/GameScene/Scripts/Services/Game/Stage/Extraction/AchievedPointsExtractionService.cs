using System;
using GameScene.Managers.Game.Enums;
using GameScene.Managers.Game.Settings;
using GameScene.Services.Content;
using GameScene.Services.Content.Parameters;
using GameScene.Services.Game.Parameters;

namespace GameScene.Services.Game
{
    public class AchievedPointsExtractionService : BaseContentWithNonParameterizedDefaultExtractionService<GamePointsSettings, AchievedPointsChangeableExtractionParameter, 
        int>
    {
        protected override int ExtractDefaultContent()
        {
            return 0;
        }

        protected override int ExtractNonDefaultContent(ContentNonDefaultExtractionParameters<GamePointsSettings,
            AchievedPointsChangeableExtractionParameter> nonDefaultExtractionParameter)
        {
            AchievedPointsChangeableExtractionParameter pointsChangeableExtractionParameter = nonDefaultExtractionParameter.Changeable;
            GamePointsSettings gamePointsSettings = nonDefaultExtractionParameter.Constant;
            int changedPoints = pointsChangeableExtractionParameter.Points + gamePointsSettings.AdditionSettings.GetSettings(pointsChangeableExtractionParameter.Type);

            return Math.Abs(changedPoints) <= gamePointsSettings.AbsoluteLimit ? changedPoints : pointsChangeableExtractionParameter.Points;
        }
    }
}

namespace GameScene.Services.Game.Parameters
{
    public struct AchievedPointsChangeableExtractionParameter
    {
        public AchievedPointsChangeableExtractionParameter(int points, GamePointsType type)
        {
            Points = points;
            Type = type;
        }

        public int Points { get; private set; }

        public GamePointsType Type { get; private set; }
    }
}