using System;
using System.Collections.Generic;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Managers.ControlPanel.Info;
using GameScene.Managers.ControlPanel.Settings;
using GameScene.Managers.ControlPanel.Settings.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Objects.Interfaces;
using GameScene.Services.ControlPanel.Enums;
using GameScene.Services.ControlPanel.Info;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.ControlPanel
{
    public abstract class BaseChildControlEntityManager<T1, T2, T3, T4, T5, T6, T7> : BaseControlEntityManager<T2, T5>
        where T1 : MonoBehaviour, INotifiedlyHideableMaterializedObjectBehaviour<UnityEvent>, INotifiedlyDestroyableObject, ISetupableMaterializedObjectBehaviour<T7>
        where T2 : OwnedEntityObjectSettings, IRectTransformableControlSettings<T3> where T3 : BaseControlRectTransformSettings, 
        IPositionableControlRectTransformSettings<T4>, new() where T4 : BaseControlRectTransformPositionSettings where T5 : ChildControlEntityInfo<T6>, new() where T6 : Enum
    {
        public BaseChildControlEntityManager()
        {
            EntityInfo = new T5();
        }

        protected static void SetupControlRectTransformSettings(GameObject control, BaseControlRectTransformSettings rectTransformSettings,
            Action<RectTransform> additionalCustomRectTransformSetupAction, bool addRectTransform = true)
        {
            if (addRectTransform)
                control.AddComponent<RectTransform>();

            RectTransform rectTransform = control.GetComponent<RectTransform>();

            rectTransform.localRotation = Quaternion.Euler(rectTransformSettings.Rotation);
            rectTransform.localScale = rectTransformSettings.Scale;
            additionalCustomRectTransformSetupAction(rectTransform);
        }

        protected abstract PanelType GetPanelType();

        protected void CreateControl(ControlInstantiatingInfo controlInstantiatingInfo, Delegate customAdditionalControlSetupAction,
            Delegate controlBehaviourInstantiatingInfoExtractor, Func<GameObject, object> customControlSetupParameterExtractor = null)
        {
            Vector3 GetControlAnchoredPosition()
            {
                return entityObjectSettings.RectTransformSettings.PositionSettings.DefaultPosition +
                    controlInstantiatingInfo.PositionIndex * entityObjectSettings.RectTransformSettings.PositionSettings.Offset;
            }

            GameObject control = Instantiate(controlInstantiatingInfo.Prefab);
            object customObjectSetupParameter = null;

            void PerformCustomAdditionalControlSetupAction()
            {
                if (customObjectSetupParameter != null)
                    ((Action<GameObject, object>)customAdditionalControlSetupAction)(control, customObjectSetupParameter);
                else
                    ((Action<GameObject>)customAdditionalControlSetupAction)(control);
            }

            control.name = string.Concat(controlInstantiatingInfo.NamePrefix, entityObjectSettings.InstanceName);

            AttachToEntity(control);

            control.GetComponent<Animator>().runtimeAnimatorController = controlInstantiatingInfo.AnimatorController;

            SetupControlRectTransformSettings(control, entityObjectSettings.RectTransformSettings, rectTransformParameter => 
            rectTransformParameter.anchoredPosition3D = GetControlAnchoredPosition());

            if (customControlSetupParameterExtractor != null)
                customObjectSetupParameter = customControlSetupParameterExtractor(control);

            PerformCustomAdditionalControlSetupAction();
            CreateAndInitiallySetupControlBehaviour(control, (customObjectSetupParameter == null) ? 
                ((Func<ControlBehaviourInstantiatingInfo>)controlBehaviourInstantiatingInfoExtractor).Invoke() :
                ((Func<object, ControlBehaviourInstantiatingInfo>)controlBehaviourInstantiatingInfoExtractor).Invoke(customObjectSetupParameter));
        }

        protected virtual void RemoveControlBehaviourEventsListeners(T1 controlBehaviour)
        {
            eventsListenersService.RemoveOrdinaryEventListener(controlBehaviour.Destroyed);
            eventsListenersService.RemoveOrdinaryEventListener(controlBehaviour.AnimatedlyDisappeared);
        }

        protected override PanelInfo GetPanelInfo()
        {
            return new PanelInfo(GetPanelType(), entityObjectSettings.OwnerInstanceName);
        }

        protected virtual T1 CreateAndInitiallySetupControlBehaviour(GameObject control, ControlBehaviourInstantiatingInfo controlBehaviourInstantiatingInfo)
        {
            control.AddComponent<T1>();

            T1 controlBehaviour = control.GetComponent<T1>();

            controlBehaviour.Setup(controlBehaviourInstantiatingInfo.SetupInfo);
            eventsListenersService.AddOrdinaryEventListener(() => RemoveControlBehaviourEventsListeners(controlBehaviour), controlBehaviour.Destroyed);
            eventsListenersService.AddOrdinaryEventListener(() => OnControlAnimatedlyDisappeared(controlBehaviourInstantiatingInfo.Category), 
                controlBehaviour.AnimatedlyDisappeared);

            return controlBehaviour;
        }

        protected virtual void OnControlAnimatedlyDisappeared(T6 controlCategory)
        {
            Destroy(EntityInfo.Controls[controlCategory]);
            EntityInfo.Controls.Remove(controlCategory);
        }

        protected struct ControlBehaviourInstantiatingInfo
        {
            public ControlBehaviourInstantiatingInfo(T6 category, T7 setupInfo, object[] parameters = null)
            {
                Category = category;
                SetupInfo = setupInfo;
                Parameters = parameters;
            }

            public T6 Category { get; private set; }

            public T7 SetupInfo { get; private set; }

            public object[] Parameters { get; private set; }
        }

        protected struct ControlInstantiatingInfo
        {
            public ControlInstantiatingInfo(int positionIndex, string namePrefix, GameObject prefab, RuntimeAnimatorController animatorController)
            {
                PositionIndex = positionIndex;
                NamePrefix = namePrefix;
                Prefab = prefab;
                AnimatorController = animatorController;
            }

            public int PositionIndex { get; private set; }

            public string NamePrefix { get; private set; }

            public GameObject Prefab { get; private set; }

            public RuntimeAnimatorController AnimatorController { get; private set; }
        }
    }
}

namespace GameScene.Managers.ControlPanel.Info
{
    public class ChildControlEntityInfo<T> : BasicControlEntityInfo where T : Enum
    {
        private IDictionary<T, GameObject> controls;

        public IDictionary<T, GameObject> Controls
        {
            get
            {
                if (controls == null)
                    controls = new Dictionary<T, GameObject>();

                return controls;
            }
        }
    }
}

namespace GameScene.Managers.ControlPanel.Settings
{
    [Serializable]
    public abstract class BaseControlRectTransformPositionSettings
    {
        [SerializeField]
        private Vector3 defaultPosition;

        [SerializeField]
        private Vector3 offset;

        public Vector3 DefaultPosition
        {
            get
            {
                return defaultPosition;
            }
        }

        public Vector3 Offset
        {
            get
            {
                return offset;
            }
        }
    }

    public abstract class BaseControlRectTransformSettings
    {
        [SerializeField]
        private Vector3 rotation;

        [SerializeField]
        private Vector3 scale;

        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
        }
    }
}

namespace GameScene.Managers.ControlPanel.Settings.Interfaces
{
    public interface IIndexableControlRectTransformPositionSettings<T> where T : class
    {
        T PositionIndexes { get; }
    }

    public interface IPositionableControlRectTransformSettings<T> where T : BaseControlRectTransformPositionSettings
    {
        T PositionSettings { get; }
    }

    public interface IRectTransformableControlSettings<T> where T : BaseControlRectTransformSettings, new()
    {
        T RectTransformSettings { get; }
    }
}