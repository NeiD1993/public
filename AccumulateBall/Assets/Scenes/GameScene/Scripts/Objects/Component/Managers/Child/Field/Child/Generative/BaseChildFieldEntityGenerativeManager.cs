using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Events;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Field.Settings;
using GameScene.Objects.Interfaces;
using GameScene.Services.Colors;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Field
{
    public abstract class BaseChildFieldEntityGenerativeManager<T1, T2, T3, T4, T5> : BaseChildFieldEntityManager<T1, T2, GeneratedFieldObjectInfo, FieldObjectPositionEvent>,
        ICheckableGeneratedFieldObjectManager<Vector2Int>, IFlushableGeneratedFieldObjectManager,
        INotifiedlyDestroyableByObjectChildFieldEntityManager<FieldObjectPositionEvent> where T1 : MonoBehaviour, INotifiedlyHideableMaterializedObjectBehaviour<UnityEvent>,
        ILaunchableFieldObjectBehaviour, IPartialGeneratedFieldObjectBehaviour, INotifiedlyDestroyableObject, IRetainableFieldObjectBehaviour 
        where T2 : GeneratedFieldObjectSettings<T3, T4, T5> where T3 : GeneratedFieldObjectPartSettings<T4, T5> where T4 : IUnitedlyGettableEntityCategorySettings<Color, T5> 
        where T5 : Enum
    {
        protected MaterializedObjectElementColorService materializedObjectElementColorService;

        public BaseChildFieldEntityGenerativeManager()
        {
            ObjectPartsFlushed = new UnityEvent();
            EntityObjectDestroyed = new FieldObjectPositionEvent();
        }

        public override GameObject Entity
        {
            get
            {
                return EntityInfo.Object;
            }
        }

        public UnityEvent ObjectPartsFlushed { get; private set; }

        public FieldObjectPositionEvent EntityObjectDestroyed { get; private set; }

        private void AddGeneratedObjectMinorPartAnimatedDisappearanceEventsListeners(T1 generatedObjectBehaviour)
        {
            AnimationPassingEvents<UnityEvent> generatedObjectMinorPartAnimatedDisappearanceEvents = generatedObjectBehaviour.MinorPartAnimatedDisappearance;

            generatedObjectMinorPartAnimatedDisappearanceEvents.Started.AddListener(OnGeneratedObjectMinorPartAnimatedDisappearanceStarted);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively()),
                generatedObjectMinorPartAnimatedDisappearanceEvents.Ended);
        }

        private void EditObjectGeneratedInfo(GameObject obj, Vector2Int? position)
        {
            EntityInfo.Object = obj;
            EntityInfo.Position = position;
        }

        private void RemoveGeneratedObjectMinorPartAnimatedDisappearanceEventsListeners(T1 generatedObjectBehaviour)
        {
            AnimationPassingEvents<UnityEvent> generatedObjectMinorPartAnimatedDisappearanceEvents = generatedObjectBehaviour.MinorPartAnimatedDisappearance;

            generatedObjectMinorPartAnimatedDisappearanceEvents.Started.RemoveListener(OnGeneratedObjectMinorPartAnimatedDisappearanceStarted);
            eventsListenersService.RemoveOrdinaryEventListener(generatedObjectMinorPartAnimatedDisappearanceEvents.Ended);
        }

        private IEnumerator ProcessObjectAnimatedlyLaunchedIteratively()
        {
            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(new FieldEntityPrimalStatusData(FieldEntityAvailabilityStatus.Available,
                FieldEntityFinalizationStatus.ReadyForFinalization));

            EntityLaunched.Invoke();
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            materializedObjectElementColorService = SharedSceneServicesLocator.GetService<MaterializedObjectElementColorService>();
        }

        protected override void RemoveObjectBehaviourEventsListeners(T1 objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            objectBehaviour.AnimatedlyDisappeared.RemoveListener(OnGeneratedObjectAnimatedlyDisappeared);
            RemoveGeneratedObjectMinorPartAnimatedDisappearanceEventsListeners(objectBehaviour);
        }

        protected override void ResumeEntityInternally()
        {
            EntityInfo.Object.GetComponent<T1>().Resume();
        }

        protected override void SuspendEntityInternally()
        {
            EntityInfo.Object.GetComponent<T1>().Suspend();
        }

        protected virtual IEnumerator ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively()
        {
            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));
        }

        protected void GenerateObject(IDictionary<Vector2Int, GameObject> freePlatforms, ICollection<Vector2Int> availablePositions, bool isApplyRootMotion = false,
            Func<Vector2Int, object> customObjectSetupParameterExtractor = null, Delegate customAdditionalObjectSetupAction = null)
        {
            Vector2Int objectPosition = availablePositions.ElementAt(UnityEngine.Random.Range(0, availablePositions.Count));
            GameObject obj = Instantiate(entityObjectSettings.Prefab, freePlatforms[objectPosition].transform.position + new Vector3(0, entityObjectSettings.Displacement, 0),
                Quaternion.identity);
            object customObjectSetupParameter = null;

            EditObjectGeneratedInfo(obj, objectPosition);
            EntityPlaced.Invoke(objectPosition);

            obj.name = entityObjectSettings.InstanceName;

            Animator animator = obj.GetComponent<Animator>();

            animator.runtimeAnimatorController = entityObjectSettings.AnimatorController;
            animator.applyRootMotion = isApplyRootMotion;
            animator.updateMode = animator.applyRootMotion ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
            animator.cullingMode = (animator.updateMode == AnimatorUpdateMode.AnimatePhysics) ? AnimatorCullingMode.CullCompletely : AnimatorCullingMode.AlwaysAnimate;

            if (customObjectSetupParameterExtractor != null)
                customObjectSetupParameter = customObjectSetupParameterExtractor(objectPosition);

            if (customObjectSetupParameter != null)
                ((Action<GameObject, object>)customAdditionalObjectSetupAction).Invoke(obj, customObjectSetupParameter);
            else
                ((Action<GameObject>)customAdditionalObjectSetupAction).Invoke(obj);

            CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, customObjectSetupParameter);
        }

        protected override T1 CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            T1 objectBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            objectBehaviour.AnimatedlyDisappeared.AddListener(OnGeneratedObjectAnimatedlyDisappeared);
            AddGeneratedObjectMinorPartAnimatedDisappearanceEventsListeners(objectBehaviour);

            return objectBehaviour;
        }

        public void RunObject()
        {
            EntityInfo.Object.GetComponent<T1>().TryRun();
        }

        public Vector2Int GetObjectCheckableInfo()
        {
            return EntityInfo.Position.Value;
        }

        public IEnumerator BeginObjectHidingIteratively()
        {
            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(new FieldEntityPrimalStatusData());

            EntityInfo.Object.GetComponent<T1>().BeginHiding();
        }

        private void OnGeneratedObjectAnimatedlyDisappeared()
        {
            Vector2Int generatedObjectPosition = EntityInfo.Position.Value;

            Destroy(EntityInfo.Object);
            EditObjectGeneratedInfo(null, null);
            EntityObjectDestroyed.Invoke(generatedObjectPosition);
        }

        private void OnGeneratedObjectMinorPartAnimatedDisappearanceStarted()
        {
            StartCoroutine(EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable)));
        }

        protected override void OnObjectAnimatedlyLaunched(GameObject obj, Vector2Int objectPosition)
        {
            StartCoroutine(ProcessObjectAnimatedlyLaunchedIteratively());
        }
    }
}

namespace GameScene.Managers.Field.Events
{
    public class FieldObjectPositionEvent : UnityEvent<Vector2Int> { }
}

namespace GameScene.Managers.Field.Info
{
    public class GeneratedFieldObjectInfo : ChildFieldEntityInfo
    {
        public Vector2Int? Position { get; set; }

        public GameObject Object { get; set; }
    }
}

namespace GameScene.Managers.Field.Interfaces
{
    public interface ICheckableGeneratedFieldObjectManager<T>
    {
        T GetObjectCheckableInfo();
    }

    public interface ICheckableGeneratedFieldObjectManager<T1, T2> where T2 : struct
    {
        T1 GetObjectCheckableInfo(T2 parameter);
    }

    public interface IFlushableGeneratedFieldObjectManager
    {
        UnityEvent ObjectPartsFlushed { get; }
    }
}

namespace GameScene.Managers.Field.Settings
{
    public abstract class GeneratedFieldObjectPartSettings<T1, T2> : SimpleEntityObjectSettings where T1 : IUnitedlyGettableEntityCategorySettings<Color, T2> where T2 : Enum
    {
        [SerializeField]
        private T1 colors;

        public T1 Colors
        {
            get
            {
                return colors;
            }
        }
    }

    public abstract class GeneratedFieldObjectSettings<T1, T2, T3> : FieldObjectSettings, IPartialEntityObjectSettings<T1>, ISingleAnimatorControllerEntityObjectSettings 
        where T1 : GeneratedFieldObjectPartSettings<T2, T3> where T2 : IUnitedlyGettableEntityCategorySettings<Color, T3> where T3 : Enum
    {
        [SerializeField]
        private string rootName;

        [SerializeField]
        private RuntimeAnimatorController animatorController;

        [SerializeField]
        private T1 partSettings;

        public string RootName
        {
            get
            {
                return rootName;
            }
        }

        public RuntimeAnimatorController AnimatorController
        {
            get
            {
                return animatorController;
            }
        }

        public T1 PartSettings
        {
            get
            {
                return partSettings;
            }
        }
    }
}