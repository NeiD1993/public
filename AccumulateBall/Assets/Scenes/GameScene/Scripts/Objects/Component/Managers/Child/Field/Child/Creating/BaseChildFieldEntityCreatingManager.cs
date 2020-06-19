using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Settings;
using GameScene.Objects.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Field
{
    public abstract class BaseChildFieldEntityCreatingManager<T1, T2, T3> : BaseChildFieldEntityManager<T1, T2, T3, UnityEvent>, IIterativelyRunnableEntityManager
        where T1 : MonoBehaviour, IBlockableCreatedFieldObjectBehaviour, INotifiedlyHideableMaterializedObjectBehaviour<MaterializedObjectBehaviourEvent>,
        ILaunchableFieldObjectBehaviour, INotifiedlyDestroyableObject, IRetainableFieldObjectBehaviour where T2 : CreatedFieldObjectSettings
        where T3 : CreatedFieldEntityInfo, new()
    {
        protected static void RetainObjects(IEnumerable<GameObject> objects, bool isSuspensionAction = true)
        {
            Action<T1> objectBehaviourRetentionAction;

            if (isSuspensionAction)
                objectBehaviourRetentionAction = objectBehaviourParameter => objectBehaviourParameter.Suspend();
            else
                objectBehaviourRetentionAction = objectBehaviourParameter => objectBehaviourParameter.Resume();

            foreach (GameObject obj in objects)
                objectBehaviourRetentionAction(obj.GetComponent<T1>());
        }

        protected static string GetObjectNameOnPosition(string objectName, Vector2Int objectPosition)
        {
            return string.Concat(objectName, objectPosition);
        }

        private void AddCreatedObjectBlockingAnimationPassingEventsListeners(T1 createdObjectBehaviour)
        {
            AnimationPassingEvents<UnityEvent> createdObjectBlockingAnimationPassingEvents = createdObjectBehaviour.BlockingAnimation;

            createdObjectBlockingAnimationPassingEvents.Started.AddListener(OnCreatedObjectBlockingAnimationStarted);
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessCreatedObjectBlockingAnimationEndedIteratively()),
                createdObjectBlockingAnimationPassingEvents.Ended);
        }

        private void RemoveCreatedObjectBlockingAnimationPassingEventsListeners(T1 createdObjectBehaviour)
        {
            AnimationPassingEvents<UnityEvent> createdObjectBlockingAnimationPassingEvents = createdObjectBehaviour.BlockingAnimation;

            createdObjectBlockingAnimationPassingEvents.Started.RemoveListener(OnCreatedObjectBlockingAnimationStarted);
            eventsListenersService.RemoveOrdinaryEventListener(createdObjectBlockingAnimationPassingEvents.Ended);
        }

        protected override void RemoveObjectBehaviourEventsListeners(T1 objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            eventsListenersService.RemoveOrdinaryEventListener(objectBehaviour.AnimatedlyDisappeared);
            RemoveCreatedObjectBlockingAnimationPassingEventsListeners(objectBehaviour);
        }

        protected override void ResumeEntityInternally()
        {
            RetainObjects(EntityInfo.FreeObjects.Values, false);
        }

        protected override void SuspendEntityInternally()
        {
            RetainObjects(EntityInfo.FreeObjects.Values);
        }

        protected IEnumerator PerformRoutineWithEntityPrimalStatusTogglingIteratively(IEnumerator objectsRoutine, 
            BaseEntityPrimalStatusTogglingRoutinesInfo togglingRoutinesInfo)
        {
            (IEnumerator input, IEnumerator output) GetEntityPrimalStatusTogglingRoutines()
            {
                IEnumerator GetEntityPrimalStatusTogglingRoutine(bool isAvailabilityEnablingMannerRoutine = false)
                {
                    IEnumerator ToggleEntityPrimalStatusInAvailabilityEnablingMannerIteratively(Func<FieldEntityFinalizationStatus> entityFinalizationStatusAfterAvailabilityEnablingExtractor)
                    {
                        FieldEntityFinalizationStatus entityFinalizationStatusAfterAvailabilityEnabling = 
                            (entityFinalizationStatusAfterAvailabilityEnablingExtractor != null) ? entityFinalizationStatusAfterAvailabilityEnablingExtractor() : 
                            SummaryEntityPrimalStatus.Finalization;

                        yield return EntityInfo.PrimalStatusData.SetStatusIteratively(new FieldEntityPrimalStatusData(FieldEntityAvailabilityStatus.Available, 
                            entityFinalizationStatusAfterAvailabilityEnabling));
                    }

                    return isAvailabilityEnablingMannerRoutine ? ToggleEntityPrimalStatusInAvailabilityEnablingMannerIteratively(((EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo)togglingRoutinesInfo).EntityFinalizationStatusAfterAvailabilityEnablingExtractor) : 
                        EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable));
                }

                switch (togglingRoutinesInfo.Manner)
                {
                    case EntityPrimalStatusTogglingRoutinesManner.AvailabilityEnabling:
                        return (((EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo)togglingRoutinesInfo).AreLonely ? null :
                            GetEntityPrimalStatusTogglingRoutine(), GetEntityPrimalStatusTogglingRoutine(true));
                    default:
                        {
                            if (((EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo)togglingRoutinesInfo).AreInput)
                                return (GetEntityPrimalStatusTogglingRoutine(), null);
                            else
                                return (null, GetEntityPrimalStatusTogglingRoutine());
                        }
                }
            }

            (IEnumerator input, IEnumerator output) entityPrimalStatusTogglingRoutines = GetEntityPrimalStatusTogglingRoutines();

            if (entityPrimalStatusTogglingRoutines.input != null)
                yield return entityPrimalStatusTogglingRoutines.input;

            yield return objectsRoutine;

            if (entityPrimalStatusTogglingRoutines.output != null)
                yield return entityPrimalStatusTogglingRoutines.output;
        }

        protected virtual IEnumerator ProcessCreatedObjectBlockingAnimationEndedIteratively()
        {
            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));
        }

        protected override T1 CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            T1 objectBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            eventsListenersService.AddOrdinaryEventListener(createdObjectParameter => OnCreatedObjectAnimatedlyDisappeared(createdObjectParameter, objectPosition),
                objectBehaviour.AnimatedlyDisappeared);
            AddCreatedObjectBlockingAnimationPassingEventsListeners(objectBehaviour);

            return objectBehaviour;
        }

        public abstract IEnumerator RunEntityIteratively();

        private void OnCreatedObjectBlockingAnimationStarted()
        {
            StartCoroutine(EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable)));
        }

        protected virtual void OnCreatedObjectAnimatedlyDisappeared(GameObject obj, Vector2Int objectPosition)
        {
            Destroy(obj);
            EntityInfo.FreeObjects.Remove(objectPosition);

            if (EntityInfo.FreeObjects.Count == 0)
                Destroy(Entity);
        }

        protected override void OnObjectAnimatedlyLaunched(GameObject obj, Vector2Int objectPosition)
        {
            EntityInfo.FreeObjects.Add(objectPosition, obj);
        }

        protected enum EntityPrimalStatusTogglingRoutinesManner
        {
            AvailabilityEnabling,

            OnlyAvailabilityDisabling
        }

        protected class EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo : BaseEntityPrimalStatusTogglingRoutinesInfo
        {
            public EntityPrimalStatusAvailabilityEnablingMannerTogglingRoutinesInfo(bool areLonely = false, 
                Func<FieldEntityFinalizationStatus> entityFinalizationStatusAfterAvailabilityEnablingExtractor = null) : base(EntityPrimalStatusTogglingRoutinesManner.AvailabilityEnabling)
            {
                AreLonely = areLonely;
                EntityFinalizationStatusAfterAvailabilityEnablingExtractor = entityFinalizationStatusAfterAvailabilityEnablingExtractor;
            }

            public bool AreLonely { get; private set; }

            public Func<FieldEntityFinalizationStatus> EntityFinalizationStatusAfterAvailabilityEnablingExtractor { get; private set; }
        }

        protected class EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo : BaseEntityPrimalStatusTogglingRoutinesInfo
        {
            public EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo(bool areInput = false) : 
                base(EntityPrimalStatusTogglingRoutinesManner.OnlyAvailabilityDisabling)
            {
                AreInput = areInput;
            }

            public bool AreInput { get; private set; }
        }

        protected abstract class BaseEntityPrimalStatusTogglingRoutinesInfo
        {
            public BaseEntityPrimalStatusTogglingRoutinesInfo(EntityPrimalStatusTogglingRoutinesManner manner)
            {
                Manner = manner;
            }

            public EntityPrimalStatusTogglingRoutinesManner Manner { get; private set; }
        }
    }
}

namespace GameScene.Managers.Field.Info
{
    public class CreatedFieldEntityInfo : ChildFieldEntityInfo
    {
        private IDictionary<Vector2Int, GameObject> freeObjects;

        public IDictionary<Vector2Int, GameObject> FreeObjects
        {
            get
            {
                if (freeObjects == null)
                    freeObjects = new Dictionary<Vector2Int, GameObject>();

                return freeObjects;
            }
        }
    }
}

namespace GameScene.Managers.Field.Settings
{
    [Serializable]
    public class CreatedFieldObjectSettings : FieldObjectSettings, IOwnedEntityObjectSettings
    {
        [SerializeField]
        private string ownerInstanceName;

        public string OwnerInstanceName
        {
            get
            {
                return ownerInstanceName;
            }
        }
    }
}