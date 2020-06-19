using MenuScene.Services.Subscription;
using ServicesLocators;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScene.Behaviours.TextControl
{
    public abstract class BaseSelectableTextControlBehaviour<T1, T2> : MonoBehaviour where T1 : Selectable where T2 : struct
    {
        protected ApplicationEventsListenersService applicationEventsListenersService;

        protected abstract T2 Value { get; set; }

        protected T1 Control { get; set; }

        protected abstract T2 GetDefaultValue();

        protected virtual void SetupControl()
        {
            Value = GetDefaultValue();
        }

        protected virtual void Start()
        {
            applicationEventsListenersService = SharedSceneServicesLocator.GetService<ApplicationEventsListenersService>();
            Control = gameObject.GetComponentInChildren<T1>();
            SetupControl();
        }
    }
}

namespace MenuScene.Behaviours.TextControl.Interfaces
{
    public interface IAdmittableSelectableTextControlBehaviour
    {
        void ProcessValueAdmitted();
    }

    public interface IAdmittableSelectableTextControlBehaviour<T> where T : struct
    {
        void ProcessValueAdmitted(T value);
    }
}