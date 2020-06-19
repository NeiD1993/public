using System.Collections.Generic;
using GameScene.Services.Field.Enums;
using GameScene.Services.Field.Interfaces;
using GameScene.Services.Subscription;
using GameScene.Services.Subscription.Events;
using ServicesLocators;
using UnityEngine;

namespace GameScene.Services.Field
{
    public class AwaitableFieldActionsOperator : IGenericallyAddingAwaitableFieldActionsOperator, INonGenericallyAddingAwaitableFieldActionsOperator
    {
        private readonly EventsListenersService eventsListenersService;

        private readonly ISet<AwaitableFieldActionType> awaitableFieldActionTypes;

        public AwaitableFieldActionsOperator()
        {
            eventsListenersService = SharedSceneServicesLocator.GetService<EventsListenersService>();
            awaitableFieldActionTypes = new HashSet<AwaitableFieldActionType>();
        }

        private void AddAwaitableFieldActionType(AwaitableFieldActionType awaitableFieldActionType)
        {
            awaitableFieldActionTypes.Add(awaitableFieldActionType);
        }

        private void RemoveAwaitableFieldActionType(AwaitableFieldActionType awaitableFieldActionType)
        {
            awaitableFieldActionTypes.Remove(awaitableFieldActionType);
        }

        public void AddAwaitableFieldAction(EventsWithUnsubscribingListeners eventsWithUnsubscribingListeners, AwaitableFieldActionType awaitableFieldActionType)
        {
            AddAwaitableFieldActionType(awaitableFieldActionType);
            eventsListenersService.AddUnsubscribingEventListener(() => RemoveAwaitableFieldActionType(awaitableFieldActionType), eventsWithUnsubscribingListeners);
        }

        public void AddAwaitableFieldAction<T>(EventsWithUnsubscribingListeners<T> eventsWithUnsubscribingListeners, AwaitableFieldActionType awaitableFieldActionType)
        {
            AddAwaitableFieldActionType(awaitableFieldActionType);
            eventsListenersService.AddUnsubscribingEventListener(fieldEventParameter => RemoveAwaitableFieldActionType(awaitableFieldActionType), 
                eventsWithUnsubscribingListeners);
        }

        public bool ContainsAwaitableFieldAction(AwaitableFieldActionType awaitableFieldActionType)
        {
            return awaitableFieldActionTypes.Contains(awaitableFieldActionType);
        }

        public WaitUntil WaitUntilAwaitableFieldActionExecutedIteratively(AwaitableFieldActionType awaitableFieldActionType)
        {
            return new WaitUntil(() => !ContainsAwaitableFieldAction(awaitableFieldActionType));
        }

        public WaitUntil WaitUntilAwaitableFieldActionsExecutedIteratively()
        {
            return new WaitUntil(() => awaitableFieldActionTypes.Count == 0);
        }
    }
}

namespace GameScene.Services.Field.Enums
{
    public enum AwaitableFieldActionType
    {
        BallPartFilling,

        BallPartsFlushing,

        FieldIdlingBeforeInterruptingFinalization,

        PlatformBanning,

        PlatformEnlarging,

        PlatformLowering,

        PlatformPostFocusing,

        PlatformPostRejecting,

        PlatformsUnbanning,

        ReservoirFlushing,

        ReservoirFreezing,

        RotatorDestroying,

        RotatorPreparingForRotation,

        RotatorRotation
    }
}

namespace GameScene.Services.Field.Interfaces
{
    public interface IGenericallyAddingAwaitableFieldActionsOperator
    {
        void AddAwaitableFieldAction<T>(EventsWithUnsubscribingListeners<T> eventsWithUnsubscribingListeners, AwaitableFieldActionType awaitableFieldActionType);
    }

    public interface INonGenericallyAddingAwaitableFieldActionsOperator
    {
        void AddAwaitableFieldAction(EventsWithUnsubscribingListeners eventsWithUnsubscribingListeners, AwaitableFieldActionType awaitableFieldActionType);
    }
}