using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Managers.ControlPanel;
using GameScene.Managers.ControlPanel.Enums;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Game.Enums;
using GameScene.Managers.Game.Settings;
using GameScene.Managers.Game.Settings.Interfaces;
using GameScene.Managers.Light;
using GameScene.Services.Game;
using GameScene.Services.Game.Actions;
using GameScene.Services.Game.Characteristics.Interfaces;
using GameScene.Services.Game.ConductingTask;
using GameScene.Services.Game.ConductingTask.Enums;
using GameScene.Services.Game.Data;
using GameScene.Services.Game.Info;
using GameScene.Services.Game.Metrics;
using GameScene.Services.Game.Units;
using GameScene.Services.Managers;
using Loaders;
using Loaders.Enums;
using ServicesLocators;
using UnityEngine;

namespace GameScene.Managers.Game
{
    public class GameManager : BaseManager
    {
        private readonly GameStatesData gameStatesData;

        [SerializeField]
        private GameStageSettings gameStageSettings;

        private EntityManagersAccessor entityManagersAccessor;

        private GameLogicService gameLogicService;

        public GameManager()
        {
            gameStatesData = new GameStatesData();
        }

        private ControlPanelManager ControlPanelManager { get; set; }

        private FieldManager FieldManager { get; set; }

        private LightsManager LightsManager { get; set; }

        private void BeginGamePreInitialization()
        {
            StartCoroutine(LightsManager.SpawnLightsIteratively(FieldManager.Dimension));
            StartCoroutine(FieldManager.BeginFieldPreInitializationIteratively());
        }

        private void BeginGameStageGenerating()
        {
            void AddGameStageConductingTaskEventsListeners(GameStageConductingTask gameStageConductingTask)
            {
                gameStageConductingTask.Done.AddListener(OnGameStageConductingTaskDone);
            }

            void RemoveGameStageConductingTaskEventsListeners(GameStageConductingTask gameStageConductingTask)
            {
                gameStageConductingTask.Done.RemoveListener(OnGameStageConductingTaskDone);
            }

            void AddGameStageMetricsEventsListeners(GameStageMetrics gameStageMetrics)
            {
                eventsListenersService.AddOrdinaryEventListener(() => OnGameStageCharacteristicRefreshed(gameStageMetrics.AchievedPoints),
                    gameStageMetrics.AchievedPoints.Refreshed);
                eventsListenersService.AddOrdinaryEventListener(() => OnGameStageCharacteristicRefreshed(gameStageMetrics.LeftTime),
                    gameStageMetrics.LeftTime.Refreshed);
                eventsListenersService.AddOrdinaryEventListener(() => OnGameStageCharacteristicRefreshed(gameStageMetrics.RemainedSteps),
                    gameStageMetrics.RemainedSteps.Refreshed);
            }

            void RemoveGameStageMetricsEventsListeners(GameStageMetrics gameStageMetrics)
            {
                eventsListenersService.RemoveOrdinaryEventListener(gameStageMetrics.AchievedPoints.Refreshed);
                eventsListenersService.RemoveOrdinaryEventListener(gameStageMetrics.LeftTime.Refreshed);
                eventsListenersService.RemoveOrdinaryEventListener(gameStageMetrics.RemainedSteps.Refreshed);
            }

            GameStageGenerativeInfo gameStageGenerativeInfo = new GameStageGenerativeInfo(FieldManager.CreateFieldInitializationData());

            StartCoroutine(gameLogicService.GenerateGameStageDataIteratively(gameStageGenerativeInfo, gameStageSettings,
                new GameStageCreatedInfoUnits<GameStageCreatedInfoElementSetupActions<GameStageConductingTask>,
                GameStageCreatedInfoElementSetupActions<GameStageMetrics>>(
                    new GameStageCreatedInfoElementSetupActions<GameStageConductingTask>(AddGameStageConductingTaskEventsListeners,
                    RemoveGameStageConductingTaskEventsListeners),
                    new GameStageCreatedInfoElementSetupActions<GameStageMetrics>(AddGameStageMetricsEventsListeners, RemoveGameStageMetricsEventsListeners))));
        }

        private void ChangeGameStageConductingMode(GameStageConductingMode mode)
        {
            if (gameStatesData.Completion == GameCompletionState.Uncompleted)
            {
                GameStageConductingTask gameStageConductingTask = gameLogicService.GameStageCreatedInfo.ConductingTask;

                switch (mode)
                {
                    case GameStageConductingMode.Off:
                        gameStageConductingTask.SuppressionStatus = GameStageConductingTaskSuppressionStatus.Suppressed;
                        break;
                    case GameStageConductingMode.On:
                        {
                            if (gameStageConductingTask.SuppressionStatus == GameStageConductingTaskSuppressionStatus.None)
                                StartCoroutine(gameStageConductingTask.Routine);
                            else
                                gameStageConductingTask.SuppressionStatus = GameStageConductingTaskSuppressionStatus.NotSuppressed;
                        }
                        break;
                    default:
                        StopCoroutine(gameStageConductingTask.Routine);
                        break;
                }
            }
        }

        private void ObtainChildGameManagers()
        {
            ControlPanelManager = entityManagersAccessor.GetManager<ControlPanelManager>();
            FieldManager = entityManagersAccessor.GetManager<FieldManager>();
            LightsManager = entityManagersAccessor.GetManager<LightsManager>();
        }

        private void SetupChildGameManagers()
        {
            SetupControlPanelManager();
            SetupFieldManager();
        }

        private void SetupControlPanelManager()
        {
            ControlPanelManager.Destroyed.AddListener(OnControlPanelManagerDestroyed);
            ControlPanelManager.EntityActivated.AddListener(OnControlPanelActivated);
            ControlPanelManager.EntityEnabled.AddListener(OnControlPanelEnabled);
            ControlPanelManager.EntityObjectsDestroyed.AddListener(OnControlPanelEntityObjectsDestroyed);
            ControlPanelManager.EntityRunned.AddListener(OnControlPanelRunned);
            ControlPanelManager.EntityWaitForDisablingEvent.AddListener(OnControlPanelWaitForDisabling);
        }

        private void SetupFieldManager()
        {
            FieldManager.Destroyed.AddListener(OnFieldManagerDestroyed);
            FieldManager.EntityPrimalStatusChanged.AddListener(OnFieldEntityPrimalStatusChanged);
            FieldManager.EntityBanned.AddListener(OnFieldBanned);
            FieldManager.EntityPreBanned.AddListener(OnFieldPreBanned);
            FieldManager.FieldInitialized.AddListener(OnFieldInitialized);
            FieldManager.FieldFinalized.AddListener(OnFieldFinalized);
            FieldManager.FieldPreCompletelyFinalized.AddListener(OnFieldPreCompletelyFinalized);
            FieldManager.FieldPreInitialized.AddListener(OnFieldPreInitialized);
            FieldManager.FieldPreMoved.AddListener(OnFieldPreMoved);
            FieldManager.EntityAccepted.AddListener(OnFieldAccepted);
            FieldManager.FieldPreChecked.AddListener(OnFieldPreChecked);
        }

        private void SetupGameLogicService()
        {
            gameLogicService = new GameLogicService();
            gameLogicService.GameStageDataGenerated.AddListener(OnGameStageDataGenerated);
        }

        private void ShutdownGameStage(GameCompletionState completionState)
        {
            ChangeGameStageConductingMode(GameStageConductingMode.Shutdown);
            gameStatesData.Completion = completionState;
        }

        private IEnumerator FinishGameStateFailingIteratively()
        {
            yield return ControlPanelManager.CalibrateEntityIteratively();

            gameLogicService.RecordGameStage(GamePointsType.StageFailing);
            FieldManager.ProceedFieldInterruptingFinalization();
        }

        protected override void PerpetrateStartProcessing()
        {
            SetupChildGameManagers();
            BeginGamePreInitialization();
            base.PerpetrateStartProcessing();
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            entityManagersAccessor = SharedSceneServicesLocator.GetService<EntityManagersAccessor>();
        }

        protected override void PreProcessStart()
        {
            base.PreProcessStart();
            SetupGameLogicService();
            ObtainChildGameManagers();
        }

        protected override Func<bool> GetCanConfigureConditionFunction()
        {
            return () => (ControlPanelManager?.IsConfigurated).Value && (FieldManager?.IsConfigurated).Value && (LightsManager?.IsConfigurated).Value;
        }

        private void OnControlPanelActivated()
        {
            BeginGameStageGenerating();
        }

        private void OnControlPanelManagerDestroyed()
        {
            ControlPanelManager.Destroyed.RemoveListener(OnControlPanelManagerDestroyed);
            ControlPanelManager.EntityActivated.RemoveListener(OnControlPanelActivated);
            ControlPanelManager.EntityEnabled.RemoveListener(OnControlPanelEnabled);
            ControlPanelManager.EntityObjectsDestroyed.RemoveListener(OnControlPanelEntityObjectsDestroyed);
            ControlPanelManager.EntityRunned.RemoveListener(OnControlPanelRunned);
            ControlPanelManager.EntityWaitForDisablingEvent.RemoveListener(OnControlPanelWaitForDisabling);
        }

        private void OnControlPanelEnabled()
        {
            FieldManager.ResumeEntity();
            ChangeGameStageConductingMode(GameStageConductingMode.On);
            gameStatesData.Running = GameRunningState.Runned;
        }

        private void OnControlPanelEntityObjectsDestroyed()
        {
            FieldManager.ProceedFieldInterruptingFinalization();
        }

        private void OnControlPanelRunned()
        {
            IEnumerator ProcessControlPanelRunnedIteratively()
            {
                IEnumerator fieldActuatingRoutine;

                switch (gameStatesData.Running)
                {
                    case GameRunningState.None:
                        fieldActuatingRoutine = FieldManager.RunEntityIteratively();
                        break;
                    default:
                        fieldActuatingRoutine = FieldManager.ContinueFieldIteratively();
                        break;
                }

                yield return fieldActuatingRoutine;

                gameStatesData.Completion = GameCompletionState.Uncompleted;
                ChangeGameStageConductingMode(GameStageConductingMode.On);
                ControlPanelManager.IntensifyEntity();
                gameStatesData.Running = GameRunningState.Runned;
            }

            StartCoroutine(ProcessControlPanelRunnedIteratively());
        }

        private void OnControlPanelWaitForDisabling(ControlPanelDisablingType controlPanelDisablingType)
        {
            IEnumerator ProcessControlPanelWaitForDisablingIteratively()
            {
                FieldNonSequentalActionType nonSequentalFieldActionType;
                bool nonSequentalFieldActionInitiationResult = default;

                switch (controlPanelDisablingType)
                {
                    case ControlPanelDisablingType.WithButtonsDestroying:
                        nonSequentalFieldActionType = FieldNonSequentalActionType.CompleteFinalization;
                        break;
                    default:
                        nonSequentalFieldActionType = FieldNonSequentalActionType.Suspending;
                        break;
                }

                yield return FieldManager.InitiateFieldNonSequentalActionIteratively(nonSequentalFieldActionType,
                    nonSequentalFieldActionExecutionResultParameter => nonSequentalFieldActionInitiationResult = nonSequentalFieldActionExecutionResultParameter);

                if (nonSequentalFieldActionInitiationResult)
                {
                    if (nonSequentalFieldActionType == FieldNonSequentalActionType.CompleteFinalization)
                        ShutdownGameStage(GameCompletionState.Aborted);
                    else
                        ChangeGameStageConductingMode(GameStageConductingMode.Off);

                    ControlPanelManager.PerformEntityDisabling();
                    gameStatesData.Running = GameRunningState.NotRunned;
                }
                else
                    ControlPanelManager.CancelEntityDisabling();
            }

            StartCoroutine(ProcessControlPanelWaitForDisablingIteratively());
        }

        private void OnDestroy()
        {
            gameLogicService.GameStageDataGenerated.RemoveListener(OnGameStageDataGenerated);
            gameLogicService.Dispose();
        }

        private void OnFieldBanned()
        {
            StartCoroutine(FinishGameStateFailingIteratively());
        }

        private void OnFieldEntityPrimalStatusChanged(FieldEntityPrimalStatusData fieldEntityPrimalStatus)
        {
            if (fieldEntityPrimalStatus.Availability == FieldEntityAvailabilityStatus.Unavailable)
                ControlPanelManager.ChangeEntityStatus(ControlPanelStatus.CompletelyDisabled);
            else
            {
                if (fieldEntityPrimalStatus.Finalization == FieldEntityFinalizationStatus.NotReadyForFinalization)
                    ControlPanelManager.ChangeEntityStatus(ControlPanelStatus.DisabledForFinalization);
                else
                    ControlPanelManager.ChangeEntityStatus(ControlPanelStatus.Enabled);
            }
        }

        private void OnFieldInitialized()
        {
            ControlPanelRunningType controlPanelRunningType;

            switch (gameStatesData.Running)
            {
                case GameRunningState.None:
                    controlPanelRunningType = ControlPanelRunningType.Wholly;
                    break;
                default:
                    controlPanelRunningType = ControlPanelRunningType.WithoutButtons;
                    break;
            }

            StartCoroutine(ControlPanelManager.BeginControlPanelRunningIteratively(controlPanelRunningType));
        }

        private void OnFieldFinalized(FieldFinalizationType fieldFinalizationType)
        {
            if (fieldFinalizationType == FieldFinalizationType.Interval)
                gameLogicService.RecordGameStage(GamePointsType.RoundEnding);

            if (fieldFinalizationType == FieldFinalizationType.Complete)
                SceneLoader.LoadScene(SceneKind.MenuScene);
            else
                BeginGameStageGenerating();
        }

        private void OnFieldManagerDestroyed()
        {
            FieldManager.Destroyed.RemoveListener(OnFieldManagerDestroyed);
            FieldManager.EntityPrimalStatusChanged.RemoveListener(OnFieldEntityPrimalStatusChanged);
            FieldManager.EntityBanned.RemoveListener(OnFieldBanned);
            FieldManager.EntityPreBanned.RemoveListener(OnFieldPreBanned);
            FieldManager.FieldInitialized.RemoveListener(OnFieldInitialized);
            FieldManager.FieldFinalized.RemoveListener(OnFieldFinalized);
            FieldManager.FieldPreCompletelyFinalized.RemoveListener(OnFieldPreCompletelyFinalized);
            FieldManager.FieldPreInitialized.RemoveListener(OnFieldPreInitialized);
            FieldManager.FieldPreMoved.RemoveListener(OnFieldPreMoved);
            FieldManager.EntityAccepted.RemoveListener(OnFieldAccepted);
            FieldManager.FieldPreChecked.RemoveListener(OnFieldPreChecked);
        }

        private void OnFieldPreBanned()
        {
            ShutdownGameStage(GameCompletionState.Aborted);
        }

        private void OnFieldPreCompletelyFinalized()
        {
            StartCoroutine(LightsManager.DespawnLightsIteratively());
        }

        private void OnFieldPreInitialized()
        {
            ControlPanelManager.InitializeControlPanel();
        }

        private void OnFieldPreMoved()
        {
            gameLogicService.RecordGameStage(GamePointsType.ExtraStepCompletion);
        }

        private void OnFieldAccepted(ISet<CardinalPoint> ballUnfilledCardinalPoints)
        {
            FieldFinalizationType fieldFinalizationType = gameLogicService.IsGameRoundEnded(ballUnfilledCardinalPoints) ? FieldFinalizationType.Interval :
                FieldFinalizationType.Regular;

            StartCoroutine(FieldManager.BeginFieldFinalizationIteratively(fieldFinalizationType));
        }

        private void OnFieldPreChecked(CardinalPoint ballCheckableCardinalPoint, SubstanceColorType reservoirSubstanceColorType)
        {
            IEnumerator BeginGameStageAcceptanceIteratively()
            {
                ShutdownGameStage(GameCompletionState.Completed);

                yield return FieldManager.BeginFieldAcceptanceIteratively(ballCheckableCardinalPoint, FieldCheckingPhase.Early);

                yield return ControlPanelManager.CalibrateEntityIteratively();

                gameLogicService.RecordGameStage(GamePointsType.StageAcception);

                yield return FieldManager.BeginFieldAcceptanceIteratively(ballCheckableCardinalPoint, FieldCheckingPhase.Late);
            }

            IEnumerator RejectGameStageIteratively()
            {
                yield return FieldManager.RejectFieldIteratively(FieldCheckingPhase.Early);

                gameLogicService.RecordGameStage(GamePointsType.StageRejection);

                yield return FieldManager.RejectFieldIteratively(FieldCheckingPhase.Late);
            }

            IEnumerator fieldPostCheckedRoutine = gameLogicService.IsGameStageEnded(ballCheckableCardinalPoint, reservoirSubstanceColorType) ?
                BeginGameStageAcceptanceIteratively() : RejectGameStageIteratively();

            StartCoroutine(fieldPostCheckedRoutine);
        }

        private void OnGameStageCharacteristicRefreshed(IDetailedExtractableGameStageContentCharacteristics gameStageMetric)
        {
            ControlPanelManager.RefreshEntityPartially(gameStageMetric.GetDetails());
        }

        private void OnGameStageConductingTaskDone()
        {
            IEnumerator ProcessGameStageConductingTaskDoneIteratively()
            {
                bool possibleFieldFailingFinalizationInitiationResult = default;

                yield return FieldManager.InitiateFieldNonSequentalActionIteratively(FieldNonSequentalActionType.PossibleFailingFinalization,
                    possibleFieldFailingIntervalFinalizationResultParameter =>
                    possibleFieldFailingFinalizationInitiationResult = possibleFieldFailingIntervalFinalizationResultParameter);

                if (possibleFieldFailingFinalizationInitiationResult)
                {
                    gameStatesData.Completion = GameCompletionState.Aborted;

                    yield return FinishGameStateFailingIteratively();
                }
            }

            StartCoroutine(ProcessGameStageConductingTaskDoneIteratively());
        }

        private void OnGameStageDataGenerated(GameStageData gameStageData)
        {
            IEnumerator ProcessGameStageDataGeneratedIteratively()
            {
                yield return ControlPanelManager.RefreshEntityCompletelyIteratively(gameStageData.Extractable);

                FieldManager.BeginFieldInitialization(gameStageData.PathToReservoir, gameStageData.CheckingConditionFunction);
            }

            StartCoroutine(ProcessGameStageDataGeneratedIteratively());
        }

        private enum GameCompletionState
        {
            Aborted,

            Completed,

            Uncompleted
        }

        private enum GameRunningState
        {
            None,

            NotRunned,

            Runned
        }

        private enum GameStageConductingMode
        {
            Off,

            On,

            Shutdown
        }

        private class GameStatesData
        {
            private GameCompletionState completion;

            private GameRunningState running;

            public GameStatesData()
            {
                Completion = GameCompletionState.Uncompleted;
                Running = GameRunningState.None;
            }

            public GameCompletionState Completion { get; set; }

            public GameRunningState Running { get; set; }
        }
    }
}

namespace GameScene.Managers.Game.Enums
{
    public enum GamePointsType
    {
        ExtraStepCompletion,

        RoundEnding,

        StageAcception,

        StageFailing,

        StageRejection
    }
}

namespace GameScene.Managers.Game.Settings
{
    [Serializable]
    public struct GamePointsAdditionSettings : IUnitedlyGettableEntityCategorySettings<int, GamePointsType>
    {
        [SerializeField]
        private int extraStepCompletion;

        [SerializeField]
        private int roundEnding;

        [SerializeField]
        private int stageAcception;

        [SerializeField]
        private int stageFailing;

        [SerializeField]
        private int stageRejection;

        public int GetSettings(GamePointsType category)
        {
            switch (category)
            {
                case GamePointsType.ExtraStepCompletion:
                    return extraStepCompletion;
                case GamePointsType.RoundEnding:
                    return roundEnding;
                case GamePointsType.StageAcception:
                    return stageAcception;
                case GamePointsType.StageFailing:
                    return stageFailing;
                default:
                    return stageRejection;
            }
        }
    }

    [Serializable]
    public struct GamePointsSettings
    {
        [SerializeField]
        private int absoluteLimit;

        [SerializeField]
        private GamePointsAdditionSettings additionSettings;

        public int AbsoluteLimit
        {
            get
            {
                return absoluteLimit;
            }
        }

        public GamePointsAdditionSettings AdditionSettings
        {
            get
            {
                return additionSettings;
            }
        }
    }

    [Serializable]
    public struct GameStageRestrictionsSettings
    {
        [SerializeField]
        private GameStageStepDurationSettings stepDuration;

        [SerializeField]
        private PathToReservoirSettings pathToReservoirSettings;

        public GameStageStepDurationSettings StepDuration
        {
            get
            {
                return stepDuration;
            }
        }

        public PathToReservoirSettings PathToReservoirSettings
        {
            get
            {
                return pathToReservoirSettings;
            }
        }
    }

    [Serializable]
    public struct GameStageSettings
    {
        [SerializeField]
        private GamePointsSettings points;

        [SerializeField]
        private GameStageRestrictionsSettings restrictions;

        public GamePointsSettings Points
        {
            get
            {
                return points;
            }
        }

        public GameStageRestrictionsSettings Restrictions
        {
            get
            {
                return restrictions;
            }
        }
    }

    [Serializable]
    public struct GameStageStepDurationSettings
    {
        [Range(0, 2), SerializeField]
        private int minutes;

        [Range(0, 59), SerializeField]
        private int seconds;

        public int Minutes
        {
            get
            {
                return minutes;
            }
        }

        public int Seconds
        {
            get
            {
                return seconds;
            }
        }
    }

    [Serializable]
    public struct PathToReservoirSettings
    {
        [SerializeField]
        private PathToReservoirLengthSettings length;

        [SerializeField]
        private PathToReservoirRotationPointsPercentageSettings rotationPointsPercentage;

        public PathToReservoirLengthSettings Length
        {
            get
            {
                return length;
            }
        }

        public PathToReservoirRotationPointsPercentageSettings RotationPointsPercentage
        {
            get
            {
                return rotationPointsPercentage;
            }
        }
    }

    [Serializable]
    public struct PathToReservoirLengthSettings : IPathToReservoirBoundarySettings<int>
    {
        [SerializeField]
        private int maximal;

        [SerializeField]
        private int minimal;

        public int Maximal
        {
            get
            {
                return maximal;
            }
        }

        public int Minimal
        {
            get
            {
                return minimal;
            }
        }
    }

    [Serializable]
    public struct PathToReservoirRotationPointsPercentageSettings : IPathToReservoirBoundarySettings<float>
    {
        [Range(0.5f, 1), SerializeField]
        private float maximal;

        [Range(0, 0.5f), SerializeField]
        private float minimal;

        public float Maximal
        {
            get
            {
                return maximal;
            }
        }

        public float Minimal
        {
            get
            {
                return minimal;
            }
        }
    }
}

namespace GameScene.Managers.Game.Settings.Interfaces
{
    public interface IPathToReservoirBoundarySettings<T> where T : struct
    {
        T Maximal { get; }

        T Minimal { get; }
    }
}