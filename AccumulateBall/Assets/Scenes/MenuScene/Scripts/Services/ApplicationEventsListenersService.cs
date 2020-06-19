using System;
using System.Collections.Generic;
using MenuScene.Services.Subscription.Enums;
using UnityEngine;

namespace MenuScene.Services.Subscription
{
    public class ApplicationEventsListenersService : BaseSharedService
    {
        private readonly ApplicationEventsListenersData eventsListenersData;

        public ApplicationEventsListenersService()
        {
            eventsListenersData = new ApplicationEventsListenersData();
        }

        private void AddEventListener(ApplicationEventType eventType, MonoBehaviour obj, Delegate action)
        {
            switch (eventType)
            {
                case ApplicationEventType.Quitting:
                    Application.quitting += (Action)action;
                    break;
            }

            eventsListenersData.Add(eventType, obj, action);
        }

        private void RemoveEventListener(ApplicationEventType eventType, MonoBehaviour obj)
        {
            Delegate action = eventsListenersData[eventType, obj];

            if (action != null)
            {
                switch (eventType)
                {
                    case ApplicationEventType.Quitting:
                        Application.quitting -= (Action)action;
                        break;
                }

                eventsListenersData.Remove(eventType, obj);
            }
        }

        public void EditEventListener(ApplicationEventType eventType, MonoBehaviour obj, Delegate action)
        {
            RemoveEventListener(eventType, obj);
            AddEventListener(eventType, obj, action);
        }

        private class ApplicationEventsListenersData
        {
            private IDictionary<ApplicationEventType, IDictionary<MonoBehaviour, Delegate>> eventsListeners;

            private IDictionary<ApplicationEventType, IDictionary<MonoBehaviour, Delegate>> EventsListeners
            {
                get
                {
                    if (eventsListeners == null)
                        eventsListeners = new Dictionary<ApplicationEventType, IDictionary<MonoBehaviour, Delegate>>();

                    return eventsListeners;
                }
            }

            private IDictionary<MonoBehaviour, Delegate> this[ApplicationEventType eventType]
            {
                get
                {
                    if (!EventsListeners.ContainsKey(eventType))
                        EventsListeners.Add(eventType, new Dictionary<MonoBehaviour, Delegate>());

                    return EventsListeners[eventType];
                }
            }

            public Delegate this[ApplicationEventType eventType, MonoBehaviour obj]
            {
                get
                {
                    IDictionary<MonoBehaviour, Delegate> eventListeners = this[eventType];

                    return eventListeners.ContainsKey(obj) ? eventListeners[obj] : null;
                }
            }

            public void Add(ApplicationEventType eventType, MonoBehaviour obj, Delegate action)
            {
                this[eventType].Add(obj, action);
            }

            public void Remove(ApplicationEventType eventType, MonoBehaviour obj)
            {
                IDictionary<MonoBehaviour, Delegate> eventListeners = this[eventType];

                eventListeners.Remove(obj);

                if (eventListeners.Count == 0)
                    EventsListeners.Remove(eventType);
            }
        }
    }
}

namespace MenuScene.Services.Subscription.Enums
{
    public enum ApplicationEventType
    {
        Quitting
    }
}