using System;
using System.Collections.Generic;
using GameScene.Services.Subscription.Actions;
using GameScene.Services.Subscription.Actions.Interfaces;
using GameScene.Services.Subscription.Events;
using GameScene.Services.Subscription.Events.Enums;
using GameScene.Services.Subscription.Events.Interfaces;
using GameScene.Services.Subscription.Interfaces;
using UnityEngine.Events;

namespace GameScene.Services.Subscription
{
    public class EventsListenersService : BaseSharedService, IGenericEventsListenersService, INonGenericEventsListenersService
    {
        private readonly OrdinaryEventsListeners ordinaryEventsListeners;

        public EventsListenersService()
        {
            ordinaryEventsListeners = new OrdinaryEventsListeners();
        }

        public void AddOrdinaryEventListener(UnityAction action, UnityEvent evt)
        {
            evt.AddListener(action);
            ordinaryEventsListeners.NonGeneric.Add(evt, action);
        }

        public void AddOrdinaryEventListener<T>(UnityAction<T> action, UnityEvent<T> evt)
        {
            evt.AddListener(action);
            ordinaryEventsListeners.Generic.Add(evt, action);
        }

        public void AddUnsubscribingEventListener(UnityAction action, EventsWithUnsubscribingListeners eventsWithUnsubscribingListeners)
        {
            UnsubscribingActions unsubscribingActions = null;

            eventsWithUnsubscribingListeners.AddUnsubscribingListeners(unsubscribingActions = new UnsubscribingActions(() =>
            {
                action();
                eventsWithUnsubscribingListeners.RemoveUnsubscribingListeners(unsubscribingActions, UnsubscribingListenersRemovingOrder.Backward);
            },
            () => eventsWithUnsubscribingListeners.RemoveUnsubscribingListeners(unsubscribingActions)));
        }

        public void AddUnsubscribingEventListener<T>(UnityAction<T> action, EventsWithUnsubscribingListeners<T> eventsWithUnsubscribingListeners)
        {
            UnsubscribingActions<T> unsubscribingActions = null;

            eventsWithUnsubscribingListeners.AddUnsubscribingListeners(unsubscribingActions = new UnsubscribingActions<T>(parameter =>
            {
                action(parameter);
                eventsWithUnsubscribingListeners.RemoveUnsubscribingListeners(unsubscribingActions, UnsubscribingListenersRemovingOrder.Backward);
            },
            () => eventsWithUnsubscribingListeners.RemoveUnsubscribingListeners(unsubscribingActions)));
        }

        public void RemoveOrdinaryEventListener(UnityEvent evt)
        {
            evt.RemoveListener(ordinaryEventsListeners.NonGeneric[evt]);
            ordinaryEventsListeners.NonGeneric.Remove(evt);
        }

        public void RemoveOrdinaryEventListener<T>(UnityEvent<T> evt)
        {
            evt.RemoveListener((UnityAction<T>)ordinaryEventsListeners.Generic[evt]);
            ordinaryEventsListeners.Generic.Remove(evt);
        }

        private class OrdinaryEventsListeners
        {
            private IDictionary<UnityEventBase, Delegate> generic;

            private IDictionary<UnityEvent, UnityAction> nonGeneric;

            public IDictionary<UnityEventBase, Delegate> Generic
            {
                get
                {
                    if (generic == null)
                        generic = new Dictionary<UnityEventBase, Delegate>();

                    return generic;
                }
            }

            public IDictionary<UnityEvent, UnityAction> NonGeneric
            {
                get
                {
                    if (nonGeneric == null)
                        nonGeneric = new Dictionary<UnityEvent, UnityAction>();

                    return nonGeneric;
                }
            }
        }
    }
}

namespace GameScene.Services.Subscription.Actions
{
    public abstract class BaseUnsubscribingActions
    {
        public BaseUnsubscribingActions(UnityAction unexpected)
        {
            Unexpected = unexpected;
        }

        public UnityAction Unexpected { get; private set; }
    }

    public class UnsubscribingActions : BaseUnsubscribingActions, IExpectedUnsubscribingActions<UnityAction>
    {
        public UnsubscribingActions(UnityAction expected, UnityAction unexpected) : base(unexpected)
        {
            Expected = expected;
        }

        public UnityAction Expected { get; private set; }
    }

    public class UnsubscribingActions<T> : BaseUnsubscribingActions, IExpectedUnsubscribingActions<UnityAction<T>>
    {
        public UnsubscribingActions(UnityAction<T> expected, UnityAction unexpected) : base(unexpected)
        {
            Expected = expected;
        }

        public UnityAction<T> Expected { get; private set; }
    }
}

namespace GameScene.Services.Subscription.Actions.Interfaces
{
    public interface IExpectedUnsubscribingActions<T> where T : Delegate
    {
        T Expected { get; }
    }
}

namespace GameScene.Services.Subscription.Events
{
    public abstract class BaseEventsWithUnsubscribingListeners : IEventsAddingUnsubscribingListeners<BaseUnsubscribingActions>,
        IEventsRemovingUnsubscribingListeners<BaseUnsubscribingActions>
    {
        public BaseEventsWithUnsubscribingListeners(UnityEvent forUnexpectedUnsubscribing)
        {
            ForUnexpectedUnsubscribing = new UnityEvent[] { forUnexpectedUnsubscribing };
        }

        public BaseEventsWithUnsubscribingListeners(IEnumerable<UnityEvent> forUnexpectedUnsubscribing)
        {
            ForUnexpectedUnsubscribing = forUnexpectedUnsubscribing;
        }

        public IEnumerable<UnityEvent> ForUnexpectedUnsubscribing { get; private set; }

        public void AddUnsubscribingListeners(BaseUnsubscribingActions unsubscribingActions)
        {
            foreach (UnityEvent forUnexpectedUnsubscribing in ForUnexpectedUnsubscribing)
                forUnexpectedUnsubscribing.AddListener(unsubscribingActions.Unexpected);
        }

        public void RemoveUnsubscribingListeners(BaseUnsubscribingActions unsubscribingActions)
        {
            foreach (UnityEvent forUnexpectedUnsubscribing in ForUnexpectedUnsubscribing)
                forUnexpectedUnsubscribing.RemoveListener(unsubscribingActions.Unexpected);
        }
    }

    public class EventsWithUnsubscribingListeners : BaseEventsWithUnsubscribingListeners, IEventsAddingUnsubscribingListeners<UnsubscribingActions>,
        IEventsForExpectedUnsubscribing<UnityEvent>, IEventsRemovingUnsubscribingListenersInOrder<UnsubscribingActions>
    {
        public EventsWithUnsubscribingListeners(UnityEvent forExpectedUnsubscribing, UnityEvent forUnexpectedUnsubscribing) : base(forUnexpectedUnsubscribing)
        {
            ForExpectedUnsubscribing = forExpectedUnsubscribing;
        }

        public EventsWithUnsubscribingListeners(UnityEvent forExpectedUnsubscribing, IEnumerable<UnityEvent> forUnexpectedUnsubscribing) : base(forUnexpectedUnsubscribing)
        {
            ForExpectedUnsubscribing = forExpectedUnsubscribing;
        }

        public UnityEvent ForExpectedUnsubscribing { get; private set; }

        public void AddUnsubscribingListeners(UnsubscribingActions unsubscribingActions)
        {
            base.AddUnsubscribingListeners(unsubscribingActions);
            ForExpectedUnsubscribing.AddListener(unsubscribingActions.Expected);
        }

        public void RemoveUnsubscribingListeners(UnsubscribingActions unsubscribingActions, UnsubscribingListenersRemovingOrder removingOrder =
            UnsubscribingListenersRemovingOrder.Forward)
        {
            switch (removingOrder)
            {
                case UnsubscribingListenersRemovingOrder.Backward:
                    {
                        base.RemoveUnsubscribingListeners(unsubscribingActions);
                        ForExpectedUnsubscribing.RemoveListener(unsubscribingActions.Expected);
                    }
                    break;
                default:
                    {
                        ForExpectedUnsubscribing.RemoveListener(unsubscribingActions.Expected);
                        base.RemoveUnsubscribingListeners(unsubscribingActions);
                    }
                    break;
            }
        }
    }

    public class EventsWithUnsubscribingListeners<T> : BaseEventsWithUnsubscribingListeners, IEventsAddingUnsubscribingListeners<UnsubscribingActions<T>>,
        IEventsForExpectedUnsubscribing<UnityEvent<T>>, IEventsRemovingUnsubscribingListenersInOrder<UnsubscribingActions<T>>
    {
        public EventsWithUnsubscribingListeners(UnityEvent<T> forExpectedUnsubscribing, UnityEvent forUnexpectedUnsubscribing) : base(forUnexpectedUnsubscribing)
        {
            ForExpectedUnsubscribing = forExpectedUnsubscribing;
        }

        public EventsWithUnsubscribingListeners(UnityEvent<T> forExpectedUnsubscribing, IEnumerable<UnityEvent> forUnexpectedUnsubscribing) : base(forUnexpectedUnsubscribing)
        {
            ForExpectedUnsubscribing = forExpectedUnsubscribing;
        }

        public UnityEvent<T> ForExpectedUnsubscribing { get; private set; }

        public void AddUnsubscribingListeners(UnsubscribingActions<T> unsubscribingActions)
        {
            base.AddUnsubscribingListeners(unsubscribingActions);
            ForExpectedUnsubscribing.AddListener(unsubscribingActions.Expected);
        }

        public void RemoveUnsubscribingListeners(UnsubscribingActions<T> unsubscribingActions, UnsubscribingListenersRemovingOrder removingOrder =
            UnsubscribingListenersRemovingOrder.Forward)
        {
            switch (removingOrder)
            {
                case UnsubscribingListenersRemovingOrder.Backward:
                    {
                        base.RemoveUnsubscribingListeners(unsubscribingActions);
                        ForExpectedUnsubscribing.RemoveListener(unsubscribingActions.Expected);
                    }
                    break;
                default:
                    {
                        ForExpectedUnsubscribing.RemoveListener(unsubscribingActions.Expected);
                        base.RemoveUnsubscribingListeners(unsubscribingActions);
                    }
                    break;
            }
        }
    }
}

namespace GameScene.Services.Subscription.Events.Enums
{
    public enum UnsubscribingListenersRemovingOrder
    {
        Backward,

        Forward
    }
}

namespace GameScene.Services.Subscription.Events.Interfaces
{
    public interface IEventsAddingUnsubscribingListeners<T> where T : BaseUnsubscribingActions
    {
        void AddUnsubscribingListeners(T unsubscribingActions);
    }

    public interface IEventsForExpectedUnsubscribing<T> where T : UnityEventBase
    {
        T ForExpectedUnsubscribing { get; }
    }

    public interface IEventsRemovingUnsubscribingListeners<T> where T : BaseUnsubscribingActions
    {
        void RemoveUnsubscribingListeners(T unsubscribingActions);
    }

    public interface IEventsRemovingUnsubscribingListenersInOrder<T> where T : BaseUnsubscribingActions
    {
        void RemoveUnsubscribingListeners(T unsubscribingActions, UnsubscribingListenersRemovingOrder removingOrder = UnsubscribingListenersRemovingOrder.Forward);
    }
}

namespace GameScene.Services.Subscription.Interfaces
{
    public interface IGenericEventsListenersService
    {
        void AddOrdinaryEventListener<T>(UnityAction<T> action, UnityEvent<T> evt);

        void AddUnsubscribingEventListener<T>(UnityAction<T> action, EventsWithUnsubscribingListeners<T> eventsWithUnsubscribingListeners);

        void RemoveOrdinaryEventListener<T>(UnityEvent<T> evt);
    }

    public interface INonGenericEventsListenersService
    {
        void AddOrdinaryEventListener(UnityAction action, UnityEvent evt);

        void AddUnsubscribingEventListener(UnityAction action, EventsWithUnsubscribingListeners eventsWithUnsubscribingListeners);

        void RemoveOrdinaryEventListener(UnityEvent evt);
    }
}