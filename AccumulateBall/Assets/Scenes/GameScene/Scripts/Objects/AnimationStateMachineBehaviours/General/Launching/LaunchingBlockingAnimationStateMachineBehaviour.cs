using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Launching
{
    public class LaunchingBlockingAnimationStateMachineBehaviour : LaunchingAnimationStateMachineBehaviour, IBlockingAnimationStateMachineBehaviour
    {
        public LaunchingBlockingAnimationStateMachineBehaviour()
        {
            BlockingStateEntered = new UnityEvent();
        }

        public UnityEvent BlockingStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Blocking))
                BlockingStateEntered.Invoke();
        }
    }
}