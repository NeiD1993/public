using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameScene.Behaviours.Ball;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Ball.Events;
using GameScene.Behaviours.Ball.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Managers.Ball.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Events;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Field.Settings;
using GameScene.Services.Ball.Data;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Platform;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Managers.Ball
{
    public class BallManager : BaseChildFieldEntityGenerativeManager<BallBehaviour, BallSettings, BallContentsSettings, BallContentsColorsSettings, CardinalPoint>, 
        ICheckableGeneratedFieldObjectManager<BallInfo>, ICheckableGeneratedFieldObjectManager<ISet<CardinalPoint>>, 
        ICheckableGeneratedFieldObjectManager<CardinalPoint, MoveDirection>
    {
        private PositionService positionService;

        public BallManager()
        {
            BallPartFilled = new UnityEvent();
            BallRotationsDataApplied = new UnityEvent();
            BallRotationStarted = new UnityEvent();
            BallMovingStarted = new FieldObjectPositionEvent();
            BallMoved = new FieldObjectPositionEvent();
        }

        public UnityEvent BallPartFilled { get; private set; }

        public UnityEvent BallRotationsDataApplied { get; private set; }

        public UnityEvent BallRotationStarted { get; private set; }

        public FieldObjectPositionEvent BallMoved { get; private set; }

        public FieldObjectPositionEvent BallMovingStarted { get; private set; }

        BallInfo ICheckableGeneratedFieldObjectManager<BallInfo>.GetObjectCheckableInfo()
        {
            return ((ICheckableGeneratedFieldObjectBehaviour<BallInfo>)EntityInfo.Object.GetComponent<BallBehaviour>()).CreateCheckableData();
        }

        ISet<CardinalPoint> ICheckableGeneratedFieldObjectManager<ISet<CardinalPoint>>.GetObjectCheckableInfo()
        {
            return ((ICheckableGeneratedFieldObjectBehaviour<ISet<CardinalPoint>>)EntityInfo.Object.GetComponent<BallBehaviour>()).CreateCheckableData();
        }

        private void AddBallPartHoldingAnimationPassingEventsListeners(BallBehaviour ballBehaviour)
        {
            AnimationPassingEvents<BallPartHoldingAnimationExitedEvent> ballPartHoldingAnimationPassingEvents = ballBehaviour.HoldingAnimation;

            ballPartHoldingAnimationPassingEvents.Started.AddListener(OnBallPartHoldingAnimationStarted);
            ballPartHoldingAnimationPassingEvents.Ended.AddListener(OnBallPartHoldingAnimationEnded);
        }

        private void RemoveBallPartHoldingAnimationPassingEventsListeners(BallBehaviour ballBehaviour)
        {
            AnimationPassingEvents<BallPartHoldingAnimationExitedEvent> ballPartHoldingAnimationPassingEvents = ballBehaviour.HoldingAnimation;

            ballPartHoldingAnimationPassingEvents.Started.RemoveListener(OnBallPartHoldingAnimationStarted);
            ballPartHoldingAnimationPassingEvents.Ended.RemoveListener(OnBallPartHoldingAnimationEnded);
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            positionService = SharedSceneServicesLocator.GetService<PositionService>();
        }

        protected override void RemoveObjectBehaviourEventsListeners(BallBehaviour objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            objectBehaviour.AnimatedMovingStarted.RemoveListener(OnBallAnimatedMovingStarted);
            objectBehaviour.AnimatedlyMoved.RemoveListener(OnBallAnimatedlyMoved);
            objectBehaviour.AnimatedRotationStarted.RemoveListener(OnBallAnimatedRotationStarted);
            RemoveBallPartHoldingAnimationPassingEventsListeners(objectBehaviour);
        }

        protected override BallBehaviour CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            BallBehaviour ballBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            ballBehaviour.AnimatedMovingStarted.AddListener(OnBallAnimatedMovingStarted);
            ballBehaviour.AnimatedlyMoved.AddListener(OnBallAnimatedlyMoved);
            ballBehaviour.AnimatedRotationStarted.AddListener(OnBallAnimatedRotationStarted);
            AddBallPartHoldingAnimationPassingEventsListeners(ballBehaviour);

            return ballBehaviour;
        }

        protected override IEnumerator ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively()
        {
            yield return base.ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively();

            if (EntityInfo.Object.GetComponent<BallBehaviour>().ArePartsFlushed(FlushingType.Full))
                ObjectPartsFlushed.Invoke();
        }

        public void BeginBallMoving(MoveDirection moveDirection)
        {
            EntityInfo.Object.GetComponent<BallBehaviour>().BeginMoving(moveDirection);
        }

        public void BeginBallPartFilling(CardinalPoint cardinalPoint)
        {
            EntityInfo.Object.GetComponent<BallBehaviour>().BeginPartFillingLevelChanging(cardinalPoint, 2);
        }

        public void BeginBallPartsFlushing(FlushingType ballPartsFlushingType)
        {
            EntityInfo.Object.GetComponent<BallBehaviour>().BeginPartsFlushing(ballPartsFlushingType);
        }

        public void GenerateBall(IDictionary<Vector2Int, GameObject> freePlatforms)
        {
            void SetupContents(EntityPartSetupInfo<BallContentsSettings> ballContentsSetupInfo)
            {
                void SetContentColor((Transform transform, string namePostfix) contentDescription, BallContentsColorsSettings contentsColorsSettings)
                {
                    CardinalPoint DefineContentCardinalPoint((string value, string postfix) contentNameDescription)
                    {
                        string stringCardinalPoint = Regex.Replace(contentNameDescription.value, contentNameDescription.postfix, string.Empty, RegexOptions.RightToLeft);

                        return (CardinalPoint)Enum.Parse(typeof(CardinalPoint), stringCardinalPoint);
                    }

                    GameObject content = contentDescription.transform.gameObject;

                    materializedObjectElementColorService.SetElementColor(content, contentsColorsSettings.GetSettings(DefineContentCardinalPoint((content.name, contentDescription.namePostfix))));
                }

                BallContentsSettings contentsSettings = entityObjectSettings.PartSettings;
                string contentNamePostfix = contentsSettings.InstanceName.TrimEnd(new char[] { (char)115 });

                foreach (Transform contentTransform in ballContentsSetupInfo.Part.transform)
                    SetContentColor((contentTransform, contentNamePostfix), contentsSettings.Colors);
            }

            GenerateObject(freePlatforms, freePlatforms.Keys, true, customAdditionalObjectSetupAction : (Action<GameObject>)((ballParameter) => SetupContents(new EntityPartSetupInfo<BallContentsSettings>(ballParameter, entityObjectSettings))));
        }

        public void PrepareBallForMoving(MoveDirection moveDirection)
        {
            EntityInfo.Object.GetComponent<BallBehaviour>().PrepareForMoving(moveDirection);
        }

        public void ReRunBall()
        {
            EntityInfo.Object.GetComponent<BallBehaviour>().ReRun();
        }

        public CardinalPoint GetObjectCheckableInfo(MoveDirection ballCheckableMoveDirection)
        {
            return EntityInfo.Object.GetComponent<BallBehaviour>().CreateCheckableData(ballCheckableMoveDirection);
        }

        public IEnumerator ApplyBallRotationsDataIteratively(RotationsData ballRotationsData)
        {
            yield return EntityInfo.Object.GetComponent<BallBehaviour>().ApplyRotationsDataIteratively(ballRotationsData);

            BallRotationsDataApplied.Invoke();
        }

        public IEnumerator RotateBallIteratively(RotationType rotationType)
        {
            yield return EntityInfo.Object.GetComponent<BallBehaviour>().RotateIteratively(rotationType);
        }

        private void OnBallAnimatedMovingStarted(MoveDirection moveDirection)
        {
            BallMovingStarted.Invoke(positionService.MovePosition(EntityInfo.Position.Value, moveDirection));
        }

        private void OnBallAnimatedlyMoved(MoveDirection moveDirection)
        {
            Vector2Int previousObjectPosition = EntityInfo.Position.Value;

            EntityInfo.Position = positionService.MovePosition(previousObjectPosition, moveDirection);
            BallMoved.Invoke(previousObjectPosition);
        }

        private void OnBallAnimatedRotationStarted()
        {
            BallRotationStarted.Invoke();
        }

        private void OnBallPartHoldingAnimationEnded(BallPartHoldingAnimationType ballPartHoldingAnimationType)
        {
            IEnumerator ProcessBallPartHoldingAnimationEndedIteratively()
            {
                IEnumerator entityPrimalStatusChangingRoutine = EntityInfo.PrimalStatusData
                    .SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Available));

                switch (ballPartHoldingAnimationType)
                {
                    case BallPartHoldingAnimationType.FullFilling:
                        {
                            yield return entityPrimalStatusChangingRoutine;

                            BallPartFilled.Invoke();
                        }
                        break;
                    default:
                        {
                            if (EntityInfo.Object.GetComponent<BallBehaviour>().ArePartsFlushed(FlushingType.Half))
                            {
                                yield return entityPrimalStatusChangingRoutine;

                                ObjectPartsFlushed.Invoke();
                            }
                        }
                        break;
                }
            }

            StartCoroutine(ProcessBallPartHoldingAnimationEndedIteratively());
        }

        private void OnBallPartHoldingAnimationStarted()
        {
            StartCoroutine(EntityInfo.PrimalStatusData.SetStatusIteratively(SummaryEntityPrimalStatus.GetWithChangedAvailability(FieldEntityAvailabilityStatus.Unavailable)));
        }
    }
}

namespace GameScene.Managers.Ball.Settings
{
    [Serializable]
    public struct BallContentsColorsSettings : IUnitedlyGettableEntityCategorySettings<Color, CardinalPoint>
    {
        [SerializeField]
        private Color east;

        [SerializeField]
        private Color north;

        [SerializeField]
        private Color south;

        [SerializeField]
        private Color west;

        public Color GetSettings(CardinalPoint category)
        {
            switch (category)
            {
                case CardinalPoint.East:
                    return east;
                case CardinalPoint.North:
                    return north;
                case CardinalPoint.South:
                    return south;
                default:
                    return west;
            }
        }
    }

    [Serializable]
    public class BallContentsSettings : GeneratedFieldObjectPartSettings<BallContentsColorsSettings, CardinalPoint> { }

    [Serializable]
    public class BallSettings : GeneratedFieldObjectSettings<BallContentsSettings, BallContentsColorsSettings, CardinalPoint> { }
}