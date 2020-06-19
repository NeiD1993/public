using System;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.Control.Info;
using GameScene.Behaviours.MaterializedObject;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Info;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using UnityEngine.Events;

namespace GameScene.Behaviours.Control
{
    public abstract class BaseControlBehaviour<T1, T2, T3, T4, T5, T6, T7> : BaseMaterializedObjectBehaviour<UnityEvent, T1, T2, T3, T6, T7>, 
        ISetupableMaterializedObjectBehaviour<T4> where T1 : BaseMaterializedObjectAnimatorInfo<T2, T3, T6, T7> 
        where T2 : SinglePartAnimationStateMachinesBehavioursDescription<T3> where T3 : MajorExitingAnimationStateMachineBehaviour 
        where T4 : CharacteristicalControlBehaviourSetupInfo<T5> where T6 : Enum where T7 : Enum
    {
        public T5 Characteristics { get; private set; }

        public virtual void Setup(T4 setupParameter)
        {
            Characteristics = setupParameter.Characteristics;
        }

        protected override void OnMajorAnimationStateMachineExiting()
        {
            AnimatedlyDisappeared.Invoke();
        }
    }
}

namespace GameScene.Behaviours.Control.Info
{
    public class CharacteristicalControlBehaviourSetupInfo<T>
    {
        public CharacteristicalControlBehaviourSetupInfo(T characteristics)
        {
            Characteristics = characteristics;
        }

        public T Characteristics { get; private set; }
    }
}