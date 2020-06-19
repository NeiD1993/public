using GameScene.Behaviours.AnimationStateMachine.Button.Enums;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Button
{
    public class ButtonAnimationStateMachineBehaviour : MajorExitingAnimationStateMachineBehaviour
    {
        public ButtonAnimationStateMachineBehaviour()
        {
            AppearanceStateExited = new UnityEvent();
        }

        public UnityEvent AppearanceStateExited { get; private set; }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, ButtonAnimationStateTag.Appearance))
                AppearanceStateExited.Invoke();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Button.Enums
{
    public enum ButtonAnimationStateTag
    {
        Appearance,

        Disabling,

        Enabled
    }
}