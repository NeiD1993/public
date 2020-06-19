using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.Button;
using GameScene.Behaviours.AnimationStateMachine.Button.Enums;
using GameScene.Behaviours.Button.Characteristics;
using GameScene.Behaviours.Button.Enums;
using GameScene.Behaviours.Button.Info;
using GameScene.Behaviours.Control;
using GameScene.Behaviours.Control.Info;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Info;
using UnityEngine;

namespace GameScene.Behaviours.Button
{
    public class ButtonBehaviour : BaseControlBehaviour<ButtonAnimatorInfo, SinglePartAnimationStateMachinesBehavioursDescription<ButtonAnimationStateMachineBehaviour>, 
        ButtonAnimationStateMachineBehaviour, CharacteristicalControlBehaviourSetupInfo<ButtonCharacteristics>, ButtonCharacteristics, ButtonAnimatorControllerLayer, 
        ButtonAnimatorControllerParameter>
    {
        public ButtonBehaviour()
        {
            AnimatedlyAppeared = new MaterializedObjectBehaviourEvent();
            Clicked = new MaterializedObjectBehaviourEvent();
        }

        public MaterializedObjectBehaviourEvent AnimatedlyAppeared { get; private set; }

        public MaterializedObjectBehaviourEvent Clicked { get; private set; }

        private IEnumerator SwitchIteratively((ButtonAnimationStateTag animationStateTag, ButtonAnimatorControllerParameter animatorControllerParameter) switchingDescription)
        {
            yield return new WaitForFixedUpdate();

            if (animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), switchingDescription.animationStateTag))
                AnimatorInfo.SetParameter(switchingDescription.animatorControllerParameter);
        }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(ButtonAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.AppearanceStateExited.RemoveListener(OnAppearanceStateExited);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(ButtonAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.AppearanceStateExited.AddListener(OnAppearanceStateExited);
        }

        protected override ButtonAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new ButtonAnimatorInfo(animator);
        }

        public IEnumerator DisableIteratively()
        {
            yield return SwitchIteratively((ButtonAnimationStateTag.Enabled, ButtonAnimatorControllerParameter.IsDisabling));
        }

        public IEnumerator EnableIteratively()
        {
            yield return SwitchIteratively((ButtonAnimationStateTag.Disabling, ButtonAnimatorControllerParameter.IsEnabled));
        }

        private void OnAppearanceStateExited()
        {
            AnimatedlyAppeared.Invoke(gameObject);
        }

        private void OnMouseUpAsButton()
        {
            if (animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), ButtonAnimationStateTag.Enabled))
                Clicked.Invoke(gameObject);
        }
    }
}

namespace GameScene.Behaviours.Button.Characteristics
{
    public struct ButtonCharacteristics
    {
        public ButtonCharacteristics(ButtonType type)
        {
            Type = type;
        }

        public ButtonType Type { get; private set; }
    }
}

namespace GameScene.Behaviours.Button.Enums
{
    public enum ButtonAnimatorControllerLayer
    {
        Button
    }

    public enum ButtonAnimatorControllerParameter
    {
        IsDisabling,

        IsDisappearance,

        IsEnabled
    }

    public enum ButtonCategory
    {
        Forming,

        Keeping
    }

    public enum ButtonType
    {
        Continue,

        Pause,

        Start,

        Stop
    }
}

namespace GameScene.Behaviours.Button.Info
{
    public class ButtonAnimatorInfo : BaseMaterializedObjectAnimatorInfo<SinglePartAnimationStateMachinesBehavioursDescription<ButtonAnimationStateMachineBehaviour>, 
        ButtonAnimationStateMachineBehaviour, ButtonAnimatorControllerLayer, ButtonAnimatorControllerParameter>
    {
        public ButtonAnimatorInfo(Animator animator) : base(animator) { }

        protected override SinglePartAnimationStateMachinesBehavioursDescription<ButtonAnimationStateMachineBehaviour> CreateAnimationStateMachinesBehavioursDescription(Animator animator)
        {
            return new SinglePartAnimationStateMachinesBehavioursDescription<ButtonAnimationStateMachineBehaviour>(animator);
        }

        public override ButtonAnimatorControllerLayer GetMajorLayer()
        {
            return ButtonAnimatorControllerLayer.Button;
        }

        public override ButtonAnimatorControllerParameter GetDisappearanceParameter()
        {
            return ButtonAnimatorControllerParameter.IsDisappearance;
        }
    }
}