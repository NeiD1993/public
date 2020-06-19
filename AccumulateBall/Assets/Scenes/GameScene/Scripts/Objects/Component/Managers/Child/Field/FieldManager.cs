using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Ball.Info;
using GameScene.Behaviours.Platform.Enums;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Managers.Ball;
using GameScene.Managers.Entity.Actions;
using GameScene.Managers.Entity.Data;
using GameScene.Managers.Entity.Data.Interfaces;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Field.Assignment;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Events;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Info.Interfaces;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Platforms;
using GameScene.Managers.Platforms.Enums;
using GameScene.Managers.Platforms.Info;
using GameScene.Managers.Reservoir;
using GameScene.Managers.Rotators;
using GameScene.Managers.Rotators.Settings;
using GameScene.Services.Ball.Data;
using GameScene.Services.Field;
using GameScene.Services.Field.Data;
using GameScene.Services.Field.Enums;
using GameScene.Services.Field.Info;
using GameScene.Services.Game.Data;
using GameScene.Services.Game.Info;
using GameScene.Services.Game.Settings;
using GameScene.Services.Managers;
using GameScene.Services.Platform;
using GameScene.Services.Subscription.Events;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Managers.Field
{
    public class FieldManager : BaseFieldEntityManager<SimpleEntityObjectSettings, FieldInfo, FieldPrimalStatusData, FieldPrimalStatusChangedEvent>,
        IAcceptableFieldEntityManager<FieldAcceptedEvent>, IIterativelyRunnableEntityManager, INotifiedlyBannableFieldEntityManager
    {
        private readonly FieldOpeningRoutinesExecutor fieldOpeningRoutinesExecutor;

        private readonly FieldSummaryPrimalStatusFormer fieldSummaryPrimalStatusFormer;

        private AwaitableFieldActionsOperator awaitableFieldActionsOperator;

        private EntityManagersAccessor entityManagersAccessor;

        private FieldEntityBindingService fieldEntityBindingService;

        private PositionService positionService;

        public FieldManager()
        {
            fieldOpeningRoutinesExecutor = new FieldOpeningRoutinesExecutor();
            fieldSummaryPrimalStatusFormer = new FieldSummaryPrimalStatusFormer();
            FieldIdlingBeforeInterruptingFinalizationFinished = new UnityEvent();
            SetupFieldUnsuccessfullyFocusedEvent();
            EntityBanned = new UnityEvent();
            EntityPreBanned = new UnityEvent();
            FieldInitialized = new UnityEvent();
            FieldPreCompletelyFinalized = new UnityEvent();
            FieldPreInitialized = new UnityEvent();
            FieldPreMoved = new UnityEvent();
            EntityAccepted = new FieldAcceptedEvent();
            FieldFinalized = new FieldFinalizedEvent();
            FieldPreChecked = new FieldPreCheckedEvent();
        }

        private BallManager BallManager { get; set; }

        private PlatformsManager PlatformsManager { get; set; }

        private ReservoirManager ReservoirManager { get; set; }

        private RotatorsManager RotatorsManager { get; set; }

        private UnityEvent FieldIdlingBeforeInterruptingFinalizationFinished { get; set; }

        private UnityEvent FieldUnsuccessfullyFocused { get; set; }

        public int Dimension
        {
            get
            {
                return PlatformsManager.GetDimensionPlatformsCount();
            }
        }

        public override GameObject Entity
        {
            get
            {
                return EntityInfo.Field;
            }
        }

        public override FieldEntityPrimalStatusData SummaryEntityPrimalStatus
        {
            get
            {
                return fieldSummaryPrimalStatusFormer.FormSummaryStatus(new FieldEntityPrimalStatusData[]
                {
                    BallManager.SummaryEntityPrimalStatus,
                    ReservoirManager.SummaryEntityPrimalStatus,
                    PlatformsManager.SummaryEntityPrimalStatus,
                    RotatorsManager.SummaryEntityPrimalStatus
                });
            }
        }

        public UnityEvent EntityBanned { get; private set; }

        public UnityEvent EntityPreBanned { get; private set; }

        public UnityEvent FieldInitialized { get; private set; }

        public UnityEvent FieldPreCompletelyFinalized { get; private set; }

        public UnityEvent FieldPreInitialized { get; private set; }

        public UnityEvent FieldPreMoved { get; private set; }

        public FieldAcceptedEvent EntityAccepted { get; private set; }

        public FieldFinalizedEvent FieldFinalized { get; private set; }

        public FieldPreCheckedEvent FieldPreChecked { get; private set; }

        private void EndFieldFinalization(FieldFinalizationType fieldFinalizationType)
        {
            EntityInfo.FlowingInfo.AssignmentsInfo.Reset();
            EntityInfo.FlowingInfo.TraitsInfo.Reset();
            FieldFinalized.Invoke(fieldFinalizationType);
        }

        private void ObtainChildFieldEntityManagers()
        {
            BallManager = entityManagersAccessor.GetManager<BallManager>();
            PlatformsManager = entityManagersAccessor.GetManager<PlatformsManager>();
            ReservoirManager = entityManagersAccessor.GetManager<ReservoirManager>();
            RotatorsManager = entityManagersAccessor.GetManager<RotatorsManager>();
        }

        private void PostProcessFieldUpdatingOnBallMovedToActivatedPlatform(Vector2Int previousBallPosition)
        {
            MoveDirection moveDirectionToReservoir = positionService.GetMoveDirectionBetweenPositions(BallManager.GetObjectCheckableInfo(),
                ReservoirManager.GetObjectCheckableInfo());
            CardinalPoint ballCheckableCardinalPoint = BallManager.GetObjectCheckableInfo(moveDirectionToReservoir);
            SubstanceColorType reservoirSubstanceColorType = ((ICheckableGeneratedFieldObjectManager<SubstanceColorType>)ReservoirManager).GetObjectCheckableInfo();

            FieldPreChecked.Invoke(ballCheckableCardinalPoint, reservoirSubstanceColorType);
        }

        private void PrepareFieldUpdatingOnBallMovingToPosition((Vector2Int current, Vector2Int next) ballPositions)
        {
            FieldPositionType DefineNextBallPositionType()
            {
                Vector2Int nextBallPosition = ballPositions.next;

                if (PlatformsManager.IsActivatedPlatformPosition(nextBallPosition))
                    return FieldPositionType.ActivatedPlatform;
                else if (RotatorsManager.IsRotatorPosition(nextBallPosition))
                    return FieldPositionType.Rotator;
                else
                    return FieldPositionType.IdlePlatform;
            }

            void AnalyzeFieldUpdatingOnBallMovingToPosition(FieldPositionType nextBallPositionType, bool isMovingFromActivatedPlatform)
            {
                void PrepareFieldUpdatingOnBallMovingToPositionStarted()
                {
                    (Action<Vector2Int> from, Action<Vector2Int> to) CreateActionsOnBallMovingToPositionStarted()
                    {
                        Action<Vector2Int> CreatOutputActionOnBallMovingToPositionStarted()
                        {
                            if (isMovingFromActivatedPlatform)
                                return null;
                            else
                                return PlatformsManager.BeginPlatformBanning;
                        }

                        Action<Vector2Int> CreatInputActionOnBallMovingToPositionStarted()
                        {
                            switch (nextBallPositionType)
                            {
                                case FieldPositionType.ActivatedPlatform:
                                    return nextBallPositionParameter => PlatformsManager.ChangePlatformCheckingStatus(nextBallPositionParameter, CheckingStatus.Begun);
                                case FieldPositionType.IdlePlatform:
                                    return null;
                                default:
                                    return RotatorsManager.PrepareRotatorForRotation;
                            }
                        }

                        return (CreatOutputActionOnBallMovingToPositionStarted(), CreatInputActionOnBallMovingToPositionStarted());
                    }

                    void AddAwaitableFieldActionsOnBallMovingToPositionStarted()
                    {
                        void AddCustomAwaitableFieldActionOnBallMovingToPositionStarted()
                        {
                            (EventsWithUnsubscribingListeners eventsWithUnsubscribingListeners, AwaitableFieldActionType awaitableFieldActionType) awaitableFieldActionAddingParameters;

                            switch (nextBallPositionType)
                            {
                                case FieldPositionType.ActivatedPlatform:
                                    awaitableFieldActionAddingParameters = (new EventsWithUnsubscribingListeners(PlatformsManager.PlatformPostFocused, PlatformsManager.Destroyed), AwaitableFieldActionType.PlatformPostFocusing);
                                    break;
                                default:
                                    awaitableFieldActionAddingParameters = (new EventsWithUnsubscribingListeners(RotatorsManager.RotatorPreparedForRotation, RotatorsManager.Destroyed), AwaitableFieldActionType.RotatorPreparingForRotation);
                                    break;
                            }

                            awaitableFieldActionsOperator.AddAwaitableFieldAction(awaitableFieldActionAddingParameters.eventsWithUnsubscribingListeners, awaitableFieldActionAddingParameters.awaitableFieldActionType);
                        }

                        if (!isMovingFromActivatedPlatform)
                            awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(PlatformsManager.PlatformBanned,
                                PlatformsManager.Destroyed), AwaitableFieldActionType.PlatformBanning);

                        if (nextBallPositionType != FieldPositionType.IdlePlatform)
                            AddCustomAwaitableFieldActionOnBallMovingToPositionStarted();
                    }

                    eventsListenersService.AddUnsubscribingEventListener(nextBallPositionParameter => 
                    OnBallMovingToPositionStarted(ballPositions.next, CreateActionsOnBallMovingToPositionStarted()), 
                    new EventsWithUnsubscribingListeners<Vector2Int>(BallManager.BallMovingStarted, BallManager.Destroyed));
                    AddAwaitableFieldActionsOnBallMovingToPositionStarted();
                }

                void PrepareFieldUpdatingOnBallMovedToPosition()
                {
                    Action<Vector2Int> CreateCustomAdditionalActionOnBallMovedToPosition()
                    {
                        switch (nextBallPositionType)
                        {
                            case FieldPositionType.ActivatedPlatform:
                                return PostProcessFieldUpdatingOnBallMovedToActivatedPlatform;
                            case FieldPositionType.IdlePlatform:
                                return previousBallPositionParameter => StartCoroutine(MakeFieldDecisionIteratively(EffectFieldFocusingIteratively()));
                            default:
                                return previousBallPositionParameter => StartCoroutine(PostProcessFieldUpdatingOnBallMovedToRotatorIteratively(previousBallPositionParameter));
                        }
                    }

                    eventsListenersService.AddUnsubscribingEventListener(previousBallPositionParameter => 
                    StartCoroutine(ProcessFieldUpdatingOnBallMovedToPositionIteratively(previousBallPositionParameter, CreateCustomAdditionalActionOnBallMovedToPosition())), 
                    new EventsWithUnsubscribingListeners<Vector2Int>(BallManager.BallMoved, BallManager.Destroyed));
                }

                PrepareFieldUpdatingOnBallMovingToPositionStarted();
                PrepareFieldUpdatingOnBallMovedToPosition();
            }

            AnalyzeFieldUpdatingOnBallMovingToPosition(DefineNextBallPositionType(), PlatformsManager.IsActivatedPlatformPosition(ballPositions.current));
        }

        private void SetupChildFieldEntityManagers()
        {
            SetupBallManager();
            SetupPlatformsManager();
            SetupReservoirManager();
            SetupRotatorsManager();
        }

        private void SetupBallManager()
        {
            BallManager.Destroyed.AddListener(OnBallManagerDestroyed);
            BallManager.EntityLaunched.AddListener(OnBallLaunched);
            BallManager.EntityPlaced.AddListener(OnBallPlaced);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessChildManagerEntityPrimalStatusChangedIteratively(BallManager)),
                BallManager.EntityPrimalStatusChanged);
            BallManager.EntityObjectDestroyed.AddListener(OnBallDestroyed);
        }

        private void SetupPlatformsManager()
        {
            void AddPlatformsConfiningEventsListeners()
            {
                PlatformsManager.FocusedPlatformsConfining.Confined.AddListener(OnFocusedPlatformsConfined);
                PlatformsManager.FocusedPlatformsConfining.ReadyForConfining.AddListener(OnFocusedPlatformsReadyForConfining);
            }

            PlatformsManager.Destroyed.AddListener(OnPlatformsManagerDestroyed);
            PlatformsManager.EntityLaunched.AddListener(OnPlatformsLaunched);
            PlatformsManager.EntityPlaced.AddListener(OnPlatformsPlaced);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessChildManagerEntityPrimalStatusChangedIteratively(PlatformsManager)),
                PlatformsManager.EntityPrimalStatusChanged);
            PlatformsManager.EntityAccepted.AddListener(OnPlatformAccepted);
            PlatformsManager.EntityActivated.AddListener(OnPlatformsActivated);
            PlatformsManager.PlatformsDestroyed.AddListener(OnPlatformsDestroyed);
            AddPlatformsConfiningEventsListeners();
        }

        private void SetupReservoirManager()
        {
            ReservoirManager.Destroyed.AddListener(OnReservoirManagerDestroyed);
            ReservoirManager.EntityPlaced.AddListener(OnReservoirPlaced);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessChildManagerEntityPrimalStatusChangedIteratively(ReservoirManager)),
                ReservoirManager.EntityPrimalStatusChanged);
        }

        private void SetupRotatorsManager()
        {
            RotatorsManager.Destroyed.AddListener(OnRotatorsManagerDestroyed);
            RotatorsManager.EntityPlaced.AddListener(OnRotatorsPlaced);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessChildManagerEntityPrimalStatusChangedIteratively(RotatorsManager)),
                RotatorsManager.EntityPrimalStatusChanged);
        }

        private void SetupFieldUnsuccessfullyFocusedEvent()
        {
            FieldUnsuccessfullyFocused = new UnityEvent();
            FieldUnsuccessfullyFocused.AddListener(OnFieldUnsuccessfullyFocused);
        }

        private IEnumerator ActuateFieldIteratively(bool isFirstActuating = true)
        {
            yield return RotatorsManager.RunEntityIteratively();

            ReservoirManager.RunObject();

            if (isFirstActuating)
            {
                yield return PlatformsManager.RunEntityWithPrimalStatusTogglingIteratively();

                BallManager.RunObject();
            }

            yield return EffectFieldFocusingIteratively();
        }

        private IEnumerator ContinueFieldFinalizationOnReservoirDestroyingIteratively(FieldFinalizationType fieldFinalizationType, Vector2Int reservoirDestroyingPosition)
        {
            IEnumerator UpdateFieldByBallFinalizationIteratively()
            {
                if ((fieldFinalizationType != FieldFinalizationType.Failing) && (fieldFinalizationType != FieldFinalizationType.Regular))
                {
                    awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(BallManager.ObjectPartsFlushed, BallManager.Destroyed),
                        AwaitableFieldActionType.BallPartsFlushing);
                    BallManager.BeginBallPartsFlushing(fieldFinalizationType == FieldFinalizationType.Complete ? FlushingType.Full : FlushingType.Half);

                    yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.BallPartsFlushing);
                }

                if (fieldFinalizationType == FieldFinalizationType.Complete)
                    yield return BallManager.BeginObjectHidingIteratively();
            }

            yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionsExecutedIteratively();

            PlatformsManager.AddFreePlatform(reservoirDestroyingPosition);

            yield return UpdateFieldByBallFinalizationIteratively();

            if (fieldFinalizationType != FieldFinalizationType.Complete)
                EndFieldFinalization(fieldFinalizationType);
        }

        private IEnumerator EffectFieldFocusingIteratively(bool isDefocusing = false)
        {
            IEnumerator FocusFieldIteratively()
            {
                bool isSuccessful = default;
                FieldFlowingTraitsInfo traitsInfo;

                yield return PlatformsManager.FocusNeighboringPlatformsWithEntityPrimalStatusTogglingIteratively(BallManager.GetObjectCheckableInfo(), 
                    isSuccessfulParameter => isSuccessful = isSuccessfulParameter);

                traitsInfo = EntityInfo.FlowingInfo.TraitsInfo;

                if (isSuccessful)
                {
                    traitsInfo.TryModify(FieldFlowingStatus.Selection);
                    traitsInfo.TryModify(FieldFlowingVerdict.None);
                }
                else
                {
                    traitsInfo.TryModify(FieldFlowingVerdict.Abortion);
                    FieldUnsuccessfullyFocused.Invoke();
                }
            }

            FieldFlowingAssignmentsInfo fieldFlowingAssignmentsInfo = EntityInfo.FlowingInfo.AssignmentsInfo;

            if (isDefocusing)
            {
                FieldIndividualAssignment fieldIndividualAssignment = fieldFlowingAssignmentsInfo.IndividualAssignment;

                if (!fieldIndividualAssignment.IsFulfilled)
                {
                    yield return new WaitUntil(() => PlatformsManager.AreLiberatablePlatformsExist(LiberatablePlatformsType.Focused));

                    StopCoroutine(fieldIndividualAssignment.Routine);
                }

                yield return PlatformsManager.DefocusIdlyFocusedPlatformsWithEntityPrimalStatusTogglingIteratively();
            }
            else
            {
                fieldFlowingAssignmentsInfo.Modify(new FieldIndividualAssignment(FocusFieldIteratively()));

                yield return StartCoroutine(fieldFlowingAssignmentsInfo.IndividualAssignment.Routine);
            }
        }

        private IEnumerator IdleFieldBeforeInterruptingFinalizationIteratively(IEnumerator customAdditionalRoutine = null)
        {
            awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(FieldIdlingBeforeInterruptingFinalizationFinished, Destroyed),
                AwaitableFieldActionType.FieldIdlingBeforeInterruptingFinalization);

            yield return awaitableFieldActionsOperator
                .WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.FieldIdlingBeforeInterruptingFinalization);

            if (customAdditionalRoutine != null)
                yield return customAdditionalRoutine;
        }

        private IEnumerator MakeFieldDecisionIteratively(IEnumerator decisionRoutine, FieldFlowingVerdict verdict = FieldFlowingVerdict.Continuation)
        {
            FieldFlowingTraitsInfo traitsInfo = EntityInfo.FlowingInfo.TraitsInfo;

            traitsInfo.TryModify(FieldFlowingStatus.Decision);

            if (traitsInfo.TryModify(verdict))
                yield return decisionRoutine;
        }

        private IEnumerator PostProcessFieldUpdatingOnBallMovedToRotatorIteratively(Vector2Int previousBallPosition)
        {
            void PrepareFieldUpdatingOnBallRotation()
            {
                eventsListenersService.AddUnsubscribingEventListener(OnBallRotationStarted,
                    new EventsWithUnsubscribingListeners(BallManager.BallRotationStarted, BallManager.Destroyed));
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(RotatorsManager.RotatorRotated, RotatorsManager.Destroyed),
                    AwaitableFieldActionType.RotatorRotation);
            }

            IEnumerator EndFieldUpdatingOnBallMovedToRotatorIteratively()
            {
                void PrepareFieldUpdatingOnBallMovingFromRotator()
                {
                    eventsListenersService.AddUnsubscribingEventListener(OnBallMovingFromRotatorStarted,
                        new EventsWithUnsubscribingListeners<Vector2Int>(BallManager.BallMovingStarted, new UnityEvent[] { BallManager.Destroyed, RotatorsManager.EntityObjectDestroyed }));
                    awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(RotatorsManager.EntityObjectDestroyed,
                        RotatorsManager.Destroyed), AwaitableFieldActionType.RotatorDestroying);
                }

                PrepareFieldUpdatingOnBallMovingFromRotator();

                yield return EffectFieldFocusingIteratively();
            }

            PrepareFieldUpdatingOnBallRotation();

            yield return BallManager.RotateBallIteratively(RotatorsManager.GetRotatorRotationType(BallManager.GetObjectCheckableInfo()));

            yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.RotatorRotation);

            yield return MakeFieldDecisionIteratively(EndFieldUpdatingOnBallMovedToRotatorIteratively());
        }

        private IEnumerator PreBeginFieldFailingFinalizationIteratively((object before, Action after) customAdditionalFieldInputFailingActivities)
        {
            IEnumerator PerformCustomAdditionalActivityBeforeFieldInputFailingIteratively()
            {
                object customAdditionalActivityBeforeFieldInputFailing = customAdditionalFieldInputFailingActivities.before;

                if (customAdditionalActivityBeforeFieldInputFailing != null)
                {
                    if (customAdditionalActivityBeforeFieldInputFailing is Action)
                        ((Action)customAdditionalActivityBeforeFieldInputFailing).Invoke();
                    else if (customAdditionalActivityBeforeFieldInputFailing is IEnumerator)
                        yield return (IEnumerator)customAdditionalActivityBeforeFieldInputFailing;
                }
            }

            IEnumerator PreBeginFieldFailingFinalizationAfterIdlingIteratively(Vector2Int failedPlatformPosition)
            {
                yield return UpdateFieldWithPossibleParallelUnbanningIteratively(PlatformsManager.FailPlatformWithEntityPrimalStatusTogglingIteratively(failedPlatformPosition, FailingType.Output));

                yield return BeginFieldFinalizationIteratively(FieldFinalizationType.Failing);
            }

            Vector2Int ballPosition = BallManager.GetObjectCheckableInfo();

            yield return PerformCustomAdditionalActivityBeforeFieldInputFailingIteratively();

            yield return PlatformsManager.FailPlatformWithEntityPrimalStatusTogglingIteratively(ballPosition, FailingType.Input);

            customAdditionalFieldInputFailingActivities.after?.Invoke();

            StartCoroutine(IdleFieldBeforeInterruptingFinalizationIteratively(PreBeginFieldFailingFinalizationAfterIdlingIteratively(ballPosition)));
        }

        private IEnumerator ProcessChildManagerEntityPrimalStatusChangedIteratively(IStandardFieldEntityManager childFieldEntityManager)
        {
            Action unlockManagersWithPrimalStatusChangingAction = () =>
            {
                childFieldEntityManager.Unlock();
                Unlock();
            };

            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus,
                () => EntityInfo.Suspension == FieldEntitySuspensionStatus.NotSuspended,
                new EntityStatusChangingActions<FieldEntityPrimalStatusData>(
                    entityPrimalStatusDataParameter =>
                    {
                        unlockManagersWithPrimalStatusChangingAction();
                        EntityPrimalStatusChanged.Invoke(entityPrimalStatusDataParameter);
                    },
                    () => unlockManagersWithPrimalStatusChangingAction()));
        }

        private IEnumerator ProcessFieldUpdatingOnBallMovedToPositionIteratively(Vector2Int previousBallPosition, Action<Vector2Int> customAdditionalAction)
        {
            yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionsExecutedIteratively();

            PlatformsManager.AddFreePlatform(previousBallPosition);
            PlatformsManager.RemoveFreePlatform(BallManager.GetObjectCheckableInfo());
            FieldPreMoved.Invoke();
            BallManager.ReRunBall();
            customAdditionalAction(previousBallPosition);
        }

        private IEnumerator ProcessFieldUpdatingOnReservoirLaunchedIteratively(IEnumerable<GeneratedRotatorSettings> generatedRotatorSettings)
        {
            yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionsExecutedIteratively();

            StartCoroutine(RotatorsManager.CreateRotatorsWithEntityPrimalStatusTogglingIteratively(PlatformsManager.FreePlatforms, generatedRotatorSettings));
        }

        private IEnumerator UpdateFieldWithPossibleParallelUnbanningIteratively(IEnumerator updateRoutine)
        {
            IEnumerator UpdateFieldWithParallelUnbanningIteratively()
            {
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(PlatformsManager.EntityUnbanned, PlatformsManager.Destroyed),
                    AwaitableFieldActionType.PlatformsUnbanning);
                PlatformsManager.BeginPlatformsUnbanning();

                yield return updateRoutine;

                yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.PlatformsUnbanning);
            }

            yield return PlatformsManager.AreLiberatablePlatformsExist(LiberatablePlatformsType.Banned) ? UpdateFieldWithParallelUnbanningIteratively() : updateRoutine;
        }

        protected override void PerpetrateStartProcessing()
        {
            SetupChildFieldEntityManagers();
            base.PerpetrateStartProcessing();
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            awaitableFieldActionsOperator = new AwaitableFieldActionsOperator();
            entityManagersAccessor = SharedSceneServicesLocator.GetService<EntityManagersAccessor>();
            fieldEntityBindingService = SharedSceneServicesLocator.GetService<FieldEntityBindingService>();
            positionService = SharedSceneServicesLocator.GetService<PositionService>();
        }

        protected override void PreProcessStart()
        {
            base.PreProcessStart();
            ObtainChildFieldEntityManagers();
        }

        protected override void ResumeEntityInternally()
        {
            BallManager.ResumeEntity();
            ReservoirManager.ResumeEntity();
            PlatformsManager.ResumeEntity();
            RotatorsManager.ResumeEntity();
        }

        protected override void SuspendEntityInternally()
        {
            BallManager.SuspendEntity();
            ReservoirManager.SuspendEntity();
            PlatformsManager.SuspendEntity();
            RotatorsManager.SuspendEntity();
        }

        protected override Func<bool> GetCanConfigureConditionFunction()
        {
            return () => (BallManager?.IsConfigurated).Value && (ReservoirManager?.IsConfigurated).Value && (PlatformsManager?.IsConfigurated).Value &&
            (RotatorsManager?.IsConfigurated).Value;
        }

        public void BeginFieldInitialization(PathToReservoirData pathToReservoirData, Func<bool> checkingConditionFunction)
        {
            void PrepareFieldUpdatingOnReservoirLaunched(ICollection<GeneratedRotatorSettings> generatedRotatorsSettings)
            {
                eventsListenersService.AddUnsubscribingEventListener(() => StartCoroutine(ProcessFieldUpdatingOnReservoirLaunchedIteratively(generatedRotatorsSettings)),
                    new EventsWithUnsubscribingListeners(ReservoirManager.EntityLaunched, ReservoirManager.Destroyed));
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(PlatformsManager.PlatformEnlarged, PlatformsManager.Destroyed),
                    AwaitableFieldActionType.PlatformEnlarging);
            }

            PathToReservoirObservingInfo pathToReservoirObservingInfo = pathToReservoirData.ObservingInfo;
            PathToReservoirGeneratedEntitiesSettings pathToReservoirGeneratedEntitiesSettings = pathToReservoirData.GeneratedEntitiesSettings;

            PrepareFieldUpdatingOnReservoirLaunched(pathToReservoirGeneratedEntitiesSettings.GeneratedRotatorsSettings);
            eventsListenersService.AddUnsubscribingEventListener(() => OnRotatorsLaunched(pathToReservoirGeneratedEntitiesSettings.PossibleSettings.BallRotationsData), 
                new EventsWithUnsubscribingListeners(RotatorsManager.EntityLaunched, RotatorsManager.Destroyed));
            eventsListenersService.AddUnsubscribingEventListener(() =>
            OnBallRotationsDataApplied(pathToReservoirGeneratedEntitiesSettings.PossibleSettings.ActivatedPlatformsPositionsNearReservoirs), 
            new EventsWithUnsubscribingListeners(BallManager.BallRotationsDataApplied, BallManager.Destroyed));
            fieldOpeningRoutinesExecutor.AddRoutine(PlatformsManager.OpenPlatformsWithEntityPrimalStatusTogglingIteratively(pathToReservoirObservingInfo.Opening));
            PlatformsManager.NeighboringPlatformsPreferringInfo = new NeighboringPlatformsPreferringInfo(checkingConditionFunction, pathToReservoirObservingInfo.VariativePathPointsDataPickingRoutineExtractor);
            ReservoirManager.GenerateReservoir(EntityInfo.Field, PlatformsManager.FreePlatforms, 
                pathToReservoirGeneratedEntitiesSettings.PossibleSettings.GeneratedReservoirSettings);
        }

        public void ProceedFieldInterruptingFinalization()
        {
            FieldIdlingBeforeInterruptingFinalizationFinished.Invoke();
        }

        public IEnumerator BeginFieldAcceptanceIteratively(CardinalPoint ballAcceptedCardinalPoint, FieldCheckingPhase fieldAcceptancePhase)
        {
            void UpdateFieldByBallPartFillingInitiation()
            {
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(BallManager.BallPartFilled, BallManager.Destroyed),
                    AwaitableFieldActionType.BallPartFilling);
                BallManager.BeginBallPartFilling(ballAcceptedCardinalPoint);
            }

            void UpdateFieldByReservoirFlushingInitiation()
            {
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(ReservoirManager.ObjectPartsFlushed, ReservoirManager.Destroyed),
                    AwaitableFieldActionType.ReservoirFlushing);
                ReservoirManager.BeginReservoirSubstanceFlushing();
            }

            Vector2Int acceptedPlatformPosition = BallManager.GetObjectCheckableInfo();

            switch (fieldAcceptancePhase)
            {
                case FieldCheckingPhase.Early:
                    yield return MakeFieldDecisionIteratively(UpdateFieldWithPossibleParallelUnbanningIteratively(PlatformsManager.CheckPlatformIteratively(acceptedPlatformPosition, CheckingAction.Accept)),
                        FieldFlowingVerdict.Shutting);
                    break;
                default:
                    {
                        yield return fieldOpeningRoutinesExecutor.ExecuteRoutinesIteratively();

                        yield return PlatformsManager.CloseIdlyOpenedPlatformsIteratively();

                        UpdateFieldByBallPartFillingInitiation();
                        UpdateFieldByReservoirFlushingInitiation();

                        yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionsExecutedIteratively();

                        PlatformsManager.ChangePlatformCheckingStatus(acceptedPlatformPosition, CheckingStatus.Ended);
                    }
                    break;
            }
        }

        public IEnumerator BeginFieldFinalizationIteratively(FieldFinalizationType fieldFinalizationType)
        {
            IEnumerator UpdateFieldByBeginningPlatformsFinalizationIteratively()
            {
                if (fieldFinalizationType == FieldFinalizationType.Complete)
                {
                    yield return EffectFieldFocusingIteratively(true);

                    yield return UpdateFieldWithPossibleParallelUnbanningIteratively(PlatformsManager.SkipPlatformIteratively(BallManager.GetObjectCheckableInfo()));
                }

                if (awaitableFieldActionsOperator.ContainsAwaitableFieldAction(AwaitableFieldActionType.PlatformPostRejecting))
                {
                    PlatformsManager.ChangePlatformCheckingStatus(BallManager.GetObjectCheckableInfo(), CheckingStatus.Ended);

                    yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.PlatformPostRejecting);
                }

                yield return PlatformsManager.DeactivatePlatformsIteratively();
            }

            IEnumerator UpdateFieldByRotatorsFinalizationIteratively()
            {
                if (awaitableFieldActionsOperator.ContainsAwaitableFieldAction(AwaitableFieldActionType.RotatorDestroying))
                {
                    yield return RotatorsManager.BeginRotatorHidingIteratively(BallManager.GetObjectCheckableInfo());

                    yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.RotatorDestroying);
                }

                yield return RotatorsManager.HideRotatorsIteratively();
            }

            void PrepareFieldUpdatingOnReservoirDestroyed()
            {
                eventsListenersService.AddUnsubscribingEventListener(reservoirDestroyingPositionParameter =>
                StartCoroutine(ContinueFieldFinalizationOnReservoirDestroyingIteratively(fieldFinalizationType, reservoirDestroyingPositionParameter)),
                new EventsWithUnsubscribingListeners<Vector2Int>(ReservoirManager.EntityObjectDestroyed, ReservoirManager.Destroyed));
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(PlatformsManager.PlatformLowered, PlatformsManager.Destroyed),
                    AwaitableFieldActionType.PlatformLowering);
            }

            IEnumerator UpdateFieldByReservoirDisappearingInitiationIteratively()
            {
                awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(ReservoirManager.ReservoirFreezed, ReservoirManager.Destroyed),
                    AwaitableFieldActionType.ReservoirFreezing);

                yield return ReservoirManager.BeginObjectHidingIteratively();

                yield return awaitableFieldActionsOperator.WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType.ReservoirFreezing);
            }

            if ((fieldFinalizationType == FieldFinalizationType.Complete) || (fieldFinalizationType == FieldFinalizationType.Failing))
                fieldOpeningRoutinesExecutor.ClearRoutines();

            yield return UpdateFieldByBeginningPlatformsFinalizationIteratively();

            yield return UpdateFieldByRotatorsFinalizationIteratively();

            PrepareFieldUpdatingOnReservoirDestroyed();

            yield return UpdateFieldByReservoirDisappearingInitiationIteratively();

            PlatformsManager.BeginPlatformLowering(ReservoirManager.GetObjectCheckableInfo());
        }

        public IEnumerator BeginFieldPreInitializationIteratively()
        {
            EntityInfo.Field = new GameObject(entityObjectSettings.InstanceName);

            yield return PlatformsManager.CreatePlatformsIteratively();
        }

        public IEnumerator ContinueFieldIteratively()
        {
            yield return ActuateFieldIteratively(false);
        }

        public IEnumerator InitiateFieldNonSequentalActionIteratively(FieldNonSequentalActionType actionType, Action<bool> resultExtractor)
        {
            IEnumerator InitiateFieldPossibleFailingFinalizationIteratively()
            {
                IEnumerator GetFieldFailingIndicatorsIteratively(Action<(bool result, FieldFlowingStatus? abortionStatus)> failingIndicatorsExtractor)
                {
                    IEnumerator GetAbortionFieldFlowingStatusIteratively(Action<FieldFlowingStatus?> abortionStatusExtractor)
                    {
                        FieldFlowingStatus? abortionStatus = null;
                        FieldFlowingTraitsInfo traitsInfo = EntityInfo.FlowingInfo.TraitsInfo;

                        if (traitsInfo.TryModify(FieldFlowingVerdict.Abortion))
                        {
                            if ((traitsInfo.Status == FieldFlowingStatus.Decision) || (traitsInfo.Status == FieldFlowingStatus.Selection))
                                abortionStatus = traitsInfo.Status;
                            else if (traitsInfo.Status == FieldFlowingStatus.Reaction)
                            {
                                yield return new WaitUntil(() => traitsInfo.Status == FieldFlowingStatus.Decision);

                                if (traitsInfo.Verdict == FieldFlowingVerdict.Abortion)
                                    abortionStatus = FieldFlowingStatus.Reaction;
                            }
                        }

                        abortionStatusExtractor(abortionStatus);
                    }

                    (bool result, FieldFlowingStatus? abortionStatus) indicators = default;

                    yield return GetAbortionFieldFlowingStatusIteratively(abortionStatusParameter => indicators.abortionStatus = abortionStatusParameter);

                    indicators.result = indicators.abortionStatus != null;
                    failingIndicatorsExtractor(indicators);
                }

                IEnumerator UnfocusFieldBeforePreBeginningFailingFinalizationIteratively(FieldFlowingStatus abortionStatus)
                {
                    if (abortionStatus != FieldFlowingStatus.Reaction)
                        yield return EffectFieldFocusingIteratively(true);
                }

                (bool result, FieldFlowingStatus? abortionStatus) failingIndicators = default;

                yield return GetFieldFailingIndicatorsIteratively(failingIndicatorsParameter => failingIndicators = failingIndicatorsParameter);

                if (failingIndicators.result)
                    yield return PreBeginFieldFailingFinalizationIteratively((UnfocusFieldBeforePreBeginningFailingFinalizationIteratively(failingIndicators.abortionStatus.Value), null));

                resultExtractor(failingIndicators.result);
            }

            IEnumerator InitiateFieldNecessaryNonSequentalActionIteratively()
            {
                IEnumerator PreBeginFieldCompleteFinalizationIteratively()
                {
                    if (EntityInfo.Suspension == FieldEntitySuspensionStatus.NotSuspended)
                        SuspendEntity();

                    yield return IdleFieldBeforeInterruptingFinalizationIteratively();

                    ResumeEntity();

                    yield return BeginFieldFinalizationIteratively(FieldFinalizationType.Complete);
                }

                Action action;
                Func<FieldEntityPrimalStatusData, bool> actionInitiationConditionFunction;

                switch (actionType)
                {
                    case FieldNonSequentalActionType.CompleteFinalization:
                        {
                            action = () => StartCoroutine(PreBeginFieldCompleteFinalizationIteratively());
                            actionInitiationConditionFunction = primalStatusDataParameter =>
                            (primalStatusDataParameter.Availability == FieldEntityAvailabilityStatus.Available) &&
                            (primalStatusDataParameter.Finalization == FieldEntityFinalizationStatus.ReadyForFinalization) &&
                            EntityInfo.FlowingInfo.TraitsInfo.TryModify(FieldFlowingVerdict.Abortion);
                        }
                        break;
                    default:
                        {
                            action = () => SuspendEntity();
                            actionInitiationConditionFunction = primalStatusDataParameter =>
                            primalStatusDataParameter.Availability == FieldEntityAvailabilityStatus.Available;
                        }
                        break;
                }

                yield return EntityInfo.PrimalStatusData.GetStatusIteratively(primalStatusDataParameter =>
                {
                    bool result = actionInitiationConditionFunction(primalStatusDataParameter);

                    if (result)
                        action();

                    resultExtractor(result);
                    Unlock();
                });
            }

            IEnumerator fieldNonSequentalActionInitiationRoutine;

            fieldNonSequentalActionInitiationRoutine = (actionType == FieldNonSequentalActionType.PossibleFailingFinalization) ?
                InitiateFieldPossibleFailingFinalizationIteratively() : InitiateFieldNecessaryNonSequentalActionIteratively();

            yield return fieldNonSequentalActionInitiationRoutine;
        }

        public IEnumerator RejectFieldIteratively(FieldCheckingPhase fieldRejectingPhase)
        {
            IEnumerator EndFieldRejectingIteratively()
            {
                eventsListenersService.AddUnsubscribingEventListener(OnBallMovingFromActivatedPlatformStarted,
                    new EventsWithUnsubscribingListeners<Vector2Int>(BallManager.BallMovingStarted, BallManager.Destroyed));

                yield return EffectFieldFocusingIteratively();
            }

            switch (fieldRejectingPhase)
            {
                case FieldCheckingPhase.Early:
                    {
                        yield return PlatformsManager.CheckPlatformIteratively(BallManager.GetObjectCheckableInfo(), CheckingAction.Reject);

                        awaitableFieldActionsOperator.AddAwaitableFieldAction(new EventsWithUnsubscribingListeners(PlatformsManager.PlatformPostRejected,
                            PlatformsManager.Destroyed), AwaitableFieldActionType.PlatformPostRejecting);
                    }
                    break;
                default:
                    yield return MakeFieldDecisionIteratively(EndFieldRejectingIteratively());
                    break;
            }
        }

        public IEnumerator RunEntityIteratively()
        {
            yield return ActuateFieldIteratively();
        }

        public PathToReservoirGenerativeInfo CreateFieldInitializationData()
        {
            return new PathToReservoirGenerativeInfo(BallManager.GetObjectCheckableInfo(), PlatformsManager.GetDimensionPlatformsCount(true),
                ((ICheckableGeneratedFieldObjectManager<BallInfo>)BallManager).GetObjectCheckableInfo());
        }

        private void OnBallDestroyed(Vector2Int ballDestroyingPosition)
        {
            PlatformsManager.AddFreePlatform(ballDestroyingPosition);
            StartCoroutine(PlatformsManager.BeginPlatformsHidingIteratively());
            FieldPreCompletelyFinalized.Invoke();
        }

        private void OnBallLaunched()
        {
            FieldPreInitialized.Invoke();
        }

        private void OnBallManagerDestroyed()
        {
            BallManager.Destroyed.RemoveListener(OnBallManagerDestroyed);
            BallManager.EntityLaunched.RemoveListener(OnBallLaunched);
            BallManager.EntityPlaced.RemoveListener(OnBallPlaced);
            eventsListenersService.RemoveOrdinaryEventListener(BallManager.EntityPrimalStatusChanged);
            BallManager.EntityObjectDestroyed.RemoveListener(OnBallDestroyed);
        }

        private void OnBallMovingFromActivatedPlatformStarted(Vector2Int nextBallPosition)
        {
            PlatformsManager.ChangePlatformCheckingStatus(BallManager.GetObjectCheckableInfo(), CheckingStatus.Ended);
        }

        private void OnBallMovingFromRotatorStarted(Vector2Int nextBallPosition)
        {
            StartCoroutine(RotatorsManager.BeginRotatorHidingIteratively(BallManager.GetObjectCheckableInfo()));
        }

        private void OnBallMovingToPositionStarted(Vector2Int nextBallPosition, (Action<Vector2Int> output, Action<Vector2Int> input) ballMovingActions)
        {
            ballMovingActions.output?.Invoke(BallManager.GetObjectCheckableInfo());
            ballMovingActions.input?.Invoke(nextBallPosition);
        }

        private void OnBallPlaced(Vector2Int ballPosition)
        {
            AttachToEntity(BallManager.Entity);
            PlatformsManager.RemoveFreePlatform(ballPosition);
        }

        private void OnBallRotationsDataApplied(IDictionary<Vector2Int, ISet<Vector2Int>> activatedPlatformsPositionsNearReservoirs)
        {
            ISet<Vector2Int> activatedPlatformsPositions = activatedPlatformsPositionsNearReservoirs[ReservoirManager.GetObjectCheckableInfo()];

            StartCoroutine(PlatformsManager.ActivatePlatformsWithEntityPrimalStatusTogglingIteratively(activatedPlatformsPositions));
        }

        private void OnBallRotationStarted()
        {
            RotatorsManager.BeginRotatorRotation(BallManager.GetObjectCheckableInfo());
        }

        private void OnFieldUnsuccessfullyFocused()
        {
            StartCoroutine(PreBeginFieldFailingFinalizationIteratively(((Action)(() => EntityPreBanned.Invoke()), () => EntityBanned.Invoke())));
        }

        private void OnFocusedPlatformsConfined(MoveDirection nextBallMoveDirection)
        {
            Vector2Int ballPosition = BallManager.GetObjectCheckableInfo();

            eventsListenersService.AddUnsubscribingEventListener(() => OnIdlyFocusedPlatformsDefocused(nextBallMoveDirection),
                new EventsWithUnsubscribingListeners(PlatformsManager.IdlyFocusedPlatformsDefocused, PlatformsManager.Destroyed));
            PrepareFieldUpdatingOnBallMovingToPosition((ballPosition, positionService.MovePosition(ballPosition, nextBallMoveDirection)));
            StartCoroutine(EffectFieldFocusingIteratively(true));
            BallManager.PrepareBallForMoving(nextBallMoveDirection);
        }

        private void OnFocusedPlatformsReadyForConfining(Action<bool?> focusedPlatformsConfiningAgreedIndicatorAppointer)
        {
            focusedPlatformsConfiningAgreedIndicatorAppointer(EntityInfo.FlowingInfo.TraitsInfo.TryModify(FieldFlowingStatus.Reaction));
        }

        private void OnIdlyFocusedPlatformsDefocused(MoveDirection nextBallMoveDirection)
        {
            BallManager.BeginBallMoving(nextBallMoveDirection);
        }

        private void OnPlatformAccepted()
        {
            EntityAccepted.Invoke(((ICheckableGeneratedFieldObjectManager<ISet<CardinalPoint>>)BallManager).GetObjectCheckableInfo());
        }

        private void OnPlatformsActivated()
        {
            FieldInitialized.Invoke();
        }

        private void OnPlatformsDestroyed()
        {
            Destroy(Entity);
            EndFieldFinalization(FieldFinalizationType.Complete);
        }

        private void OnPlatformsLaunched()
        {
            BallManager.GenerateBall(PlatformsManager.FreePlatforms);
        }

        private void OnPlatformsManagerDestroyed()
        {
            void RemovePlatformsConfiningEventsListeners()
            {
                PlatformsManager.FocusedPlatformsConfining.Confined.RemoveListener(OnFocusedPlatformsConfined);
                PlatformsManager.FocusedPlatformsConfining.ReadyForConfining.RemoveListener(OnFocusedPlatformsReadyForConfining);
            }

            PlatformsManager.Destroyed.RemoveListener(OnPlatformsManagerDestroyed);
            PlatformsManager.EntityLaunched.RemoveListener(OnPlatformsLaunched);
            PlatformsManager.EntityPlaced.RemoveListener(OnPlatformsPlaced);
            eventsListenersService.RemoveOrdinaryEventListener(PlatformsManager.EntityPrimalStatusChanged);
            PlatformsManager.EntityAccepted.RemoveListener(OnPlatformAccepted);
            PlatformsManager.EntityActivated.RemoveListener(OnPlatformsActivated);
            PlatformsManager.PlatformsDestroyed.RemoveListener(OnPlatformsDestroyed);
            RemovePlatformsConfiningEventsListeners();
        }

        private void OnPlatformsPlaced()
        {
            AttachToEntity(PlatformsManager.Entity);
        }

        private void OnReservoirPlaced(Vector2Int reservoirPosition)
        {
            AttachToEntity(ReservoirManager.Entity);
            PlatformsManager.RemoveFreePlatform(reservoirPosition);
            PlatformsManager.BeginPlatformEnlarging(reservoirPosition);
        }

        private void OnReservoirManagerDestroyed()
        {
            ReservoirManager.Destroyed.RemoveListener(OnReservoirManagerDestroyed);
            ReservoirManager.EntityPlaced.RemoveListener(OnReservoirPlaced);
            eventsListenersService.RemoveOrdinaryEventListener(ReservoirManager.EntityPrimalStatusChanged);
        }

        private void OnRotatorsLaunched(IDictionary<BallRotationsDataKey, RotationsData> ballRotationsDataDictionary)
        {
            RotationsData ballRotationsData = fieldEntityBindingService.GetBallRotationsDataByReservoirInfo(ballRotationsDataDictionary,
                new ReservoirInfoKey(ReservoirManager.GetObjectCheckableInfo(),
                ((ICheckableGeneratedFieldObjectManager<SubstanceColorType>)ReservoirManager).GetObjectCheckableInfo())).Value;

            StartCoroutine(BallManager.ApplyBallRotationsDataIteratively(ballRotationsData));
        }

        private void OnRotatorsManagerDestroyed()
        {
            RotatorsManager.Destroyed.RemoveListener(OnRotatorsManagerDestroyed);
            RotatorsManager.EntityPlaced.RemoveListener(OnRotatorsPlaced);
            eventsListenersService.RemoveOrdinaryEventListener(RotatorsManager.EntityPrimalStatusChanged);
        }

        private void OnRotatorsPlaced()
        {
            AttachToEntity(RotatorsManager.Entity);
        }

        protected override void OnDestroy()
        {
            FieldUnsuccessfullyFocused.RemoveListener(OnFieldUnsuccessfullyFocused);
            base.OnDestroy();
        }

        private enum FieldPositionType
        {
            ActivatedPlatform,

            IdlePlatform,

            Rotator
        }
    }
}

namespace GameScene.Managers.Field.Assignment
{
    public class FieldIndividualAssignment
    {
        public FieldIndividualAssignment(IEnumerator routine)
        {
            IsFulfilled = false;
            Routine = FulfillIteratively(routine);
        }

        public bool IsFulfilled { get; private set; }

        public IEnumerator Routine { get; private set; }

        private IEnumerator FulfillIteratively(IEnumerator routine)
        {
            yield return routine;

            IsFulfilled = true;
        }
    }
}

namespace GameScene.Managers.Field.Data
{
    public class FieldPrimalStatusData : BaseEntityRoutinelyLockableStatusData<FieldEntityPrimalStatusData, FieldEntityStatusChangingRoutinesExecutor>,
        IIterativelyGettableEntityStatusData<FieldEntityPrimalStatusData>, IWaitedlyIterativelySettableEntityStatusData<FieldEntityPrimalStatusData>
    {
        public FieldPrimalStatusData() : base(new FieldEntityPrimalStatusData()) { }

        public IEnumerator GetStatusIteratively(Action<FieldEntityPrimalStatusData> statusExtractedAction)
        {
            yield return routinesExecutor.ExecuteRoutineIteratively(WaitToGetStatusIteratively(statusExtractedAction));
        }

        public IEnumerator SetStatusIteratively(FieldEntityPrimalStatusData value, Func<bool> waitingConditionFunction,
            EntityStatusChangingActions<FieldEntityPrimalStatusData> statusChangingActions)
        {
            yield return routinesExecutor.ExecuteRoutineIteratively(WaitToSetStatusIteratively(value, statusChangingActions), waitingConditionFunction);
        }
    }
}

namespace GameScene.Managers.Field.Enums
{
    public enum FieldCheckingPhase
    {
        Early,

        Late
    }

    public enum FieldFinalizationType
    {
        Complete,

        Failing,

        Interval,

        Regular
    }

    public enum FieldFlowingStatus
    {
        Decision,

        None,

        Reaction,

        Selection
    }

    public enum FieldFlowingVerdict
    {
        Abortion,

        Continuation,

        None,

        Shutting
    }

    public enum FieldNonSequentalActionType
    {
        CompleteFinalization,

        PossibleFailingFinalization,

        Suspending
    }
}

namespace GameScene.Managers.Field.Events
{
    public class FieldAcceptedEvent : UnityEvent<ISet<CardinalPoint>> { }

    public class FieldFinalizedEvent : UnityEvent<FieldFinalizationType> { }

    public class FieldPreCheckedEvent : UnityEvent<CardinalPoint, SubstanceColorType> { }

    public class FieldPrimalStatusChangedEvent : UnityEvent<FieldEntityPrimalStatusData> { }
}

namespace GameScene.Managers.Field.Info
{
    public abstract class BaseChildFieldFlowingInfo : IResetableChildFieldFlowingInfo
    {
        public BaseChildFieldFlowingInfo()
        {
            Reset();
        }

        public abstract void Reset();
    }

    public class FieldFlowingAssignmentsInfo : BaseChildFieldFlowingInfo, INonReportedlyModifiableChildFieldFlowingInfo<FieldIndividualAssignment>
    {
        public FieldIndividualAssignment IndividualAssignment { get; private set; }

        public void Modify(FieldIndividualAssignment element)
        {
            if (element != null)
                IndividualAssignment = element;
        }

        public override void Reset()
        {
            IndividualAssignment = null;
        }
    }

    public class FieldFlowingTraitsInfo : BaseChildFieldFlowingInfo, IReportedlyModifiableChildFieldFlowingInfo<FieldFlowingStatus>,
        IReportedlyModifiableChildFieldFlowingInfo<FieldFlowingVerdict>
    {
        public FieldFlowingStatus Status { get; private set; }

        public FieldFlowingVerdict Verdict { get; private set; }

        public override void Reset()
        {
            Status = FieldFlowingStatus.None;
            Verdict = FieldFlowingVerdict.None;
        }

        public bool TryModify(FieldFlowingStatus element)
        {
            if (((Status == FieldFlowingStatus.None) && (element == FieldFlowingStatus.Selection)) || ((Status == FieldFlowingStatus.Selection) &&
                (element == FieldFlowingStatus.Reaction)) || ((Status == FieldFlowingStatus.Reaction) && (element == FieldFlowingStatus.Decision)) ||
                ((Status == FieldFlowingStatus.Decision) && ((element == FieldFlowingStatus.None) || (element == FieldFlowingStatus.Selection))))
            {
                Status = element;

                return true;
            }
            else
                return false;
        }

        public bool TryModify(FieldFlowingVerdict element)
        {
            if (((Verdict == FieldFlowingVerdict.None) && (element != FieldFlowingVerdict.None)) || ((Verdict == FieldFlowingVerdict.Abortion) &&
                (element == FieldFlowingVerdict.Shutting)) || ((Verdict == FieldFlowingVerdict.Continuation) && (element == FieldFlowingVerdict.Abortion)) ||
                ((Verdict != FieldFlowingVerdict.None) && (element == FieldFlowingVerdict.None)))
            {
                Verdict = element;

                return true;
            }
            else
                return false;
        }
    }

    public class FieldFlowingInfo
    {
        public FieldFlowingInfo()
        {
            AssignmentsInfo = new FieldFlowingAssignmentsInfo();
            TraitsInfo = new FieldFlowingTraitsInfo();
        }

        public FieldFlowingAssignmentsInfo AssignmentsInfo { get; private set; }

        public FieldFlowingTraitsInfo TraitsInfo { get; private set; }
    }

    public class FieldInfo : BaseFieldEntityInfo<FieldPrimalStatusData>
    {
        public FieldInfo() : base(new FieldPrimalStatusData())
        {
            FlowingInfo = new FieldFlowingInfo();
        }

        public FieldFlowingInfo FlowingInfo { get; private set; }

        public GameObject Field { get; set; }
    }
}

namespace GameScene.Managers.Field.Info.Interfaces
{
    public interface INonReportedlyModifiableChildFieldFlowingInfo<T>
    {
        void Modify(T element);
    }

    public interface IReportedlyModifiableChildFieldFlowingInfo<T> where T : Enum
    {
        bool TryModify(T element);
    }

    public interface IResetableChildFieldFlowingInfo
    {
        void Reset();
    }
}