using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.AnimationStateMachine.Reservoir;
using GameScene.Behaviours.FieldObject;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Behaviours.Reservoir.Info;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.Reservoir
{
    public class ReservoirBehaviour : BaseGeneratedFieldObjectBehaviour<ReservoirAnimatorInfo, ReservoirAnimationStateMachineBehaviour, 
        MinorExitingAnimationStateMachineBehaviour, ReservoirInfo, ReservoirAnimatorControllerLayer, ReservoirAnimatorControllerParameter>, 
        ICheckableGeneratedFieldObjectBehaviour<SubstanceColorType>, ISetupableMaterializedObjectBehaviour<SubstanceColorType>
    {
        public ReservoirBehaviour()
        {
            AnimatedlyFreezed = new UnityEvent();
        }

        public UnityEvent AnimatedlyFreezed { get; private set; }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(ReservoirAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.FreezingStateExited.RemoveListener(OnFreezingAnimationStateExited);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(ReservoirAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.FreezingStateExited.AddListener(OnFreezingAnimationStateExited);
        }

        protected override ReservoirAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new ReservoirAnimatorInfo(animator);
        }

        public void BeginSubstanceFlushing()
        {
            AnimatorInfo.SetParameter(ReservoirAnimatorControllerParameter.IsFlushing);
        }

        public void Setup(SubstanceColorType setupParameter)
        {
            HallmarksInfo = new ReservoirInfo(setupParameter);
        }

        public SubstanceColorType CreateCheckableData()
        {
            return HallmarksInfo.SubstanceColorType;
        }

        private void OnFreezingAnimationStateExited()
        {
            AnimatedlyFreezed.Invoke();
        }
    }
}

namespace GameScene.Behaviours.Reservoir.Enums
{
    public enum ReservoirAnimatorControllerLayer
    {
        Carcase,

        Substance
    }

    public enum ReservoirAnimatorControllerParameter
    {
        IsDisappearance,

        IsFlushing,

        IsIdle
    }

    public enum SubstanceColorType
    {
        Blue,

        Green,

        Orange,

        Red
    }
}

namespace GameScene.Behaviours.Reservoir.Info
{
    public struct ReservoirInfo
    {
        public ReservoirInfo(SubstanceColorType substanceColorType)
        {
            SubstanceColorType = substanceColorType;
        }

        public SubstanceColorType SubstanceColorType { get; private set; }
    }

    public class ReservoirAnimatorInfo : BaseMinorlySingleLayeredGeneratedFieldObjectAnimatorInfo<ReservoirAnimationStateMachineBehaviour, 
        MinorExitingAnimationStateMachineBehaviour, ReservoirAnimatorControllerLayer, ReservoirAnimatorControllerParameter>
    {
        public ReservoirAnimatorInfo(Animator animator) : base(animator) { }

        public override ReservoirAnimatorControllerLayer GetMajorLayer()
        {
            return ReservoirAnimatorControllerLayer.Carcase;
        }

        public override ReservoirAnimatorControllerLayer GetMinorLayer()
        {
            return ReservoirAnimatorControllerLayer.Substance;
        }

        public override ReservoirAnimatorControllerParameter GetDisappearanceParameter()
        {
            return ReservoirAnimatorControllerParameter.IsDisappearance;
        }

        public override ReservoirAnimatorControllerParameter GetIdleParameter()
        {
            return ReservoirAnimatorControllerParameter.IsIdle;
        }
    }
}