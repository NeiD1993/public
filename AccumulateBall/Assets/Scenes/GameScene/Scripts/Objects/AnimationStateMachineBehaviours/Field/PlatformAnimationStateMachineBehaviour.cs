using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Events;
using GameScene.Behaviours.AnimationStateMachine.Interfaces;
using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.AnimationStateMachine.Platform.Enums;
using GameScene.Behaviours.AnimationStateMachine.Platform.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Platform
{
    public class PlatformAnimationStateMachineBehaviour : LaunchingBlockingAnimationStateMachineBehaviour,
        IHoldingAnimationStateMachineBehaviour<PlatformHoldingAnimationStateExitedEvent>
    {
        private PlatformKeyActivatedAnimationType keyActivatedAnimationType;

        public PlatformAnimationStateMachineBehaviour()
        {
            keyActivatedAnimationType = PlatformKeyActivatedAnimationType.None;
            ActivatedStateEntered = new UnityEvent();
            BanningStateEntered = new UnityEvent();
            DeactivationStateExited = new UnityEvent();
            PostAcceptingStateExited = new UnityEvent();
            UnbanningStateExited = new UnityEvent();
            HoldingState = new AnimationStatePassingEvents<PlatformHoldingAnimationStateExitedEvent>();
        }

        public UnityEvent ActivatedStateEntered { get; private set; }

        public UnityEvent BanningStateEntered { get; private set; }

        public UnityEvent DeactivationStateExited { get; private set; }

        public UnityEvent PostAcceptingStateExited { get; private set; }

        public UnityEvent UnbanningStateExited { get; private set; }

        public AnimationStatePassingEvents<PlatformHoldingAnimationStateExitedEvent> HoldingState { get; private set; }

        private void ProcessHoldingAnimationStateExited()
        {
            PlatformHoldingAnimationStateType holdingAnimationStateType;

            switch (keyActivatedAnimationType)
            {
                case PlatformKeyActivatedAnimationType.PostEnlarging:
                    holdingAnimationStateType = PlatformHoldingAnimationStateType.GrowingDownRelated;
                    break;
                case PlatformKeyActivatedAnimationType.None:
                    holdingAnimationStateType = PlatformHoldingAnimationStateType.GrowingUpRelated;
                    break;
                default:
                    holdingAnimationStateType = PlatformHoldingAnimationStateType.PostRejectingRelated;
                    break;
            }

            if (keyActivatedAnimationType != PlatformKeyActivatedAnimationType.None)
                keyActivatedAnimationType = PlatformKeyActivatedAnimationType.None;

            HoldingState.Exited.Invoke(holdingAnimationStateType);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Activated))
                ActivatedStateEntered.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Banning))
                BanningStateEntered.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Enlarged))
                keyActivatedAnimationType = PlatformKeyActivatedAnimationType.PostEnlarging;
            else if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Holding))
                HoldingState.Entered.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Rejecting))
                keyActivatedAnimationType = PlatformKeyActivatedAnimationType.Rejection;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Deactivation))
                DeactivationStateExited.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Holding))
                ProcessHoldingAnimationStateExited();
            else if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.PostAccepting))
                PostAcceptingStateExited.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, PlatformAnimationStateTag.Unbanning))
                UnbanningStateExited.Invoke();
        }

        private enum PlatformKeyActivatedAnimationType
        {
            None,

            PostEnlarging,

            Rejection
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Platform.Enums
{
    public enum PlatformHoldingAnimationStateType
    {
        GrowingDownRelated,

        GrowingUpRelated,

        PostRejectingRelated
    }

    public enum PlatformAnimationStateTag
    {
        Accepting,

        Activated,

        ActivationallyFocused,

        Appeared,

        AutoOpening,

        Banning,

        Deactivation,

        Enlarged,

        IdlyFocused,

        InputIdleFailing,

        InputRejectedFailing,

        ManualOpening,

        OutputRejectedFailing,

        PostAccepting,

        PreDeactivated,

        Rejecting,

        Skipping,

        Unbanning
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Platform.Events
{
    public class PlatformHoldingAnimationStateExitedEvent : UnityEvent<PlatformHoldingAnimationStateType> { }
}