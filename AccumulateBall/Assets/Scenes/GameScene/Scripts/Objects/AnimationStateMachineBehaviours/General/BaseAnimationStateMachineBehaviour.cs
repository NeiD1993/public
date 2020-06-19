using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Events;
using GameScene.Objects.Interfaces;
using GameScene.Services.Animation;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine
{
    public abstract class BaseAnimationStateMachineBehaviour : StateMachineBehaviour, INotifiedlyDestroyableObject
    {
        protected AnimationStateService animationStateService;

        public BaseAnimationStateMachineBehaviour()
        {
            Destroyed = new UnityEvent();
        }

        public UnityEvent Destroyed { get; private set; }

        protected abstract void ProcessExiting();

        protected bool TryExit(AnimatorStateInfo stateInfo)
        {
            bool result = false;

            if (animationStateService.IsStateOfTag(stateInfo, PrimaryAnimationStateTag.Exiting))
            {
                ProcessExiting();
                result = true;
            }

            return result;
        }

        private void OnDisable()
        {
            Destroyed.Invoke();
        }

        private void OnEnable()
        {
            animationStateService = SharedSceneServicesLocator.GetService<AnimationStateService>();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Enums
{
    public enum PrimaryAnimationStateTag
    {
        Blocking,

        Exiting,

        Holding,

        Idle,

        Launching
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Events
{
    public class AnimationStatePassingEvents<T> where T : UnityEventBase, new()
    {
        public AnimationStatePassingEvents()
        {
            Entered = new UnityEvent();
            Exited = new T();
        }

        public UnityEvent Entered { get; private set; }

        public T Exited { get; private set; }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Interfaces
{
    public interface IBlockingAnimationStateMachineBehaviour
    {
        UnityEvent BlockingStateEntered { get; }
    }

    public interface IHoldingAnimationStateMachineBehaviour<T> where T : UnityEventBase, new()
    {
        AnimationStatePassingEvents<T> HoldingState { get; }
    }
}