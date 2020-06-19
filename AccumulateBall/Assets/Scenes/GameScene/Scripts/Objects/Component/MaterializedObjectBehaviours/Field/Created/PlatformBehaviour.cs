using System;
using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Events;
using GameScene.Behaviours.AnimationStateMachine.Platform;
using GameScene.Behaviours.AnimationStateMachine.Platform.Enums;
using GameScene.Behaviours.AnimationStateMachine.Platform.Events;
using GameScene.Behaviours.FieldObject;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Info.Interfaces;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.Platform.Enums;
using GameScene.Behaviours.Platform.Events;
using GameScene.Behaviours.Platform.Info;
using GameScene.Services.Platform;
using GameScene.Services.Platform.Enums;
using GameScene.Services.Platform.Info;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.Platform
{
    public class PlatformBehaviour : BaseInternallyFeaturedCreatedFieldObjectBehaviour<PlatformAnimatorInfo, PlatformAnimationStateMachineBehaviour, PlatformInfo,
        PlatformAnimatorControllerLayer, PlatformAnimatorControllerParameter>, IHoldableFieldObjectBehaviour<PlatformHoldingAnimationExitedEvent>
    {
        private ShiningService shiningService;

        public PlatformBehaviour()
        {
            AnimatedlyAccepted = new MaterializedObjectBehaviourEvent();
            AnimatedlyActivated = new MaterializedObjectBehaviourEvent();
            AnimatedlyBanned = new MaterializedObjectBehaviourEvent();
            AnimatedlyDeactivated = new MaterializedObjectBehaviourEvent();
            AnimatedlyUnbanned = new MaterializedObjectBehaviourEvent();
            Chosen = new MaterializedObjectBehaviourEvent();
            HoldingAnimation = new AnimationPassingEvents<PlatformHoldingAnimationExitedEvent>();
        }

        public MaterializedObjectBehaviourEvent AnimatedlyAccepted { get; private set; }

        public MaterializedObjectBehaviourEvent AnimatedlyActivated { get; private set; }

        public MaterializedObjectBehaviourEvent AnimatedlyBanned { get; private set; }

        public MaterializedObjectBehaviourEvent AnimatedlyDeactivated { get; private set; }

        public MaterializedObjectBehaviourEvent AnimatedlyUnbanned { get; private set; }

        public MaterializedObjectBehaviourEvent Chosen { get; private set; }

        public AnimationPassingEvents<PlatformHoldingAnimationExitedEvent> HoldingAnimation { get; private set; }

        private void AddHoldingAnimationStatePassingEventsListeners(PlatformAnimationStateMachineBehaviour platformAnimationStateMachineBehaviour)
        {
            AnimationStatePassingEvents<PlatformHoldingAnimationStateExitedEvent> holdingAnimationStatePassingEvents =
                platformAnimationStateMachineBehaviour.HoldingState;

            holdingAnimationStatePassingEvents.Entered.AddListener(OnHoldingAnimationStateEntered);
            holdingAnimationStatePassingEvents.Exited.AddListener(OnHoldingAnimationStateExited);
        }

        private void RemoveHoldingAnimationStatePassingEventsListeners(PlatformAnimationStateMachineBehaviour platformAnimationStateMachineBehaviour)
        {
            AnimationStatePassingEvents<PlatformHoldingAnimationStateExitedEvent> holdingAnimationStatePassingEvents =
                platformAnimationStateMachineBehaviour.HoldingState;

            holdingAnimationStatePassingEvents.Entered.RemoveListener(OnHoldingAnimationStateEntered);
            holdingAnimationStatePassingEvents.Exited.RemoveListener(OnHoldingAnimationStateExited);
        }

        private bool TryProcessComprehension(Action<float> shiningAction, Func<bool> terminationConditionFunction)
        {
            bool isTermination = terminationConditionFunction();

            shiningAction(isTermination ? 1 : AnimatorInfo.GetCurrentStateAnimationFragmentTime(AnimatorInfo.GetMajorLayer()));

            return !isTermination;
        }

        private IEnumerator ExposeWithConditionalWaitingForEndOfFrameIteratively(PlatformAnimatorControllerParameter exposingAnimatorControllerParameter, Func<bool> waitingPredicate, bool awaitForEndOfFrame)
        {
            AnimatorInfo.SetParameter(exposingAnimatorControllerParameter);

            if (awaitForEndOfFrame)
                yield return new WaitForFixedUpdate();

            yield return new WaitUntil(waitingPredicate);
        }

        private IEnumerator IdleIteratively()
        {
            Idle();

            yield return new WaitUntil(() => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()),
                PrimaryAnimationStateTag.Idle));
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            shiningService = SharedSceneServicesLocator.GetService<ShiningService>();
        }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(PlatformAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.ActivatedStateEntered.RemoveListener(OnActivatedAnimationStateEntered);
            majorAnimationStateMachineBehaviour.BanningStateEntered.RemoveListener(OnBanningAnimationStateEntered);
            majorAnimationStateMachineBehaviour.DeactivationStateExited.RemoveListener(OnDeactivationAnimationStateExited);
            majorAnimationStateMachineBehaviour.PostAcceptingStateExited.RemoveListener(OnPostAcceptingAnimationStateExited);
            majorAnimationStateMachineBehaviour.UnbanningStateExited.RemoveListener(OnUnbanningAnimationStateExited);
            RemoveHoldingAnimationStatePassingEventsListeners(majorAnimationStateMachineBehaviour);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(PlatformAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.ActivatedStateEntered.AddListener(OnActivatedAnimationStateEntered);
            majorAnimationStateMachineBehaviour.BanningStateEntered.AddListener(OnBanningAnimationStateEntered);
            majorAnimationStateMachineBehaviour.DeactivationStateExited.AddListener(OnDeactivationAnimationStateExited);
            majorAnimationStateMachineBehaviour.PostAcceptingStateExited.AddListener(OnPostAcceptingAnimationStateExited);
            majorAnimationStateMachineBehaviour.UnbanningStateExited.AddListener(OnUnbanningAnimationStateExited);
            AddHoldingAnimationStatePassingEventsListeners(majorAnimationStateMachineBehaviour);
        }

        protected override PlatformAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new PlatformAnimatorInfo(animator);
        }

        public void BeginActivation()
        {
            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsActivated);
        }

        public void BeginBanning()
        {
            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsBanned);
        }

        public void BeginEnlarging()
        {
            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsEnlarged);
        }

        public void BeginLowering()
        {
            Idle();
        }

        public void BeginUnbanning()
        {
            Idle();
        }

        public void ChangeCheckingStatus(CheckingStatus checkingStatus)
        {
            PlatformAnimatorControllerParameter checkingAnimatorControllerParameter;

            switch (checkingStatus)
            {
                case CheckingStatus.Begun:
                    checkingAnimatorControllerParameter = PlatformAnimatorControllerParameter.IsCheckingBegun;
                    break;
                default:
                    checkingAnimatorControllerParameter = PlatformAnimatorControllerParameter.IsCheckingEnded;
                    break;
            }

            AnimatorInfo.SetParameter(checkingAnimatorControllerParameter);
        }

        public void Defocus()
        {
            AnimatorStateInfo currentAnimatorStateInfo = AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer());

            if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.ActivationallyFocused))
                BeginActivation();
            else if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.IdlyFocused))
                Idle();
        }

        public void Focus()
        {
            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsFocused);
        }

        public override void Setup(PlatformInfo setupParameter)
        {
            FeaturesInfo = setupParameter;
        }

        public IEnumerator BeginDeactivationIteratively(int preparingDeactivationAnimationLoopsCount)
        {
            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsDeactivated);

            yield return new WaitUntil(() =>
            {
                PlatformAnimatorControllerLayer layer = AnimatorInfo.GetMajorLayer();

                return animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), PlatformAnimationStateTag.PreDeactivated) &&
                (AnimatorInfo.GetCurrentStateAnimationLoopsCount(layer) == preparingDeactivationAnimationLoopsCount);
            });

            Idle();
        }

        public IEnumerator CheckIteratively(CheckingAction checkingAction)
        {
            PlatformAnimatorControllerParameter checkingAnimatorControllerParameter;
            PlatformAnimationStateTag checkingAnimationStateTag;

            switch (checkingAction)
            {
                case CheckingAction.Accept:
                    {
                        checkingAnimatorControllerParameter = PlatformAnimatorControllerParameter.IsAccepted;
                        checkingAnimationStateTag = PlatformAnimationStateTag.Accepting;
                    }
                    break;
                default:
                    {
                        checkingAnimatorControllerParameter = PlatformAnimatorControllerParameter.IsRejected;
                        checkingAnimationStateTag = PlatformAnimationStateTag.Rejecting;
                    }
                    break;
            }

            AnimatorInfo.SetParameter(checkingAnimatorControllerParameter);

            yield return new WaitUntil(() =>
            {
                PlatformAnimatorControllerLayer layer = AnimatorInfo.GetMajorLayer();

                return animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), checkingAnimationStateTag) && 
                AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
            });
        }

        public IEnumerator CloseIteratively()
        {
            Action<float> shiningAction = shiningService.CreateShiningAction(FeaturesInfo.Illuminator, new ShiningInfo(ShiningType.Unmirrored, FeaturesInfo.DefaultRadianceColor));

            yield return ExposeWithConditionalWaitingForEndOfFrameIteratively(AnimatorInfo.GetIdleParameter(),
                () => !TryProcessComprehension(shiningAction, () => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), 
                PrimaryAnimationStateTag.Idle)), true);
        }

        public IEnumerator FailIteratively(FailingType failingType)
        {
            PlatformFailingAnimationInfo GetFailingAnimationInfo()
            {
                AnimatorStateInfo currentAnimatorStateInfo = AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer());
                PlatformFailingAnimationInfo failingAnimationInfo = default;

                switch (failingType)
                {
                    case FailingType.Input:
                        {
                            if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PrimaryAnimationStateTag.Idle))
                                failingAnimationInfo = new PlatformFailingAnimationInfo(PlatformAnimationStateTag.InputIdleFailing);
                            else if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.Rejecting))
                                failingAnimationInfo = new PlatformFailingAnimationInfo(PlatformAnimationStateTag.InputRejectedFailing);
                        }
                        break;
                    default:
                        {
                            if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.InputIdleFailing))
                                failingAnimationInfo = new PlatformFailingAnimationInfo(PrimaryAnimationStateTag.Idle, false);
                            else if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.InputRejectedFailing))
                                failingAnimationInfo = new PlatformFailingAnimationInfo(PlatformAnimationStateTag.OutputRejectedFailing);
                        }
                        break;
                }

                return failingAnimationInfo;
            }

            Func<bool> GetWaitingConditionFunction(PlatformFailingAnimationInfo failingAnimationInfo)
            {
                Func<bool> waitingConditionFunction = () => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), 
                    failingAnimationInfo.EndStateTag);

                return failingAnimationInfo.IsBlocking ? () => waitingConditionFunction() && AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()) : 
                    waitingConditionFunction;
            }

            AnimatorInfo.SetParameter(PlatformAnimatorControllerParameter.IsFailed, Convert.ToBoolean(failingType));

            yield return new WaitUntil(GetWaitingConditionFunction(GetFailingAnimationInfo()));
        }

        public IEnumerator OpenIteratively(Action<bool> idleIndicatorExtractor, Color radianceColor)
        {
            Func<bool> GetTerminationConditionFunction(out (bool idle, bool activated) animationStateTagsIndicators)
            {
                AnimatorStateInfo currentAnimatorStateInfo = AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer());
                Func<bool> terminationPredicate = null;

                animationStateTagsIndicators = (false, false);

                if ((animationStateTagsIndicators.idle = animationStateService.IsStateOfTag(currentAnimatorStateInfo, PrimaryAnimationStateTag.Idle)) || 
                    animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.Accepting))
                    terminationPredicate = () =>
                    {
                        PlatformAnimatorControllerLayer layer = AnimatorInfo.GetMajorLayer();

                        return animationStateService.IsStateTagAmong(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), 
                            new Enum[] { PlatformAnimationStateTag.AutoOpening, PlatformAnimationStateTag.ManualOpening }) && 
                            AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
                    };
                else if (animationStateTagsIndicators.activated = animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.Activated))
                    terminationPredicate = () => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), 
                        PlatformAnimationStateTag.Activated);

                return terminationPredicate;
            }

            Action<float> GetShiningAction(bool isAnimationStateOfIdleTag)
            {
                ShiningType shiningType = isAnimationStateOfIdleTag ? ShiningType.Unmirrored : ShiningType.Mirrored;

                return shiningService.CreateShiningAction(FeaturesInfo.Illuminator, new ShiningInfo(shiningType, radianceColor));
            }

            (bool idle, bool activated) isAnimationStateOfTag;
            Func<bool> terminationConditionFunction = GetTerminationConditionFunction(out isAnimationStateOfTag);
            Action<float> shiningAction = GetShiningAction(isAnimationStateOfTag.idle);

            yield return ExposeWithConditionalWaitingForEndOfFrameIteratively(PlatformAnimatorControllerParameter.IsOpened,
                () => !TryProcessComprehension(shiningAction, terminationConditionFunction), isAnimationStateOfTag.activated);

            idleIndicatorExtractor(isAnimationStateOfTag.idle);
        }

        public IEnumerator SkipIteratively()
        {
            Func<bool> GetWaitingConditionFunction(out bool animationStateIdleTagIndicator)
            {
                AnimatorStateInfo currentAnimatorStateInfo = AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer());

                if (animationStateIdleTagIndicator = animationStateService.IsStateOfTag(currentAnimatorStateInfo, PrimaryAnimationStateTag.Idle))
                    return () => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), PrimaryAnimationStateTag.Idle);
                else if (animationStateService.IsStateOfTag(currentAnimatorStateInfo, PlatformAnimationStateTag.Rejecting))
                    return () =>
                    {
                        PlatformAnimatorControllerLayer layer = AnimatorInfo.GetMajorLayer();

                        return animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), PlatformAnimationStateTag.Skipping) &&
                        AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
                    };
                else
                    return null;
            }

            bool isAnimationStateOfIdleTag;

            yield return ExposeWithConditionalWaitingForEndOfFrameIteratively(PlatformAnimatorControllerParameter.IsSkipped,
                GetWaitingConditionFunction(out isAnimationStateOfIdleTag), isAnimationStateOfIdleTag);
        }

        public IEnumerator UnbanIteratively()
        {
            yield return IdleIteratively();
        }

        private void OnActivatedAnimationStateEntered()
        {
            AnimatedlyActivated.Invoke(gameObject);
        }

        private void OnBanningAnimationStateEntered()
        {
            IEnumerator ProcessInputBanningAnimationStateEnteredIteratively()
            {
                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()));

                AnimatedlyBanned.Invoke(gameObject);
            }

            StartCoroutine(ProcessInputBanningAnimationStateEnteredIteratively());
        }

        private void OnDeactivationAnimationStateExited()
        {
            AnimatedlyDeactivated.Invoke(gameObject);
        }

        private void OnHoldingAnimationStateEntered()
        {
            HoldingAnimation.Started.Invoke();
        }

        private void OnHoldingAnimationStateExited(PlatformHoldingAnimationStateType holdingAnimationStateType)
        {
            PlatformHoldingAnimationType holdingAnimationType;

            switch (holdingAnimationStateType)
            {
                case PlatformHoldingAnimationStateType.GrowingDownRelated:
                    holdingAnimationType = PlatformHoldingAnimationType.GrowingDown;
                    break;
                case PlatformHoldingAnimationStateType.GrowingUpRelated:
                    holdingAnimationType = PlatformHoldingAnimationType.GrowingUp;
                    break;
                default:
                    holdingAnimationType = PlatformHoldingAnimationType.PostRejecting;
                    break;
            }

            HoldingAnimation.Ended.Invoke(holdingAnimationType);
        }

        private void OnMouseUpAsButton()
        {
            AnimatorStateInfo currentAnimatorStateInfo = AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer());

            if ((AnimatorInfo.Animator.speed > 0) && animationStateService.IsStateTagAmong(currentAnimatorStateInfo,
                new Enum[] { PlatformAnimationStateTag.IdlyFocused, PlatformAnimationStateTag.ActivationallyFocused }))
                Chosen.Invoke(gameObject);
        }

        private void OnPostAcceptingAnimationStateExited()
        {
            AnimatedlyAccepted.Invoke(gameObject);
        }

        private void OnUnbanningAnimationStateExited()
        {
            AnimatedlyUnbanned.Invoke(gameObject);
        }

        private struct PlatformFailingAnimationInfo : IFieldObjectAnimationInfo<Enum>
        {
            public PlatformFailingAnimationInfo(Enum endStateTag, bool isBlocking = true)
            {
                IsBlocking = isBlocking;
                EndStateTag = endStateTag;
            }

            public bool IsBlocking { get; set; }

            public Enum EndStateTag { get; set; }
        }
    }
}

namespace GameScene.Behaviours.Platform.Enums
{
    public enum CheckingAction
    {
        Accept,

        Reject
    }

    public enum CheckingStatus
    {
        Begun,

        Ended
    }

    public enum FailingType
    {
        Input = 1,

        Output = 0
    }

    public enum PlatformAnimatorControllerLayer
    {
        Platform
    }

    public enum PlatformAnimatorControllerParameter
    {
        IsAccepted,

        IsActivated,

        IsBanned,

        IsCheckingBegun,

        IsCheckingEnded,

        IsDeactivated,

        IsDisappearance,

        IsEnlarged,

        IsFailed,

        IsFocused,

        IsIdle,

        IsOpened,

        IsRejected,

        IsSkipped
    }

    public enum PlatformHoldingAnimationType
    {
        GrowingDown,

        GrowingUp,

        PostRejecting
    }
}

namespace GameScene.Behaviours.Platform.Events
{
    public class PlatformHoldingAnimationExitedEvent : UnityEvent<PlatformHoldingAnimationType> { }
}

namespace GameScene.Behaviours.Platform.Info
{
    public struct PlatformInfo
    {
        public PlatformInfo(Color defaultRadianceColor, GameObject illuminator)
        {
            DefaultRadianceColor = defaultRadianceColor;
            Illuminator = illuminator;
        }

        public Color DefaultRadianceColor { get; private set; }

        public GameObject Illuminator { get; private set; }
    }

    public class PlatformAnimatorInfo : BaseCreatedFieldObjectAnimatorInfo<PlatformAnimationStateMachineBehaviour, PlatformAnimatorControllerLayer,
        PlatformAnimatorControllerParameter>
    {
        public PlatformAnimatorInfo(Animator animator) : base(animator) { }

        public override PlatformAnimatorControllerLayer GetMajorLayer()
        {
            return PlatformAnimatorControllerLayer.Platform;
        }

        public override PlatformAnimatorControllerParameter GetDisappearanceParameter()
        {
            return PlatformAnimatorControllerParameter.IsDisappearance;
        }

        public override PlatformAnimatorControllerParameter GetIdleParameter()
        {
            return PlatformAnimatorControllerParameter.IsIdle;
        }
    }
}