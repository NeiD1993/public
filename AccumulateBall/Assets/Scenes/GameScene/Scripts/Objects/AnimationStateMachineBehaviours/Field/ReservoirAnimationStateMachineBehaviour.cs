using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.AnimationStateMachine.Reservoir.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Reservoir
{
    public class ReservoirAnimationStateMachineBehaviour : LaunchingAnimationStateMachineBehaviour
    {
        public ReservoirAnimationStateMachineBehaviour()
        {
            FreezingStateExited = new UnityEvent();
        }

        public UnityEvent FreezingStateExited { get; private set; }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, ReservoirAnimationStateTag.Freezing))
                FreezingStateExited.Invoke();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Reservoir.Enums
{
    public enum ReservoirAnimationStateTag
    {
        Freezing
    }
}