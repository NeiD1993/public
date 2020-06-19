using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Managers.Game.Enums;
using GameScene.Managers.Game.Settings;
using GameScene.Services.Content.Characteristics;
using GameScene.Services.Content.Data;
using GameScene.Services.Field;
using GameScene.Services.Game.Actions;
using GameScene.Services.Game.Actions.Enums;
using GameScene.Services.Game.Characteristics;
using GameScene.Services.Game.Characteristics.Details;
using GameScene.Services.Game.Characteristics.Interfaces;
using GameScene.Services.Game.ConductingTask;
using GameScene.Services.Game.ConductingTask.Enums;
using GameScene.Services.Game.Data;
using GameScene.Services.Game.Enums;
using GameScene.Services.Game.Events;
using GameScene.Services.Game.Info;
using GameScene.Services.Game.Metrics;
using GameScene.Services.Game.Parameters;
using GameScene.Services.Game.Units;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Services.Game
{
    public partial class GameLogicService : IDisposable
    {
        private const int utmostRemainedStepsCountToCheckStage = 1;

        private readonly FieldEntityBindingService fieldEntityBindingService;

        private readonly GameStageDurationEvaluator gameStageDurationEvaluator;

        private readonly GameStageContentUnits<AchievedPointsExtractionService, LeftTimeExtractionService, RemainedStepsExtractionService> gamesStageContentExtractionServices;

        private readonly PathToReservoirGenerator pathToReservoirGenerator;

        public GameLogicService()
        {
            fieldEntityBindingService = SharedSceneServicesLocator.GetService<FieldEntityBindingService>();
            gameStageDurationEvaluator = new GameStageDurationEvaluator();
            gamesStageContentExtractionServices = new GameStageContentUnits<AchievedPointsExtractionService, LeftTimeExtractionService,
                RemainedStepsExtractionService>(new AchievedPointsExtractionService(), new LeftTimeExtractionService(), new RemainedStepsExtractionService());
            pathToReservoirGenerator = new PathToReservoirGenerator();
            GameStageDataGenerated = new GameStageDataGeneratedEvent();
        }

        public GameStageCreatedInfo GameStageCreatedInfo { get; set; }

        public GameStageDataGeneratedEvent GameStageDataGenerated { get; private set; }

        private void CreateGameStageInfo(GameStageSpecifications specifications, GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActions<GameStageConductingTask>,
            GameStageCreatedInfoElementSetupActions<GameStageMetrics>> setupActions)
        {
            GameStageMetrics CreateGameStageMetrics()
            {
                AchievedPointsCharacteristics achievedPointsCharacteristics = (specifications.PointsSettings != null) ?
                    new AchievedPointsCharacteristics(gamesStageContentExtractionServices.AchievedPoints
                    .EstablishContentExtractionData(specifications.PointsSettings.Value)) : GameStageCreatedInfo.Metrics.AchievedPoints;
                LeftTimeCharacteristics leftTimeCharacteristics = new LeftTimeCharacteristics(gamesStageContentExtractionServices.LeftTime
                    .EstablishContentExtractionData(specifications.Duration));
                RemainedStepsCharacteristics remainedStepsCharacteristics = new RemainedStepsCharacteristics(gamesStageContentExtractionServices.RemainedSteps
                    .EstablishContentExtractionData(specifications.PathLength));

                return new GameStageMetrics(achievedPointsCharacteristics, leftTimeCharacteristics, remainedStepsCharacteristics);
            }

            GameStageMetrics metrics;

            GameStageCreatedInfo?.Dispose();
            GameStageCreatedInfo = new GameStageCreatedInfo(new GameStageConductingTask(metrics = CreateGameStageMetrics()), metrics, setupActions);
        }

        public void Dispose()
        {
            GameStageCreatedInfo?.Dispose();
        }

        public void RecordGameStage(GamePointsType achievedPointsType)
        {
            GameStageMetrics gameStageMetrics = GameStageCreatedInfo.Metrics;

            if (((achievedPointsType == GamePointsType.ExtraStepCompletion) && !gameStageMetrics.RemainedSteps.TryRefresh()) || 
                (achievedPointsType != GamePointsType.ExtraStepCompletion))
                gameStageMetrics.AchievedPoints.TryRefresh(achievedPointsType);
        }

        public bool IsGameRoundEnded(ISet<CardinalPoint> ballUnfilledCardinalPoints)
        {
            return ballUnfilledCardinalPoints.Count == 0;
        }

        public bool IsGameStageEnded(CardinalPoint ballCheckableCardinalPoint, SubstanceColorType reservoirSubstanceColorType)
        {
            return reservoirSubstanceColorType == fieldEntityBindingService.GetSubstanceColorTypeByCardinalPoint(ballCheckableCardinalPoint);
        }

        public IEnumerator GenerateGameStageDataIteratively(GameStageGenerativeInfo generativeInfo, GameStageSettings settings,
            GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActions<GameStageConductingTask>,
                GameStageCreatedInfoElementSetupActions<GameStageMetrics>> setupActions)
        {
            IEnumerator GeneratePathToReservoirDataIteratively(Action<PathToReservoirData> pathToReservoirDataExtractor)
            {
                yield return pathToReservoirGenerator.GeneratePathIteratively(generativeInfo.PathToReservoirGenerativeInfo, settings.Restrictions.PathToReservoirSettings, 
                    pathToReservoirDataExtractor);
            }

            GameStageSpecifications CreateGameStageSpecifications(int pathToReservoirLength)
            {
                TimeSpan stageDuration = gameStageDurationEvaluator.EvaluateStageDuration(pathToReservoirLength, settings.Restrictions.StepDuration);

                return new GameStageSpecifications(pathToReservoirLength, stageDuration, (GameStageCreatedInfo == null) ? new GamePointsSettings?(settings.Points) : null);
            }

            Func<bool> CreateGameStageCheckingConditionFunction()
            {
                return () => GameStageCreatedInfo.Metrics.RemainedSteps.Content <= utmostRemainedStepsCountToCheckStage;
            }

            GameStageContentUnits<int, TimeSpan, int> CreateExtractableGameStageData()
            {
                GameStageMetrics stageMetrics = GameStageCreatedInfo.Metrics;

                return new GameStageContentUnits<int, TimeSpan, int>(stageMetrics.AchievedPoints.Content, stageMetrics.LeftTime.Content, stageMetrics.RemainedSteps.Content);
            }

            PathToReservoirData pathToReservoirData = default;

            yield return GeneratePathToReservoirDataIteratively(pathToReservoirDataParameter => pathToReservoirData = pathToReservoirDataParameter);

            CreateGameStageInfo(CreateGameStageSpecifications(pathToReservoirData.Length), setupActions);
            GameStageDataGenerated.Invoke(new GameStageData(pathToReservoirData, CreateGameStageCheckingConditionFunction(), CreateExtractableGameStageData()));
        }

        private struct GameStageSpecifications
        {
            public GameStageSpecifications(int pathLength, TimeSpan duration, GamePointsSettings? pointsSettings)
            {
                PathLength = pathLength;
                Duration = duration;
                PointsSettings = pointsSettings;
            }

            public int PathLength { get; private set; }

            public TimeSpan Duration { get; private set; }

            public GamePointsSettings? PointsSettings { get; private set; }
        }
    }
}

namespace GameScene.Services.Game.Actions
{
    public struct GameStageCreatedInfoElementSetupActions<T> where T : class
    {
        public GameStageCreatedInfoElementSetupActions(Action<T> creating, Action<T> disposing)
        {
            Creating = creating;
            Disposing = disposing;
        }

        public Action<T> Creating { get; private set; }

        public Action<T> Disposing { get; private set; }
    }
}

namespace GameScene.Services.Game.Actions.Enums
{
    public enum GameStageCreatedInfoElementSetupActionType
    {
        Creating,

        Disposing
    }
}

namespace GameScene.Services.Game.Actions.Info
{
    public struct GameStageCreatedInfoElementSetupActionInfo
    {
        public GameStageCreatedInfoElementSetupActionInfo(GameStageCreatedInfoElementSetupActionType type, Action action)
        {
            Type = type;
            Action = action;
        }

        public GameStageCreatedInfoElementSetupActionType Type { get; private set; }

        public Action Action { get; private set; }
    }
}

namespace GameScene.Services.Game.Characteristics
{
    public abstract class BaseExtractableGameStageContentWithNonParameterizedRefreshingCharacteristics<T1> :
        BaseExtractableContentWithNonParameterizedRefreshingCharacteristics<T1, T1>, IDetailedExtractableGameStageContentCharacteristics where T1 : struct
    {
        public BaseExtractableGameStageContentWithNonParameterizedRefreshingCharacteristics(ContentExtractionData<T1, T1> contentExtractionData) :
            base(contentExtractionData)
        { }

        protected abstract GameStageContentUnitGenus GetGenus();

        protected override T1 GetChangeableExtractionParameter()
        {
            return Content;
        }

        public GameStageContentCharacteristicsDetails GetDetails()
        {
            return new GameStageContentCharacteristicsDetails(GetGenus(), Content);
        }
    }

    public abstract class BaseExtractableGameStageContentWithParameterizedRefreshingCharacteristics<T1, T2, T3> :
        BaseExtractableContentWithParameterizedNonIterativeRefreshingCharacteristics<T1, T2, T3>, IDetailedExtractableGameStageContentCharacteristics where T1 : struct
        where T2 : struct where T3 : struct
    {
        public BaseExtractableGameStageContentWithParameterizedRefreshingCharacteristics(ContentExtractionData<T1, T3> contentExtractionData) :
            base(contentExtractionData)
        { }

        protected abstract GameStageContentUnitGenus GetGenus();

        public GameStageContentCharacteristicsDetails GetDetails()
        {
            return new GameStageContentCharacteristicsDetails(GetGenus(), Content);
        }
    }

    public class AchievedPointsCharacteristics : BaseExtractableGameStageContentWithParameterizedRefreshingCharacteristics<AchievedPointsChangeableExtractionParameter,
        GamePointsType, int>
    {
        public AchievedPointsCharacteristics(ContentExtractionData<AchievedPointsChangeableExtractionParameter, int> contentExtractionData) : base(contentExtractionData) { }

        protected override GameStageContentUnitGenus GetGenus()
        {
            return GameStageContentUnitGenus.AchievedPoints;
        }

        protected override AchievedPointsChangeableExtractionParameter GetChangeableExtractionParameter(GamePointsType refreshingParameter)
        {
            return new AchievedPointsChangeableExtractionParameter(Content, refreshingParameter);
        }
    }

    public class LeftTimeCharacteristics : BaseExtractableGameStageContentWithParameterizedRefreshingCharacteristics<LeftTimeChangeableExtractionParameter, float, TimeSpan>, 
        IValidableGameStageContentCharacteristics
    {
        public LeftTimeCharacteristics(ContentExtractionData<LeftTimeChangeableExtractionParameter, TimeSpan> contentExtractionData) : base(contentExtractionData)
        {
            AreValid = true;
        }

        public bool AreValid { get; private set; }

        protected override GameStageContentUnitGenus GetGenus()
        {
            return GameStageContentUnitGenus.LeftTime;
        }

        protected override LeftTimeChangeableExtractionParameter GetChangeableExtractionParameter(float refreshingParameter)
        {
            return new LeftTimeChangeableExtractionParameter(Content, refreshingParameter);
        }

        public override bool TryRefresh(float refreshingParameter)
        {
            bool refreshingResult = base.TryRefresh(refreshingParameter);

            if (!refreshingResult)
                AreValid = false;

            return refreshingResult;
        }
    }

    public class RemainedStepsCharacteristics : BaseExtractableGameStageContentWithNonParameterizedRefreshingCharacteristics<int>
    {
        public RemainedStepsCharacteristics(ContentExtractionData<int, int> contentExtractionData) : base(contentExtractionData) { }

        protected override GameStageContentUnitGenus GetGenus()
        {
            return GameStageContentUnitGenus.RemainedSteps;
        }
    }
}

namespace GameScene.Services.Game.Characteristics.Details
{
    public struct GameStageContentCharacteristicsDetails
    {
        public GameStageContentCharacteristicsDetails(GameStageContentUnitGenus genus, object content)
        {
            Genus = genus;
            Content = content;
        }

        public GameStageContentUnitGenus Genus { get; private set; }

        public object Content { get; private set; }
    }
}

namespace GameScene.Services.Game.Characteristics.Interfaces
{
    public interface IDetailedExtractableGameStageContentCharacteristics
    {
        GameStageContentCharacteristicsDetails GetDetails();
    }

    public interface IValidableGameStageContentCharacteristics
    {
        bool AreValid { get; }
    }
}

namespace GameScene.Services.Game.ConductingTask
{
    public class GameStageConductingTask
    {
        public GameStageConductingTask(GameStageMetrics metrics)
        {
            SuppressionStatus = GameStageConductingTaskSuppressionStatus.None;
            Routine = ConductIteratively(metrics);
            Done = new UnityEvent();
        }

        public GameStageConductingTaskSuppressionStatus SuppressionStatus { get; set; }

        public IEnumerator Routine { get; private set; }

        public UnityEvent Done { get; private set; }

        private IEnumerator ConductIteratively(GameStageMetrics metrics)
        {
            SuppressionStatus = GameStageConductingTaskSuppressionStatus.NotSuppressed;

            while (metrics.AreValid)
            {
                yield return new WaitUntil(() => SuppressionStatus == GameStageConductingTaskSuppressionStatus.NotSuppressed);

                metrics.LeftTime.TryRefresh(Time.deltaTime);
            };

            SuppressionStatus = GameStageConductingTaskSuppressionStatus.None;
            Done.Invoke();
        }
    }
}

namespace GameScene.Services.Game.ConductingTask.Enums
{
    public enum GameStageConductingTaskSuppressionStatus
    {
        None,

        NotSuppressed,

        Suppressed
    }
}

namespace GameScene.Services.Game.Data
{
    public struct GameStageData
    {
        public GameStageData(PathToReservoirData pathToReservoir, Func<bool> checkingConditionFunction, GameStageContentUnits<int, TimeSpan, int> extractable)
        {
            Extractable = extractable;
            CheckingConditionFunction = checkingConditionFunction;
            PathToReservoir = pathToReservoir;
        }

        public PathToReservoirData PathToReservoir { get; private set; }

        public Func<bool> CheckingConditionFunction { get; private set; }

        public GameStageContentUnits<int, TimeSpan, int> Extractable { get; private set; }
    }
}

namespace GameScene.Services.Game.Enums
{
    public enum GameStageContentUnitGenus
    {
        AchievedPoints,

        LeftTime,

        RemainedSteps
    }
}

namespace GameScene.Services.Game.Events
{
    public class GameStageDataGeneratedEvent : UnityEvent<GameStageData> { }
}

namespace GameScene.Services.Game.Info
{
    public struct GameStageGenerativeInfo
    {
        public GameStageGenerativeInfo(PathToReservoirGenerativeInfo pathToReservoirGenerativeInfo)
        {
            PathToReservoirGenerativeInfo = pathToReservoirGenerativeInfo;
        }

        public PathToReservoirGenerativeInfo PathToReservoirGenerativeInfo { get; private set; }
    }

    public class GameStageCreatedInfo : GameStageCreatedInfoUnits<GameStageConductingTask, GameStageMetrics>, IDisposable
    {
        private readonly GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageConductingTask>,
            GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageMetrics>> setupActionsAccomplishers;

        public GameStageCreatedInfo(GameStageConductingTask conductingTask, GameStageMetrics metrics,
            GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActions<GameStageConductingTask>,
                GameStageCreatedInfoElementSetupActions<GameStageMetrics>> setupActions) : base(conductingTask, metrics)
        {
            setupActionsAccomplishers = new GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageConductingTask>,
                GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageMetrics>>(
                new GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageConductingTask>(conductingTask, setupActions.ConductingTask),
                new GameStageCreatedInfoElementSetupActionsAccomplisher<GameStageMetrics>(metrics, setupActions.Metrics));
            setupActionsAccomplishers.ConductingTask.Accomplish(GameStageCreatedInfoElementSetupActionType.Creating);
            setupActionsAccomplishers.Metrics.Accomplish(GameStageCreatedInfoElementSetupActionType.Creating);
        }

        public void Dispose()
        {
            setupActionsAccomplishers.ConductingTask.Accomplish(GameStageCreatedInfoElementSetupActionType.Disposing);
            setupActionsAccomplishers.Metrics.Accomplish(GameStageCreatedInfoElementSetupActionType.Disposing);
        }
    }
}

namespace GameScene.Services.Game.Metrics
{
    public class GameStageMetrics : GameStageContentUnits<AchievedPointsCharacteristics, LeftTimeCharacteristics, RemainedStepsCharacteristics>,
        IValidableGameStageContentCharacteristics
    {
        public GameStageMetrics(AchievedPointsCharacteristics achievedPoints, LeftTimeCharacteristics elapsedTime, RemainedStepsCharacteristics remainedSteps) :
            base(achievedPoints, elapsedTime, remainedSteps)
        { }

        public bool AreValid
        {
            get
            {
                return LeftTime.AreValid;
            }
        }
    }
}

namespace GameScene.Services.Game.Units
{
    public class GameStageContentUnits<T1, T2, T3>
    {
        public GameStageContentUnits(T1 achievedPoints, T2 leftTime, T3 remainedSteps)
        {
            AchievedPoints = achievedPoints;
            LeftTime = leftTime;
            RemainedSteps = remainedSteps;
        }

        public T1 AchievedPoints { get; protected set; }

        public T2 LeftTime { get; protected set; }

        public T3 RemainedSteps { get; protected set; }
    }

    public class GameStageCreatedInfoUnits<T1, T2>
    {
        public GameStageCreatedInfoUnits(T1 conductingTask, T2 metrics)
        {
            Metrics = metrics;
            ConductingTask = conductingTask;
        }

        public T1 ConductingTask { get; private set; }

        public T2 Metrics { get; private set; }
    }
}