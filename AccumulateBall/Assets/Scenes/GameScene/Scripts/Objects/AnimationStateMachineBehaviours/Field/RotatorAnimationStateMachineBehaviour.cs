using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.AnimationStateMachine.Rotator.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Rotator
{
    public class RotatorAnimationStateMachineBehaviour : LaunchingBlockingAnimationStateMachineBehaviour
    {
        public RotatorAnimationStateMachineBehaviour()
        {
            RotationStateEntered = new UnityEvent();
        }

        public UnityEvent RotationStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, RotatorAnimationStateTag.Rotation))
                RotationStateEntered.Invoke();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Rotator.Enums
{
    public enum RotatorAnimationStateTag
    {
        Rotation
    }
}