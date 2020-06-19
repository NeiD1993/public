using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.Rotator;
using GameScene.Managers.Field;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Field.Settings;
using GameScene.Managers.Rotators.Info;
using GameScene.Managers.Rotators.Settings;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Subscription.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Rotators
{
    public class RotatorsManager : BaseChildFieldEntityCreatingManager<RotatorBehaviour, RotatorSettings, RotatorsInfo>, 
        INotifiedlyDestroyableByObjectChildFieldEntityManager<UnityEvent>
    {
        public RotatorsManager()
        {
            EntityObjectDestroyed = new UnityEvent();
            RotatorPreparedForRotation = new UnityEvent();
            RotatorRotated = new UnityEvent();
        }

        public override GameObject Entity
        {
            get
            {
                return EntityInfo.Rotators;
            }
        }

        public UnityEvent EntityObjectDestroyed { get; private set; }

        public UnityEvent RotatorPreparedForRotation { get; private set; }

        public UnityEvent RotatorRotated { get; private set; }

        private void СhangeRotatorRotationState(Vector2Int rotatorPosition, RotatorRotationState rotatorRotationState)
        {
            RotatorBehaviour rotatorBehaviour = EntityInfo.FreeObjects[rotatorPosition].GetComponent<RotatorBehaviour>();

            switch (rotatorRotationState)
            {
                case RotatorRotationState.PreparedForRotation:
                    rotatorBehaviour.PrepareForRotation();
                    break;
                default:
                    rotatorBehaviour.BeginRotation();
                    break;
            }
        }

        private void CreateRotator(IDictionary<Vector2Int, GameObject> freePlatforms, GeneratedRotatorSettings generatedRotatorSettings, 
            RuntimeAnimatorController rotatorAnimatorController)
        {
            GameObject rotator = Instantiate(entityObjectSettings.Prefab, freePlatforms[generatedRotatorSettings.Position].transform.position +
                new Vector3(0, entityObjectSettings.Displacement, 0), Quaternion.identity);

            rotator.name = GetObjectNameOnPosition(string.Concat(generatedRotatorSettings.Type, entityObjectSettings.InstanceName), generatedRotatorSettings.Position);

            AttachToEntity(rotator);

            rotator.GetComponent<Animator>().runtimeAnimatorController = rotatorAnimatorController;

            CreateAndInitiallySetupObjectBehaviour(rotator, generatedRotatorSettings.Position, generatedRotatorSettings.Type);
        }

        private IEnumerator CreateRotatorsIteratively(IDictionary<Vector2Int, GameObject> freePlatforms, IEnumerable<GeneratedRotatorSettings> generatedRotatorsSettings,
            bool withNotifying)
        {
            RuntimeAnimatorController rotatorAnimatorController;

            EntityInfo.Rotators = new GameObject(entityObjectSettings.OwnerInstanceName);
            EntityPlaced.Invoke();

            foreach (GeneratedRotatorSettings generatedRotatorSettings in generatedRotatorsSettings)
            {
                rotatorAnimatorController = (generatedRotatorSettings.Type == RotationType.Clockwise) ? entityObjectSettings.AnimatorControllers.ClockwiseRotator :
                    entityObjectSettings.AnimatorControllers.CounterClockwiseRotator;
                CreateRotator(freePlatforms, generatedRotatorSettings, rotatorAnimatorController);

                yield return new WaitUntil(() => EntityInfo.FreeObjects.ContainsKey(generatedRotatorSettings.Position));
            }

            yield return EntityInfo.PrimalStatusData
                .SetStatusIteratively(new FieldEntityPrimalStatusData(FieldEntityAvailabilityStatus.Available, FieldEntityFinalizationStatus.ReadyForFinalization));

            if (withNotifying)
                EntityLaunched.Invoke();
        }

        protected override void RemoveObjectBehaviourEventsListeners(RotatorBehaviour objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            objectBehaviour.AnimatedlyRotated.RemoveListener(OnRotatorAnimatedlyRotated);
        }

        protected override IEnumerator ProcessCreatedObjectBlockingAnimationEndedIteratively()
        {
            yield return base.ProcessCreatedObjectBlockingAnimationEndedIteratively();

            RotatorPreparedForRotation.Invoke();
        }

        protected override RotatorBehaviour CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            RotatorBehaviour rotatorBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            rotatorBehaviour.Setup((RotationType)parameters[0]);
            rotatorBehaviour.AnimatedlyRotated.AddListener(OnRotatorAnimatedlyRotated);

            return rotatorBehaviour;
        }

        public void BeginRotatorRotation(Vector2Int rotatorPosition)
        {
            СhangeRotatorRotationState(rotatorPosition, RotatorRotationState.Rotated);
        }

        public void PrepareRotatorForRotation(Vector2Int rotatorPosition)
        {
            СhangeRotatorRotationState(rotatorPosition, RotatorRotationState.PreparedForRotation);
        }

        public bool IsRotatorPosition(Vector2Int possibleRotatorPosition)
        {
            return EntityInfo.FreeObjects.ContainsKey(possibleRotatorPosition);
        }

        public RotationType GetRotatorRotationType(Vector2Int possibleRotatorPosition)
        {
            return IsRotatorPosition(possibleRotatorPosition) ? EntityInfo.FreeObjects[possibleRotatorPosition].GetComponent<RotatorBehaviour>().FeaturesInfo.Type : RotationType.None;
        }

        public IEnumerator BeginRotatorHidingIteratively(Vector2Int rotatorPosition)
        {
            IEnumerator ProcessRotatorAnimatedlyDisappearedIteratively(GameObject rotator)
            {
                yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));

                EntityObjectDestroyed.Invoke();
            }

            RotatorBehaviour rotatorBehaviour = EntityInfo.FreeObjects[rotatorPosition].GetComponent<RotatorBehaviour>();

            eventsListenersService.AddUnsubscribingEventListener(rotatorParameter => StartCoroutine(ProcessRotatorAnimatedlyDisappearedIteratively(rotatorParameter)),
                new EventsWithUnsubscribingListeners<GameObject>(rotatorBehaviour.AnimatedlyDisappeared, rotatorBehaviour.Destroyed));

            yield return EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable));

            rotatorBehaviour.BeginHiding();
        }

        public IEnumerator CreateRotatorsIteratively(IDictionary<Vector2Int, GameObject> freePlatforms, IEnumerable<GeneratedRotatorSettings> generatedRotatorsSettings)
        {
            yield return CreateRotatorsIteratively(freePlatforms, generatedRotatorsSettings, true);
        }

        public IEnumerator CreateRotatorsWithEntityPrimalStatusTogglingIteratively(IDictionary<Vector2Int, GameObject> freePlatforms,
            IEnumerable<GeneratedRotatorSettings> generatedRotatorsSettings)
        {
            yield return PerformRoutineWithEntityPrimalStatusTogglingIteratively(CreateRotatorsIteratively(freePlatforms, generatedRotatorsSettings, false),
                new EntityPrimalStatusOnlyAvailabilityDisablingMannerTogglingRoutinesInfo(true));

            EntityLaunched.Invoke();
        }

        public IEnumerator HideRotatorsIteratively()
        {
            if (EntityInfo.FreeObjects.Count > 0)
            {
                KeyValuePair<Vector2Int, GameObject> rotatorInfoItem;

                while (EntityInfo.FreeObjects.Count > 0)
                {
                    rotatorInfoItem = EntityInfo.FreeObjects.First();
                    rotatorInfoItem.Value.GetComponent<RotatorBehaviour>().BeginHiding();

                    yield return new WaitUntil(() => EntityInfo.FreeObjects.ContainsKey(rotatorInfoItem.Key));
                }
            }
            else
                Destroy(EntityInfo.Rotators);
        }

        public override IEnumerator RunEntityIteratively()
        {
            foreach (GameObject rotator in EntityInfo.FreeObjects.Values)
                yield return rotator.GetComponent<RotatorBehaviour>().TryRun();
        }

        private void OnRotatorAnimatedlyRotated()
        {
            RotatorRotated.Invoke();
        }

        private enum RotatorRotationState
        {
            PreparedForRotation,

            Rotated
        }
    }
}

namespace GameScene.Managers.Rotators.Info
{
    public class RotatorsInfo : CreatedFieldEntityInfo
    {
        public GameObject Rotators { get; set; }
    }
}

namespace GameScene.Managers.Rotators.Settings
{
    public struct GeneratedRotatorSettings
    {
        public GeneratedRotatorSettings(Vector2Int position, RotationType type)
        {
            Position = position;
            Type = type;
        }

        public RotationType Type { get; private set; }

        public Vector2Int Position { get; private set; }
    }

    [Serializable]
    public struct RotatorAnimatorControllersSettings
    {
        [SerializeField]
        private RuntimeAnimatorController clockwiseRotator;

        [SerializeField]
        private RuntimeAnimatorController counterClockwiseRotator;

        public RuntimeAnimatorController ClockwiseRotator
        {
            get
            {
                return clockwiseRotator;
            }
        }

        public RuntimeAnimatorController CounterClockwiseRotator
        {
            get
            {
                return counterClockwiseRotator;
            }
        }
    }

    [Serializable]
    public class RotatorSettings : CreatedFieldObjectSettings
    {
        [SerializeField]
        private RotatorAnimatorControllersSettings animatorControllers;

        public RotatorAnimatorControllersSettings AnimatorControllers
        {
            get
            {
                return animatorControllers;
            }
        }
    }
}