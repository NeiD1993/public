using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Rotator;
using GameScene.Behaviours.FieldObject;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.Rotator.Enums;
using GameScene.Behaviours.Rotator.Info;
using GameScene.Services.Ball.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.Rotator
{
    public class RotatorBehaviour : BaseCreatedFieldObjectBehaviour<RotatorAnimatorInfo, RotatorAnimationStateMachineBehaviour, RotationType, RotatorAnimatorControllerLayer, 
        RotatorAnimatorControllerParameter>, IExternallyFeaturedCreatedFieldObjectBehaviour<RotatorInfo>
    {
        public RotatorBehaviour()
        {
            AnimatedlyRotated = new UnityEvent();
        }

        public RotatorInfo FeaturesInfo { get; protected set; }

        public UnityEvent AnimatedlyRotated { get; private set; }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(RotatorAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.RotationStateEntered.RemoveListener(OnRotationAnimationStateEntered);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(RotatorAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.RotationStateEntered.AddListener(OnRotationAnimationStateEntered);
        }

        protected override RotatorAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new RotatorAnimatorInfo(animator);
        }

        public void BeginRotation()
        {
            AnimatorInfo.SetParameter(RotatorAnimatorControllerParameter.IsRotation);
        }

        public void PrepareForRotation()
        {
            AnimatorInfo.SetParameter(RotatorAnimatorControllerParameter.IsPreRotation);
        }

        public override void Setup(RotationType setupParameter)
        {
            FeaturesInfo = new RotatorInfo(setupParameter);
        }

        public new IEnumerator TryRun()
        {
            if (base.TryRun())
                yield return new WaitUntil(() => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()),
                    PrimaryAnimationStateTag.Idle));
        }

        private void OnRotationAnimationStateEntered()
        {
            IEnumerator ProcessRotationAnimationStateEnteredIteratively()
            {
                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()));

                AnimatedlyRotated.Invoke();
            }

            StartCoroutine(ProcessRotationAnimationStateEnteredIteratively());
        }

        protected override void OnLaunchingAnimationStateEntered()
        {
            IEnumerator ProcessAnimatedLaunchingStateEnteredIteratively()
            {
                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()));

                base.OnLaunchingAnimationStateEntered();
            }

            StartCoroutine(ProcessAnimatedLaunchingStateEnteredIteratively());
        }
    }
}

namespace GameScene.Behaviours.Rotator.Enums
{
    public enum RotatorAnimatorControllerLayer
    {
        Rotator
    }

    public enum RotatorAnimatorControllerParameter
    {
        IsDisappearance,

        IsIdle,

        IsPreRotation,

        IsRotation
    }
}

namespace GameScene.Behaviours.Rotator.Info
{
    public struct RotatorInfo
    {
        public RotatorInfo(RotationType type)
        {
            Type = type;
        }

        public RotationType Type { get; private set; }
    }

    public class RotatorAnimatorInfo : BaseCreatedFieldObjectAnimatorInfo<RotatorAnimationStateMachineBehaviour, RotatorAnimatorControllerLayer, 
        RotatorAnimatorControllerParameter>
    {
        public RotatorAnimatorInfo(Animator animator) : base(animator) { }

        public override RotatorAnimatorControllerLayer GetMajorLayer()
        {
            return RotatorAnimatorControllerLayer.Rotator;
        }

        public override RotatorAnimatorControllerParameter GetDisappearanceParameter()
        {
            return RotatorAnimatorControllerParameter.IsDisappearance;
        }

        public override RotatorAnimatorControllerParameter GetIdleParameter()
        {
            return RotatorAnimatorControllerParameter.IsIdle;
        }
    }
}