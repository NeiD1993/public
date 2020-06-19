using GameScene.Behaviours.AnimationStateMachine.Ball.Enums;
using GameScene.Behaviours.AnimationStateMachine.Ball.Events;
using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Events;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.AnimationStateMachine.Interfaces;
using GameScene.Behaviours.Ball.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Ball
{
    public class BallPartAnimationStateMachineBehaviour : MinorExitingAnimationStateMachineBehaviour, 
        IHoldingAnimationStateMachineBehaviour<BallPartHoldingAnimationStateExitedEvent>
    {
        private bool isHoldingAnimationActivated;

        [SerializeField]
        private CardinalPoint cardinalPoint;

        public BallPartAnimationStateMachineBehaviour()
        {
            isHoldingAnimationActivated = false;
            HoldingState = new AnimationStatePassingEvents<BallPartHoldingAnimationStateExitedEvent>();
        }

        public CardinalPoint CardinalPoint
        {
            get
            {
                return cardinalPoint;
            }
        }

        public AnimationStatePassingEvents<BallPartHoldingAnimationStateExitedEvent> HoldingState { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Holding))
            {
                isHoldingAnimationActivated = true;
                HoldingState.Entered.Invoke();
            }
            else if (isHoldingAnimationActivated)
            {
                isHoldingAnimationActivated = false;
                HoldingState.Exited.Invoke(!animationStateService.IsStateOfTag(stateInfo, BallPartAnimationStateTag.HalfFilled) ? BallPartAnimationStateTag.FullFilled : 
                    BallPartAnimationStateTag.HalfFilled);
            }
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Ball.Enums
{
    public enum BallPartAnimationStateTag
    {
        FullFilled,

        HalfFilled
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Ball.Events
{
    public class BallPartHoldingAnimationStateExitedEvent : UnityEvent<BallPartAnimationStateTag> { }
}