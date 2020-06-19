using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.Platform;
using GameScene.Behaviours.Platform.Enums;
using GameScene.Behaviours.Platform.Events;
using GameScene.Behaviours.Platform.Info;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Field.Settings;
using GameScene.Managers.Platforms.Data;
using GameScene.Managers.Platforms.Descriptions;
using GameScene.Managers.Platforms.Enums;
using GameScene.Managers.Platforms.Events;
using GameScene.Managers.Platforms.Info;
using GameScene.Managers.Platforms.Settings;
using GameScene.Services.Game.Descriptions;
using GameScene.Services.Game.Info;
using GameScene.Services.Platform;
using GameScene.Services.Platform.Events;
using GameScene.Services.Subscription.Events;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Managers.Platforms
{
    public class PlatformsManager : BaseChildFieldEntityCreatingManager<PlatformBehaviour, PlatformSettings, PlatformsInfo>, IAcceptableFieldEntityManager<UnityEvent>,
        IActivatableEntityManager, INotifiedlyUnbannableFieldEntityManager
    {
        private readonly PlatformsRadianceColorsToner platformsRadianceColorsToner;

        [SerializeField]
        private int dimensionPlatformsCount;

        private NeighboringPlatformsFinder neighboringPlatformsFinder;

        private PositionService positionService;

        public PlatformsManager()
        {
            platformsRadianceColorsToner = new PlatformsRadianceColorsToner();
            EntityAccepted = new UnityEvent();
            EntityActivated = new UnityEvent();
            EntityUnbanned = new UnityEvent();
            IdlyFocusedPlatformsDefocused = new UnityEvent();
            PlatformBanned = new UnityEvent();
            PlatformEnlarged = new UnityEvent();
            PlatformLowered = new UnityEvent();
            PlatformPostRejected = new UnityEvent();
            PlatformPostFocused = new UnityEvent();
            PlatformsDestroyed = new UnityEvent();
            FocusedPlatformsConfining = new FocusedPlatformsConfiningEvents();
        }

        public NeighboringPlatformsPreferringInfo NeighboringPlatformsPreferringInfo
        {
            set
            {
                EntityInfo.NeighboringPlatformsPreferring = value;
            }
        }

        public override GameObject Entity
        {
            get
            {
                return EntityInfo.Platforms;
            }
        }

        public IDictionary<Vector2Int, GameObject> FreePlatforms
        {
            get
            {
                return EntityInfo.FreeObjects;
            }
        }

        public UnityEvent EntityAccepted { get; private set; }

        public UnityEvent EntityActivated { get; private set; }

        public UnityEvent EntityUnbanned { get; private set; }

        public UnityEvent IdlyFocusedPlatformsDefocused { get; private set; }

        public UnityEvent PlatformBanned { get; private set; }

        public UnityEvent PlatformEnlarged { get; private set; }

        public UnityEvent PlatformLowered { get; private set; }

        public UnityEvent PlatformPostFocused { get; private set; }

        public UnityEvent PlatformPostRejected { get; private set; }

        public UnityEvent PlatformsDestroyed { get; private set; }

        public FocusedPlatformsConfiningEvents FocusedPlatformsConfining { get; private set; }

        private void AddPlatformHoldingAnimationPassingEventsListeners(PlatformBehaviour platformBehaviour)
        {
            AnimationPassingEvents<PlatformHoldingAnimationExitedEvent> platformHoldingAnimationPassingEvents = platformBehaviour.HoldingAnimation;

            platformHoldingAnimationPassingEvents.Started.AddListener(OnPlatformHoldingAnimationStarted);
            platformHoldingAnimationPassingEvents.Ended.AddListener(OnPlatformHoldingAnimationEnded);
        }

        private void ConfineFocusedPlatforms(GameObject chosenPlatform)
        {
            FocusedPlatformsData focusedPlatformsData = EntityInfo.FocusedPlatformsData;
            MoveDirection chosenPlatformMoveDirection = focusedPlatformsData.Unchosen[chosenPlatform];

            focusedPlatformsData.StateDescription.Value = FocusedPlatformsState.Confined;
            focusedPlatformsData.ChosenPlatform = chosenPlatform;
            FocusedPlatformsConfining.Confined.Invoke(chosenPlatformMoveDirection);
        }

        private void CreatePlatform(int rowIndex, int columnIndex)
        {
            GameObject platform = Instantiate(entityObjectSettings.Prefab, entityObjectSettings.Displacement * new Vector3(rowIndex, 0, columnIndex), Quaternion.identity);
            Vector2Int platformPosition = new Vector2Int(rowIndex, columnIndex);

            platform.name = GetObjectNameOnPosition(entityObjectSettings.InstanceName, platformPosition);

            AttachToEntity(platform);

            platform.GetComponent<Animator>().runtimeAnimatorController = entityObjectSettings.AnimatorController;

            platform.AddComponent<BoxCollider>();

            BoxCollider boxCollider = platform.GetComponent<BoxCollider>();
            PlatformBoxColliderSettings platformBoxColliderSettings = entityObjectSettings.BoxColliderSettings;

            boxCollider.isTrigger = true;
            boxCollider.center = platformBoxColliderSettings.Center;
            boxCollider.size = platformBoxColliderSettings.Size;

            CreateAndInitiallySetupObjectBehaviour(platform, platformPosition, new PlatformInfo(entityObjectSettings.PartSettings.IlluminatorSettings.ColorsSettings.Neutral,
                new PlatformRoofSetupInfo(platform, entityObjectSettings).Illuminator));
        }

        private void RemovePlatformHoldingAnimationPassingEventsListeners(PlatformBehaviour platformBehaviour)
        {
            AnimationPassingEvents<PlatformHoldingAnimationExitedEvent> platformHoldingAnimationPassingEvents = platformBehaviour.HoldingAnimation;

            platformHoldingAnimationPassingEvents.Started.RemoveListener(OnPlatformHoldingAnimationStarted);
            platformHoldingAnimationPassingEvents.Ended.RemoveListener(OnPlatformHoldingAnimationEnded);
        }

        private IEnumerator DefocusChosenFocusedPlatformIteratively(bool isAwaitable)
        {
            FocusedPlatformsData focusedPlatformsData = EntityInfo.FocusedPlatformsData;
            GameObject chosenFocusedPlatform = focusedPlatformsData.ChosenPlatform;

            if (!EntityInfo.ActivatedPlatforms.Contains(chosenFocusedPlatform))
            {
                chosenFocusedPlatform.GetComponent<PlatformBehaviour>().Defocus();
                focusedPlatformsData.ChosenPlatform = null;

                if (isAwaitable)
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Focus.Defocusing);
            }
        }

        private IEnumerator OpenPlatformIteratively(Vector2Int platformPosition, Color platformRadianceColor)
        {
            bool isIdleOpeningPlatform = default;
            GameObject openingPlatform = FreePlatforms.ContainsKey(platformPosition) ? FreePlatforms[platformPosition] : EntityInfo.OccupiedPlatforms[platformPosition];

            yield return openingPlatform.GetComponent<PlatformBehaviour>()
                .OpenIteratively(isIdleOpeningPlatformParameter => isIdleOpeningPlatform = isIdleOpeningPlatformParameter, platformRadianceColor);

            if (isIdleOpeningPlatform)
                EntityInfo.IdlyOpenedPlatforms.Enqueue(openingPlatform);
        }

        private IEnumerator PostProcessPlatformsAnimatedlyLaunchedIteratively()
        {
            if (FreePlatforms.Count == dimensionPlatformsCount * dimensionPlatformsCount)
            {
                yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));

                EntityLaunched.Invoke();
            };
        }

        private IEnumerator RunPlatformsIteratively(ICollection<GameObject> platforms)
        {
            int platformIndex = 0;

            foreach (GameObject platform in platforms)
            {
                if (platform.GetComponent<PlatformBehaviour>().TryRun() && (platformIndex++ < platforms.Count - 1))
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Running);
            }
        }

        private IEnumerator UnbanPlatformsIteratively(ISet<Vector2Int> bannedPlatformsPositions)
        {
            PlatformBehaviour bannedPlatformBehaviour;

            foreach (Vector2Int bannedPlatformPosition in bannedPlatformsPositions)
            {
                bannedPlatformBehaviour = FreePlatforms[bannedPlatformPosition].GetComponent<PlatformBehaviour>();
                eventsListenersService.AddUnsubscribingEventListener((bannedPlatformParameter) => bannedPlatformsPositions.Remove(bannedPlatformPosition),
                    new EventsWithUnsubscribingListeners<GameObject>(bannedPlatformBehaviour.AnimatedlyUnbanned, bannedPlatformBehaviour.Destroyed));
                bannedPlatformBehaviour.BeginUnbanning();
            }

            yield return new WaitUntil(() => bannedPlatformsPositions.Count == 0);
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            neighboringPlatformsFinder = SharedSceneServicesLocator.GetService<NeighboringPlatformsFinder>();
            positionService = SharedSceneServicesLocator.GetService<PositionService>();
        }

        protected override void RemoveObjectBehaviourEventsListeners(PlatformBehaviour objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            objectBehaviour.AnimatedlyAccepted.RemoveListener(OnPlatformAnimatedlyAccepted);
            objectBehaviour.AnimatedlyActivated.RemoveListener(OnPlatformAnimatedlyActivated);
            objectBehaviour.AnimatedlyBanned.RemoveListener(OnPlatformAnimatedlyBanned);
            objectBehaviour.AnimatedlyDeactivated.RemoveListener(OnPlatformAnimatedlyDeactivated);
            objectBehaviour.AnimatedlyUnbanned.RemoveListener(OnPlatformAnimatedlyUnbanned);
            objectBehaviour.Chosen.RemoveListener(OnPlatformChosen);
            RemovePlatformHoldingAnimationPassingEventsListeners(objectBehaviour);
        }

        protected override void ResumeEntityInternally()
        {
            base.ResumeEntityInternally();
            RetainObjects(EntityInfo.OccupiedPlatforms.Values, false);
        }

        protected override void SuspendEntityInternally()
        {
            base.SuspendEntityInternally();
            RetainObjects(EntityInfo.OccupiedPlatforms.Values, true);
        }

        protected override PlatformBehaviour CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            PlatformBehaviour platformBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            platformBehaviour.Setup((PlatformInfo)parameters[0]);
            platformBehaviour.AnimatedlyAccepted.AddListener(OnPlatformAnimatedlyAccepted);
            platformBehaviour.AnimatedlyActivated.AddListener(OnPlatformAnimatedlyActivated);
            platformBehaviour.AnimatedlyBanned.AddListener(OnPlatformAnimatedlyBanned);
            platformBehaviour.AnimatedlyDeactivated.AddListener(OnPlatformAnimatedlyDeactivated);
            platformBehaviour.AnimatedlyUnbanned.AddListener(OnPlatformAnimatedlyUnbanned);
            platformBehaviour.Chosen.AddListener(OnPlatformChosen);
            AddPlatformHoldingAnimationPassingEventsListeners(platformBehaviour);

            return platformBehaviour;
        }

        protected override IEnumerator ProcessCreatedObjectBlockingAnimationEndedIteratively()
        {
            yield return base.ProcessCreatedObjectBlockingAnimationEndedIteratively();

            PlatformPostFocused.Invoke();
        }

        public void AddFreePlatform(Vector2Int occupiedPlatformPosition)
        {
            FreePlatforms.Add(occupiedPlatformPosition, EntityInfo.OccupiedPlatforms[occupiedPlatformPosition]);
            EntityInfo.OccupiedPlatforms.Remove(occupiedPlatformPosition);
        }

        public void BeginPlatformBanning(Vector2Int platformPosition)
        {
            EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().BeginBanning();
        }

        public void BeginPlatformsUnbanning()
        {
            foreach (GameObject bannedPlatform in EntityInfo.BannedPlatforms)
                bannedPlatform.GetComponent<PlatformBehaviour>().BeginUnbanning();
        }

        public void BeginPlatformEnlarging(Vector2Int platformPosition)
        {
            EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().BeginEnlarging();
        }

        public void BeginPlatformLowering(Vector2Int platformPosition)
        {
            EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().BeginLowering();
        }

        public void ChangePlatformCheckingStatus(Vector2Int platformPosition, CheckingStatus checkingStatus)
        {
            GameObject platform = (checkingStatus == CheckingStatus.Begun) ? FreePlatforms[platformPosition] : EntityInfo.OccupiedPlatforms[platformPosition];

            platform.GetComponent<PlatformBehaviour>().ChangeCheckingStatus(checkingStatus);

            if (checkingStatus == CheckingStatus.Begun)
                EntityInfo.FocusedPlatformsData.ChosenPlatform = null;
        }

        public void RemoveFreePlatform(Vector2Int freePlatformPosition)
        {
            EntityInfo.OccupiedPlatforms.Add(freePlatformPosition, FreePlatforms[freePlatformPosition]);
            FreePlatforms.Remove(freePlatformPosition);
        }

        public bool AreLiberatablePlatformsExist(LiberatablePlatformsType liberatablePlatformsType)
        {
            switch (liberatablePlatformsType)
            {
                case LiberatablePlatformsType.Banned:
                    return EntityInfo.BannedPlatforms.Count > 0;
                default:
                    return EntityInfo.FocusedPlatformsData.Unchosen.Positions.Count > 0;
            }
        }

        public bool IsActivatedPlatformPosition(Vector2Int platformPosition)
        {
            GameObject platform = FreePlatforms.ContainsKey(platformPosition) ? FreePlatforms[platformPosition] : EntityInfo.OccupiedPlatforms[platformPosition];

            return EntityInfo.ActivatedPlatforms.Contains(platform);
        }

        public int GetDimensionPlatformsCount(bool isHalfCount = false)
        {
            return isHalfCount ? dimensionPlatformsCount / 2 : dimensionPlatformsCount;
        }

        public IEnumerator ActivatePlatformsIteratively(ISet<Vector2Int> activatedPlatformsPositions)
        {
            GameObject platform;

            foreach (Vector2Int activatedPlatformPosition in activatedPlatformsPositions)
            {
                platform = FreePlatforms[activatedPlatformPosition];
                platform.GetComponent<PlatformBehaviour>().BeginActivation();

                yield return new WaitUntil(() => EntityInfo.ActivatedPlatforms.Contains(platform));
            }

            EntityActivated.Invoke();
        }

        public IEnumerator ActivatePlatformsWithEntityPrimalStatusTogglingIteratively(ISet<Vector2Int> activatedPlatformsPositions)
        {
            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(ActivatePlatformsIteratively(activatedPlatformsPositions),
                new EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo());
        }

        public IEnumerator BeginPlatformsHidingIteratively()
        {
            int freePlatformIndex = 0;

            foreach (GameObject freePlatform in FreePlatforms.Values)
            {
                freePlatform.GetComponent<PlatformBehaviour>().BeginHiding();

                if (freePlatformIndex++ < FreePlatforms.Count)
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Destroying);
            }
        }

        public IEnumerator CheckPlatformIteratively(Vector2Int platformPosition, CheckingAction checkingAction)
        {
            IEnumerator checkingRoutine = EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().CheckIteratively(checkingAction);

            if (checkingAction == CheckingAction.Accept)
                checkingRoutine = PerformRoutineWithEntityPrimalStatusTogglingIteratively(checkingRoutine,
                    new EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo());

            yield return checkingRoutine;
        }

        public IEnumerator CloseIdlyOpenedPlatformsIteratively()
        {
            GameObject idlyOpenedPlatform;
            Queue<GameObject> idlyOpenedPlatforms = EntityInfo.IdlyOpenedPlatforms;

            while (idlyOpenedPlatforms.Count > 0)
            {
                idlyOpenedPlatform = idlyOpenedPlatforms.Dequeue();

                yield return idlyOpenedPlatform.GetComponent<PlatformBehaviour>().CloseIteratively();
            }
        }

        public IEnumerator CreatePlatformsIteratively()
        {
            int dimensionHalfPlatformsCount = GetDimensionPlatformsCount(true);

            EntityInfo.Platforms = new GameObject(entityObjectSettings.OwnerInstanceName);
            EntityPlaced.Invoke();

            for (int i = -dimensionHalfPlatformsCount; i <= dimensionHalfPlatformsCount; i++)
            {
                for (int j = -dimensionHalfPlatformsCount; j <= dimensionHalfPlatformsCount; j++)
                {
                    CreatePlatform(i, j);

                    if ((i < dimensionHalfPlatformsCount) && (j < dimensionHalfPlatformsCount))
                        yield return new WaitForSeconds(entityObjectSettings.Delays.Creating);
                }
            }
        }

        public IEnumerator DeactivatePlatformsIteratively()
        {
            GameObject activatedPlatform;

            while (EntityInfo.ActivatedPlatforms.Count > 0)
            {
                activatedPlatform = EntityInfo.ActivatedPlatforms.First();

                yield return activatedPlatform.GetComponent<PlatformBehaviour>().BeginDeactivationIteratively(entityObjectSettings.PreparingDeactivationLoopsCount);

                yield return new WaitUntil(() => !EntityInfo.ActivatedPlatforms.Contains(activatedPlatform));
            }
        }

        public IEnumerator DefocusIdlyFocusedPlatformsIteratively()
        {
            GameObject unchosenFocusedPlatform;
            FocusedPlatformsData focusedPlatformsData = EntityInfo.FocusedPlatformsData;
            UnchosenFocusedPlatformsData unchosenFocusedPlatformsData = focusedPlatformsData.Unchosen;
            Func<bool> focusedPlatformsExistConditionFunction = () => AreLiberatablePlatformsExist(LiberatablePlatformsType.Focused);

            if (focusedPlatformsData.ChosenPlatform != null)
                yield return DefocusChosenFocusedPlatformIteratively(focusedPlatformsExistConditionFunction());
            else
                focusedPlatformsData.StateDescription.Value = FocusedPlatformsState.Confined;

            while (focusedPlatformsExistConditionFunction())
            {
                unchosenFocusedPlatform = FreePlatforms[unchosenFocusedPlatformsData.Positions.First()];
                unchosenFocusedPlatform.GetComponent<PlatformBehaviour>().Defocus();
                unchosenFocusedPlatformsData.Remove(unchosenFocusedPlatform);

                if (focusedPlatformsExistConditionFunction())
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Focus.Defocusing);
            }

            IdlyFocusedPlatformsDefocused.Invoke();
        }

        public IEnumerator DefocusIdlyFocusedPlatformsWithEntityPrimalStatusTogglingIteratively()
        {
            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(DefocusIdlyFocusedPlatformsIteratively(),
                new EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo(entityFinalizationStatusAfterAvailabilityEnablingExtractor:
                () => FieldEntityFinalizationStatus.NotReadyForFinalization));
        }

        public IEnumerator FailPlatformIteratively(Vector2Int platformPosition, FailingType failingType)
        {
            yield return EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().FailIteratively(failingType);
        }

        public IEnumerator FailPlatformWithEntityPrimalStatusTogglingIteratively(Vector2Int platformPosition, FailingType failingType)
        {
            BaseEntityPrimalStatusTogglingRoutinesInfo GetTogglingRoutineInfo()
            {
                switch (failingType)
                {
                    case FailingType.Input:
                        return new EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo(true);
                    default:
                        return new EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo(true);
                }
            }

            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(FailPlatformIteratively(platformPosition, failingType), GetTogglingRoutineInfo());
        }

        public IEnumerator FocusNeighboringPlatformsIteratively(Vector2Int platformPosition, Action<bool> isSuccessfulIndicatorExtractor)
        {
            IEnumerator ExtractNeighboringPlatformsMoveDirectionsItemsIteratively((NeighboringPlatformsPreferringInfo preferringInfo, Action<PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>?> moveDirectionsItemsDescriptionExtractor) neighboringPlatformsDetails)
            {
                bool IsNeighboringPlatformCanBePotentiallyFocused(Vector2Int neighboringPlatformPosition)
                {
                    bool result = FreePlatforms.ContainsKey(neighboringPlatformPosition);

                    return IsActivatedPlatformPosition(neighboringPlatformPosition) ? result && neighboringPlatformsDetails.preferringInfo.ActivationalFocusingConditionFunction() : result;
                }

                yield return neighboringPlatformsDetails.preferringInfo.DataPickingRoutineExtractor(platformPosition,
                    neighboringPlatformsFinder.GetNeighboringPlatformsMoveDirectionsItems(platformPosition, GetDimensionPlatformsCount(true), IsNeighboringPlatformCanBePotentiallyFocused),
                    neighboringPlatformsDetails.moveDirectionsItemsDescriptionExtractor);
            }

            IEnumerator FocusNeighboringPlatformsInternallyIteratively(PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection> neighboringPlatformsMoveDirectionsItemsDetails)
            {
                IEnumerator FocusNeighboringPlatformIteratively(KeyValuePair<Vector2Int, MoveDirection> neighboringPlatformMoveDirectionItem)
                {
                    Vector2Int neighboringPlatformPosition = neighboringPlatformMoveDirectionItem.Key;
                    GameObject neighboringPlatform = FreePlatforms[neighboringPlatformPosition];

                    if (AreLiberatablePlatformsExist(LiberatablePlatformsType.Focused))
                        yield return new WaitForSeconds(entityObjectSettings.Delays.Focus.Focusing);

                    neighboringPlatform.GetComponent<PlatformBehaviour>().Focus();
                    EntityInfo.FocusedPlatformsData.Unchosen.Add(neighboringPlatform, neighboringPlatformPosition, neighboringPlatformMoveDirectionItem.Value);
                }

                Func<Vector2Int, bool> neighboringPlatformFocusingConditionFunction;
                bool isOptionalFocusing = !neighboringPlatformsMoveDirectionsItemsDetails.IsFiltered;
                IDictionary<Vector2Int, MoveDirection> neighboringPlatformsMoveDirectionsItems = neighboringPlatformsMoveDirectionsItemsDetails.Value;
                ISet<GameObject> bannedPlatforms = EntityInfo.BannedPlatforms;

                if (!isOptionalFocusing)
                    yield return UnbanPlatformsIteratively(new HashSet<Vector2Int>(neighboringPlatformsMoveDirectionsItems.Keys.Where(neighboringPlatformPositionParameter => bannedPlatforms.Contains(FreePlatforms[neighboringPlatformPositionParameter]))));

                neighboringPlatformFocusingConditionFunction = (neighboringPlatformPositionParameter) => isOptionalFocusing ? !bannedPlatforms.Contains(FreePlatforms[neighboringPlatformPositionParameter]) : true;

                foreach (KeyValuePair<Vector2Int, MoveDirection> neighboringPlatformMoveDirectionsItem in neighboringPlatformsMoveDirectionsItems)
                {
                    if (neighboringPlatformFocusingConditionFunction(neighboringPlatformMoveDirectionsItem.Key))
                        yield return FocusNeighboringPlatformIteratively(neighboringPlatformMoveDirectionsItem);
                }
            }

            void SummarizeNeighboringPlatformsFocusing()
            {
                bool isSuccessful;

                if (isSuccessful = AreLiberatablePlatformsExist(LiberatablePlatformsType.Focused))
                    EntityInfo.FocusedPlatformsData.StateDescription.Value = FocusedPlatformsState.Unconfined;

                isSuccessfulIndicatorExtractor.Invoke(isSuccessful);
            }

            PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>? neighboringPlatformsMoveDirectionsItemsDescription = null;

            yield return ExtractNeighboringPlatformsMoveDirectionsItemsIteratively((EntityInfo.NeighboringPlatformsPreferring.Value, (neighboringPlatformsMoveDirectionsItemsDescriptionParameter) => neighboringPlatformsMoveDirectionsItemsDescription = neighboringPlatformsMoveDirectionsItemsDescriptionParameter));

            if (neighboringPlatformsMoveDirectionsItemsDescription != null)
                yield return FocusNeighboringPlatformsInternallyIteratively(neighboringPlatformsMoveDirectionsItemsDescription.Value);

            SummarizeNeighboringPlatformsFocusing();
        }

        public IEnumerator FocusNeighboringPlatformsWithEntityPrimalStatusTogglingIteratively(Vector2Int platformPosition, Action<bool> isSuccessfulIndicatorExternalExtractor)
        {
            bool isSuccessful = default;
            Action<bool> isSuccessfulIndicatorInternalExtractor = (isSuccessfulIndicatorParameter) =>
            {
                isSuccessful = isSuccessfulIndicatorParameter;
                isSuccessfulIndicatorExternalExtractor(isSuccessful);
            };

            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(FocusNeighboringPlatformsIteratively(platformPosition, isSuccessfulIndicatorInternalExtractor),
                new EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo(entityFinalizationStatusAfterAvailabilityEnablingExtractor:
                () => isSuccessful ? FieldEntityFinalizationStatus.ReadyForFinalization : FieldEntityFinalizationStatus.NotReadyForFinalization));
        }

        public IEnumerator OpenPlatformsIteratively(PathToReservoirOpeningInfo pathToReservoirOpeningInfo)
        {
            IList<MoveDirection> moveDirections = pathToReservoirOpeningInfo.MoveDirections;
            Vector2Int openingPlatformPosition = pathToReservoirOpeningInfo.StartPathPoint;

            platformsRadianceColorsToner.SetupToning(entityObjectSettings.PartSettings.IlluminatorSettings.ColorsSettings.NonNeutral,
                pathToReservoirOpeningInfo.MoveDirections.Count + 1);

            for (int i = 0; i <= moveDirections.Count; i++)
            {
                yield return OpenPlatformIteratively(openingPlatformPosition, platformsRadianceColorsToner.Tone(i));

                if (i != moveDirections.Count)
                    openingPlatformPosition = positionService.MovePosition(openingPlatformPosition, moveDirections[i]);
            }

            platformsRadianceColorsToner.ResetToning();
        }

        public IEnumerator OpenPlatformsWithEntityPrimalStatusTogglingIteratively(PathToReservoirOpeningInfo pathToReservoirOpeningInfo)
        {
            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(OpenPlatformsIteratively(pathToReservoirOpeningInfo),
                new EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo(true));
        }

        public override IEnumerator RunEntityIteratively()
        {
            yield return RunPlatformsIteratively(FreePlatforms.Values);

            yield return RunPlatformsIteratively(EntityInfo.OccupiedPlatforms.Values);
        }

        public IEnumerator RunEntityWithPrimalStatusTogglingIteratively()
        {
            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(RunEntityIteratively(), new EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo());
        }

        public IEnumerator SkipPlatformIteratively(Vector2Int platformPosition)
        {
            yield return EntityInfo.OccupiedPlatforms[platformPosition].GetComponent<PlatformBehaviour>().SkipIteratively();
        }

        private void OnPlatformAnimatedlyAccepted(GameObject acceptedPlatform)
        {
            EntityInfo.ActivatedPlatforms.Remove(acceptedPlatform);
            EntityAccepted.Invoke();
        }

        private void OnPlatformAnimatedlyActivated(GameObject activatedPlatform)
        {
            EntityInfo.ActivatedPlatforms.Add(activatedPlatform);
        }

        private void OnPlatformAnimatedlyBanned(GameObject bannedPlatform)
        {
            EntityInfo.BannedPlatforms.Add(bannedPlatform);
            PlatformBanned.Invoke();
        }

        private void OnPlatformAnimatedlyDeactivated(GameObject deactivatedPlatform)
        {
            EntityInfo.ActivatedPlatforms.Remove(deactivatedPlatform);
        }

        private void OnPlatformAnimatedlyUnbanned(GameObject unbannedPlatform)
        {
            ISet<GameObject> bannedPlatforms = EntityInfo.BannedPlatforms;

            bannedPlatforms.Remove(unbannedPlatform);

            if (!AreLiberatablePlatformsExist(LiberatablePlatformsType.Banned))
                EntityUnbanned.Invoke();
        }

        private void OnPlatformChosen(GameObject chosenPlatform)
        {
            IEnumerator ProcessPlatformChosenIteratively()
            {
                FocusedPlatformsStateDescription focusedPlatformsStateDescription = EntityInfo.FocusedPlatformsData.StateDescription;

                if ((focusedPlatformsStateDescription.Value == FocusedPlatformsState.Unconfined) && !focusedPlatformsStateDescription.IsAgreeingForChanging)
                {
                    bool? focusedPlatformsConfiningAgreedIndicator = null;

                    focusedPlatformsStateDescription.IsAgreeingForChanging = true;
                    FocusedPlatformsConfining.ReadyForConfining.Invoke(focusedPlatformsConfiningAgreedIndicatorParameter => focusedPlatformsConfiningAgreedIndicator = focusedPlatformsConfiningAgreedIndicatorParameter);

                    yield return new WaitUntil(() => focusedPlatformsConfiningAgreedIndicator != null);

                    focusedPlatformsStateDescription.IsAgreeingForChanging = false;

                    if (focusedPlatformsConfiningAgreedIndicator.Value)
                        ConfineFocusedPlatforms(chosenPlatform);
                }
            }

            StartCoroutine(ProcessPlatformChosenIteratively());
        }

        private void OnPlatformHoldingAnimationStarted()
        {
            StartCoroutine(EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable)));
        }

        private void OnPlatformHoldingAnimationEnded(PlatformHoldingAnimationType platformHoldingAnimationType)
        {
            IEnumerator ProcessPlatformHoldingAnimationEndedIteratively()
            {
                yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));

                switch (platformHoldingAnimationType)
                {
                    case PlatformHoldingAnimationType.GrowingDown:
                        PlatformLowered.Invoke();
                        break;
                    case PlatformHoldingAnimationType.GrowingUp:
                        PlatformEnlarged.Invoke();
                        break;
                    default:
                        PlatformPostRejected.Invoke();
                        break;
                }
            }

            StartCoroutine(ProcessPlatformHoldingAnimationEndedIteratively());
        }

        protected override void OnCreatedObjectAnimatedlyDisappeared(GameObject obj, Vector2Int objectPosition)
        {
            base.OnCreatedObjectAnimatedlyDisappeared(obj, objectPosition);

            if (FreePlatforms.Count == 0)
                PlatformsDestroyed.Invoke();
        }

        protected override void OnObjectAnimatedlyLaunched(GameObject obj, Vector2Int objectPosition)
        {
            base.OnObjectAnimatedlyLaunched(obj, objectPosition);
            StartCoroutine(PostProcessPlatformsAnimatedlyLaunchedIteratively());
        }

        private class PlatformRoofSetupInfo : EntityPartSetupInfo<PlatformRoofSettings>
        {
            public PlatformRoofSetupInfo(GameObject platform, PlatformSettings platformSettings) : base(platform, platformSettings)
            {
                Illuminator = Part.transform.Find(platformSettings.PartSettings.IlluminatorSettings.InstanceName).gameObject;
            }

            public GameObject Illuminator { get; private set; }
        }
    }
}

namespace GameScene.Managers.Platforms.Data
{
    public class FocusedPlatformsData
    {
        private GameObject chosenPlatform;

        public FocusedPlatformsData()
        {
            StateDescription = new FocusedPlatformsStateDescription();
            Unchosen = new UnchosenFocusedPlatformsData();
        }

        public FocusedPlatformsStateDescription StateDescription { get; private set; }

        public GameObject ChosenPlatform
        {
            get
            {
                return chosenPlatform;
            }

            set
            {
                chosenPlatform = value;

                if (chosenPlatform != null)
                    Unchosen.Remove(chosenPlatform);
            }
        }

        public UnchosenFocusedPlatformsData Unchosen { get; private set; }
    }

    public class UnchosenFocusedPlatformsData
    {
        private readonly IDictionary<GameObject, MoveDirection> moveDirections;

        private readonly SortedDictionary<MoveDirection, Vector2Int> positions;

        public UnchosenFocusedPlatformsData()
        {
            moveDirections = new Dictionary<GameObject, MoveDirection>();
            positions = new SortedDictionary<MoveDirection, Vector2Int>();
        }

        public SortedDictionary<MoveDirection, Vector2Int>.ValueCollection Positions
        {
            get
            {
                return positions.Values;
            }
        }

        public MoveDirection this[GameObject platform]
        {
            get
            {
                return moveDirections[platform];
            }
        }

        public void Add(GameObject platform, Vector2Int position, MoveDirection moveDirection)
        {
            moveDirections.Add(platform, moveDirection);
            positions.Add(moveDirection, position);
        }

        public void Remove(GameObject platform)
        {
            positions.Remove(moveDirections[platform]);
            moveDirections.Remove(platform);
        }
    }
}

namespace GameScene.Managers.Platforms.Descriptions
{
    public class FocusedPlatformsStateDescription
    {
        public FocusedPlatformsStateDescription()
        {
            IsAgreeingForChanging = false;
            Value = FocusedPlatformsState.Confined;
        }

        public bool IsAgreeingForChanging { get; set; }

        public FocusedPlatformsState Value { get; set; }
    }
}

namespace GameScene.Managers.Platforms.Enums
{
    public enum FocusedPlatformsState
    {
        Confined,

        Unconfined
    }

    public enum LiberatablePlatformsType
    {
        Banned,

        Focused
    }
}

namespace GameScene.Managers.Platforms.Events
{
    public class FocusedPlatformsConfiningEvents
    {
        public FocusedPlatformsConfiningEvents()
        {
            Confined = new MoveEvent();
            ReadyForConfining = new PlatformsReadyForConfiningEvent();
        }

        public MoveEvent Confined { get; private set; }

        public PlatformsReadyForConfiningEvent ReadyForConfining { get; private set; }
    }

    public class PlatformsReadyForConfiningEvent : UnityEvent<Action<bool?>> { }
}

namespace GameScene.Managers.Platforms.Info
{
    public struct NeighboringPlatformsPreferringInfo
    {
        public NeighboringPlatformsPreferringInfo(Func<bool> activationalFocusingConditionFunction, Func<Vector2Int, IDictionary<Vector2Int, MoveDirection>, Action<PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>?>, IEnumerator> dataPickingRoutineExtractor)
        {
            ActivationalFocusingConditionFunction = activationalFocusingConditionFunction;
            DataPickingRoutineExtractor = dataPickingRoutineExtractor;
        }

        public Func<bool> ActivationalFocusingConditionFunction { get; private set; }

        public Func<Vector2Int, IDictionary<Vector2Int, MoveDirection>, Action<PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>?>, IEnumerator> DataPickingRoutineExtractor { get; private set; }
    }

    public class PlatformsInfo : CreatedFieldEntityInfo
    {
        private FocusedPlatformsData focusedPlatformsData;

        private ISet<GameObject> activatedPlatforms;

        private ISet<GameObject> bannedPlatforms;

        private Queue<GameObject> idlyOpenedPlatforms;

        private IDictionary<Vector2Int, GameObject> occupiedPlatforms;

        public PlatformsInfo()
        {
            NeighboringPlatformsPreferring = null;
        }

        public NeighboringPlatformsPreferringInfo? NeighboringPlatformsPreferring { get; set; }

        public GameObject Platforms { get; set; }

        public FocusedPlatformsData FocusedPlatformsData
        {
            get
            {
                if (focusedPlatformsData == null)
                    focusedPlatformsData = new FocusedPlatformsData();

                return focusedPlatformsData;
            }
        }

        public ISet<GameObject> ActivatedPlatforms
        {
            get
            {
                if (activatedPlatforms == null)
                    activatedPlatforms = new HashSet<GameObject>();

                return activatedPlatforms;
            }
        }

        public ISet<GameObject> BannedPlatforms
        {
            get
            {
                if (bannedPlatforms == null)
                    bannedPlatforms = new HashSet<GameObject>();

                return bannedPlatforms;
            }
        }

        public Queue<GameObject> IdlyOpenedPlatforms
        {
            get
            {
                if (idlyOpenedPlatforms == null)
                    idlyOpenedPlatforms = new Queue<GameObject>();

                return idlyOpenedPlatforms;
            }
        }

        public IDictionary<Vector2Int, GameObject> OccupiedPlatforms
        {
            get
            {
                if (occupiedPlatforms == null)
                    occupiedPlatforms = new Dictionary<Vector2Int, GameObject>();

                return occupiedPlatforms;
            }
        }
    }
}

namespace GameScene.Managers.Platforms.Settings
{
    [Serializable]
    public struct PlatformBoxColliderSettings : ISizeableEntityComponentSettings<Vector3>
    {
        [SerializeField]
        private Vector3 center;

        [SerializeField]
        private Vector3 size;

        public Vector3 Center
        {
            get
            {
                return center;
            }
        }

        public Vector3 Size
        {
            get
            {
                return size;
            }
        }
    }

    [Serializable]
    public struct PlatformDelaysSettings
    {
        [SerializeField]
        private float creating;

        [SerializeField]
        private float destroying;

        [SerializeField]
        private float running;

        [SerializeField]
        private PlatformFocusDelaysSettings focus;

        public float Creating
        {
            get
            {
                return creating;
            }
        }

        public float Destroying
        {
            get
            {
                return destroying;
            }
        }

        public float Running
        {
            get
            {
                return running;
            }
        }

        public PlatformFocusDelaysSettings Focus
        {
            get
            {
                return focus;
            }
        }
    }

    [Serializable]
    public struct PlatformFocusDelaysSettings
    {
        [SerializeField]
        private float defocusing;

        [SerializeField]
        private float focusing;

        public float Defocusing
        {
            get
            {
                return defocusing;
            }
        }

        public float Focusing
        {
            get
            {
                return focusing;
            }
        }
    }

    [Serializable]
    public struct PlatformNonNeutralRoofIlluminatorColorsSettings
    {
        [SerializeField]
        private Color cold;

        [SerializeField]
        private Color warm;

        public Color Cold
        {
            get
            {
                return cold;
            }
        }

        public Color Warm
        {
            get
            {
                return warm;
            }
        }
    }

    [Serializable]
    public struct PlatformRoofIlluminatorColorsSettings
    {
        [SerializeField]
        private Color neutral;

        [SerializeField]
        private PlatformNonNeutralRoofIlluminatorColorsSettings nonNeutral;

        public Color Neutral
        {
            get
            {
                return neutral;
            }
        }

        public PlatformNonNeutralRoofIlluminatorColorsSettings NonNeutral
        {
            get
            {
                return nonNeutral;
            }
        }
    }

    [Serializable]
    public class PlatformRoofIlluminatorSettings : SimpleEntityObjectSettings
    {
        [SerializeField]
        private PlatformRoofIlluminatorColorsSettings colorsSettings;

        public PlatformRoofIlluminatorColorsSettings ColorsSettings
        {
            get
            {
                return colorsSettings;
            }
        }
    }

    [Serializable]
    public class PlatformRoofSettings : SimpleEntityObjectSettings
    {
        [SerializeField]
        private PlatformRoofIlluminatorSettings illuminatorSettings;

        public PlatformRoofIlluminatorSettings IlluminatorSettings
        {
            get
            {
                return illuminatorSettings;
            }
        }
    }

    [Serializable]
    public class PlatformSettings : CreatedFieldObjectSettings, IDelayedEntityObjectSettings<PlatformDelaysSettings>, IPartialEntityObjectSettings<PlatformRoofSettings>,
        ISingleAnimatorControllerEntityObjectSettings
    {
        [SerializeField]
        private int preparingDeactivationLoopsCount;

        [SerializeField]
        private PlatformBoxColliderSettings boxColliderSettings;

        [SerializeField]
        private PlatformDelaysSettings delays;

        [SerializeField]
        private string rootName;

        [SerializeField]
        private RuntimeAnimatorController animatorController;

        [SerializeField]
        private PlatformRoofSettings partSettings;

        public int PreparingDeactivationLoopsCount
        {
            get
            {
                return preparingDeactivationLoopsCount;
            }
        }

        public PlatformBoxColliderSettings BoxColliderSettings
        {
            get
            {
                return boxColliderSettings;
            }
        }

        public PlatformDelaysSettings Delays
        {
            get
            {
                return delays;
            }
        }

        public string RootName
        {
            get
            {
                return rootName;
            }
        }

        public PlatformRoofSettings PartSettings
        {
            get
            {
                return partSettings;
            }
        }

        public RuntimeAnimatorController AnimatorController
        {
            get
            {
                return animatorController;
            }
        }
    }
}