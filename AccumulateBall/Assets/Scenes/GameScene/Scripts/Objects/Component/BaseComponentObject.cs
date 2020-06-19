using GameScene.Services.Subscription;
using ServicesLocators;
using UnityEngine;

namespace GameScene.Objects
{
    public abstract class BaseComponentObject : MonoBehaviour
    {
        protected EventsListenersService eventsListenersService;

        private void Start()
        {
            PreProcessStart();
            ProcessStart();
        }

        protected abstract void ProcessStart();

        protected virtual void ObtainSharingRelatedServices()
        {
            eventsListenersService = SharedSceneServicesLocator.GetService<EventsListenersService>();
        }

        protected virtual void PreProcessStart()
        {
            ObtainSharingRelatedServices();
        }
    }
}
