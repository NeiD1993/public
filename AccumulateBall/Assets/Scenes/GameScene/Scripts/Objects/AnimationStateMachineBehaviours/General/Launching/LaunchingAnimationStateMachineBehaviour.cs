using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Launching
{
    public class LaunchingAnimationStateMachineBehaviour : MajorExitingAnimationStateMachineBehaviour
    {
        public LaunchingAnimationStateMachineBehaviour()
        {
            LaunchingStateEntered = new UnityEvent();
        }

        public UnityEvent LaunchingStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Launching))
                LaunchingStateEntered.Invoke();
        }
    }
}