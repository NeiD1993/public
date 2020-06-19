using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.AnimationStateMachine.Ball;
using GameScene.Behaviours.AnimationStateMachine.Ball.Enums;
using GameScene.Behaviours.AnimationStateMachine.Ball.Events;
using GameScene.Behaviours.AnimationStateMachine.Events;
using GameScene.Behaviours.Ball.Data;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Ball.Events;
using GameScene.Behaviours.Ball.Info;
using GameScene.Behaviours.FieldObject;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Info.Interfaces;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Services.Ball;
using GameScene.Services.Ball.Data;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Platform.Events;
using GameScene.Services.Subscription.Events;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Behaviours.Ball
{
    public partial class BallBehaviour : BaseGeneratedFieldObjectBehaviour<BallAnimatorInfo, BallShellAnimationStateMachineBehaviour, BallPartAnimationStateMachineBehaviour, 
        BallInfo, BallAnimatorControllerLayer, BallAnimatorControllerParameter>, ICheckableGeneratedFieldObjectBehaviour<BallInfo>, 
        ICheckableGeneratedFieldObjectBehaviour<ISet<CardinalPoint>>, ICheckableGeneratedFieldObjectBehaviour<CardinalPoint, MoveDirection>, 
        IHoldableFieldObjectBehaviour<BallPartHoldingAnimationExitedEvent>
    {
        private AxisService axisService;

        private RotationService rotationService;

        public BallBehaviour()
        {
            HallmarksInfo = new BallInfo();
            AnimatedRotationStarted = new UnityEvent();
            AnimatedMovingStarted = new MoveEvent();
            AnimatedlyMoved = new MoveEvent();
            HoldingAnimation = new AnimationPassingEvents<BallPartHoldingAnimationExitedEvent>();
        }

        public UnityEvent AnimatedRotationStarted { get; private set; }

        public MoveEvent AnimatedMovingStarted { get; private set; }

        public MoveEvent AnimatedlyMoved { get; private set; }

        public AnimationPassingEvents<BallPartHoldingAnimationExitedEvent> HoldingAnimation { get; private set; }

        BallInfo ICheckableGeneratedFieldObjectBehaviour<BallInfo>.CreateCheckableData()
        {
            return (BallInfo)HallmarksInfo.Clone();
        }

        ISet<CardinalPoint> ICheckableGeneratedFieldObjectBehaviour<ISet<CardinalPoint>>.CreateCheckableData()
        {
            return HallmarksInfo.CardinalPointsFillingData.HalfFilledCardinalPoints;
        }

        private void AddPartHoldingAnimationStatePassingEventsListeners(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            AnimationStatePassingEvents<BallPartHoldingAnimationStateExitedEvent> partHoldingAnimationStatePassingEvents =
                minorAnimationStateMachineBehaviour.HoldingState;

            partHoldingAnimationStatePassingEvents.Entered.AddListener(OnPartHoldingAnimationStateEntered);
            eventsListenersService.AddOrdinaryEventListener(partKeyAnimationStateTagParameter => 
            OnPartHoldingAnimationStateExited(minorAnimationStateMachineBehaviour.CardinalPoint, partKeyAnimationStateTagParameter), 
            partHoldingAnimationStatePassingEvents.Exited);
        }

        private void RemovePartHoldingAnimationStatePassingEventsListeners(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            AnimationStatePassingEvents<BallPartHoldingAnimationStateExitedEvent> partHoldingAnimationStatePassingEvents = 
                minorAnimationStateMachineBehaviour.HoldingState;

            partHoldingAnimationStatePassingEvents.Entered.RemoveListener(OnPartHoldingAnimationStateEntered);
            eventsListenersService.RemoveOrdinaryEventListener(partHoldingAnimationStatePassingEvents.Exited);
        }

        private void BeginPartFlushing(CardinalPoint cardinalPoint)
        {
            BallAnimatorControllerParameter partFlushingAnimatorControllerParameter;

            switch (cardinalPoint)
            {
                case CardinalPoint.East:
                    partFlushingAnimatorControllerParameter = BallAnimatorControllerParameter.IsEastFlushing;
                    break;
                case CardinalPoint.North:
                    partFlushingAnimatorControllerParameter = BallAnimatorControllerParameter.IsNorthFlushing;
                    break;
                case CardinalPoint.South:
                    partFlushingAnimatorControllerParameter = BallAnimatorControllerParameter.IsSouthFlushing;
                    break;
                default:
                    partFlushingAnimatorControllerParameter = BallAnimatorControllerParameter.IsWestFlushing;
                    break;
            }

            AnimatorInfo.SetParameter(partFlushingAnimatorControllerParameter);
        }

        private BallAnimatorControllerParameter GetRotationAnimatorControllerParameter(RotationType rotationType)
        {
            switch ((HallmarksInfo.PositioningData.TotalOrientation == Orientation.Direct) ? rotationType : rotationService.GetInverseRotationType(rotationType))
            {
                case RotationType.Clockwise:
                    return BallAnimatorControllerParameter.IsClockwiseRotation;
                default:
                    return BallAnimatorControllerParameter.IsCounterClockwiseRotation;
            }
        }

        private IEnumerator RotateIteratively(RotationType rotationType, BallAnimatorControllerParameter rotationAnimatorControllerParameter, bool isAwaitable = false)
        {
            AnimatorInfo.SetParameter(rotationAnimatorControllerParameter);

            if (isAwaitable)
                yield return new WaitForFixedUpdate();

            yield return new WaitUntil(() => 
            {
                BallAnimatorControllerLayer layer = AnimatorInfo.GetMajorLayer();

                return animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), BallShellAnimationStateTag.RotationEnd) && 
                AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
            });

            rotationService.Rotate(rotationType, HallmarksInfo.PositioningData.CardinalPoints);
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            axisService = SharedSceneServicesLocator.GetService<AxisService>();
            rotationService = SharedSceneServicesLocator.GetService<RotationService>();
        }

        protected override void PostProcessMinorAnimationStateMachinePreExiting(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            HallmarksInfo.CardinalPointsFillingData.RemoveFilledCardinalPoint(minorAnimationStateMachineBehaviour.CardinalPoint);
            base.PostProcessMinorAnimationStateMachinePreExiting(minorAnimationStateMachineBehaviour);
        }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(BallShellAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.RotationStateEntered.RemoveListener(OnRotationAnimationStateEntered);
        }

        protected override void RemoveMinorAnimationStateMachinesBehavioursEventsListeners(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            base.RemoveMinorAnimationStateMachinesBehavioursEventsListeners(minorAnimationStateMachineBehaviour);
            RemovePartHoldingAnimationStatePassingEventsListeners(minorAnimationStateMachineBehaviour);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(BallShellAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.RotationStateEntered.AddListener(OnRotationAnimationStateEntered);
        }

        protected override void SetupMinorAnimationStateMachinesBehaviour(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            base.SetupMinorAnimationStateMachinesBehaviour(minorAnimationStateMachineBehaviour);
            AddPartHoldingAnimationStatePassingEventsListeners(minorAnimationStateMachineBehaviour);
        }

        protected override BallAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new BallAnimatorInfo(animator);
        }

        public void BeginMoving(MoveDirection moveDirection)
        {
            BallAnimatorControllerParameter movingAnimatorControllerParameter;

            switch (HallmarksInfo.PositioningData.CardinalPoints[moveDirection])
            {
                case CardinalPoint.East:
                    movingAnimatorControllerParameter = BallAnimatorControllerParameter.IsMovingEast;
                    break;
                case CardinalPoint.North:
                    movingAnimatorControllerParameter = BallAnimatorControllerParameter.IsMovingNorth;
                    break;
                case CardinalPoint.South:
                    movingAnimatorControllerParameter = BallAnimatorControllerParameter.IsMovingSouth;
                    break;
                default:
                    movingAnimatorControllerParameter = BallAnimatorControllerParameter.IsMovingWest;
                    break;
            }

            AnimatorInfo.SetParameter(movingAnimatorControllerParameter);
        }

        public void BeginPartFillingLevelChanging(CardinalPoint cardinalPoint, int fillingLevel)
        {
            BallAnimatorControllerParameter partFillingLevelAnimatorControllerParameter;

            switch (cardinalPoint)
            {
                case CardinalPoint.East:
                    partFillingLevelAnimatorControllerParameter = BallAnimatorControllerParameter.EastFillingLevel;
                    break;
                case CardinalPoint.North:
                    partFillingLevelAnimatorControllerParameter = BallAnimatorControllerParameter.NorthFillingLevel;
                    break;
                case CardinalPoint.South:
                    partFillingLevelAnimatorControllerParameter = BallAnimatorControllerParameter.SouthFillingLevel;
                    break;
                default:
                    partFillingLevelAnimatorControllerParameter = BallAnimatorControllerParameter.WestFillingLevel;
                    break;
            }

            AnimatorInfo.SetParameter(partFillingLevelAnimatorControllerParameter, fillingLevel);
        }

        public void BeginPartsFlushing(FlushingType partsFlushingType)
        {
            Action<CardinalPoint> partFlushingAction;

            switch (partsFlushingType)
            {
                case FlushingType.Half:
                    partFlushingAction = cardinalPointParameter => BeginPartFillingLevelChanging(cardinalPointParameter, 1);
                    break;
                default:
                    partFlushingAction = cardinalPointParameter => BeginPartFlushing(cardinalPointParameter);
                    break;
            }

            foreach (CardinalPoint cardinalPoint in Enum.GetValues(typeof(CardinalPoint)))
                partFlushingAction(cardinalPoint);
        }

        public void PrepareForMoving(MoveDirection moveDirection)
        {
            BallAnimatorControllerParameter preparingForMovingAnimatorControllerParameter;
            BallShellAnimationStateMachineBehaviour shellAnimationStateMachineBehaviour = AnimatorInfo.AnimationStateMachinesBehavioursDescription.Major;

            eventsListenersService.AddUnsubscribingEventListener(() => OnMovingAnimationStateEntered(moveDirection), 
                new EventsWithUnsubscribingListeners(shellAnimationStateMachineBehaviour.MovingStateEntered, shellAnimationStateMachineBehaviour.Destroyed));

            switch (HallmarksInfo.PositioningData.TotalOrientation)
            {
                case Orientation.Direct:
                    preparingForMovingAnimatorControllerParameter = BallAnimatorControllerParameter.IsDirectMoving;
                    break;
                default:
                    preparingForMovingAnimatorControllerParameter = BallAnimatorControllerParameter.IsInverseMoving;
                    break;
            }

            AnimatorInfo.SetParameter(preparingForMovingAnimatorControllerParameter);
        }

        public void ReRun()
        {
            Idle();
        }

        public bool ArePartsFlushed(FlushingType flushingType)
        {
            switch (flushingType)
            {
                case FlushingType.Full:
                    return HallmarksInfo.CardinalPointsFillingData.AreUnfilled;
                default:
                    return HallmarksInfo.CardinalPointsFillingData.AreHalfFilled;
            }
        }

        public CardinalPoint CreateCheckableData(MoveDirection checkableMoveDirection)
        {
            return HallmarksInfo.PositioningData.CardinalPoints[checkableMoveDirection];
        }

        public IEnumerator ApplyRotationsDataIteratively(RotationsData rotationsData)
        {
            RotationType rotationType = rotationsData.Type;

            if (rotationType != RotationType.None)
            {
                BallAnimatorControllerParameter rotationAnimatorControllerParameter = GetRotationAnimatorControllerParameter(rotationType);

                for (int i = 0; i < rotationsData.Amount; i++)
                    yield return RotateIteratively(rotationType, rotationAnimatorControllerParameter, i > 0);

                Idle();
            }
        }

        public IEnumerator RotateIteratively(RotationType rotationType)
        {
            yield return RotateIteratively(rotationType, GetRotationAnimatorControllerParameter(rotationType));

            Idle();
        }

        private void OnMovingAnimationStateEntered(MoveDirection moveDirection)
        {
            IEnumerator ProcessMovingAnimationStateEnteredIteratively()
            {
                AnimatedMovingStarted.Invoke(moveDirection);

                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()));

                axisService.Reverse(axisService.GetTypeByMoveDirection(moveDirection), HallmarksInfo.PositioningData);
                AnimatedlyMoved.Invoke(moveDirection);
            }

            StartCoroutine(ProcessMovingAnimationStateEnteredIteratively());
        }

        private void OnPartHoldingAnimationStateEntered()
        {
            HoldingAnimation.Started.Invoke();
        }

        private void OnPartHoldingAnimationStateExited(CardinalPoint cardinalPoint, BallPartAnimationStateTag partKeyAnimationStateTag)
        {
            BallPartHoldingAnimationType partHoldingAnimationType;

            switch (partKeyAnimationStateTag)
            {
                case BallPartAnimationStateTag.FullFilled:
                    {
                        partHoldingAnimationType = BallPartHoldingAnimationType.FullFilling;
                        HallmarksInfo.CardinalPointsFillingData.AddFullFiledCardinalPoint(cardinalPoint);
                    }
                    break;
                default:
                    {
                        partHoldingAnimationType = BallPartHoldingAnimationType.HalfFlushing;
                        HallmarksInfo.CardinalPointsFillingData.AddHalfFilledCardinalPoint(cardinalPoint);
                    }
                    break;
            }

            HoldingAnimation.Ended.Invoke(partHoldingAnimationType);
        }

        private void OnRotationAnimationStateEntered()
        {
            AnimatedRotationStarted.Invoke();
        }

        private struct BallRotationAnimationInfo : IFieldObjectAnimationInfo<BallShellAnimationStateTag>
        {
            public BallRotationAnimationInfo(BallShellAnimationStateTag endStateTag, BallAnimatorControllerParameter controllerParameter)
            {
                EndStateTag = endStateTag;
                ControllerParameter = controllerParameter;
            }

            public BallShellAnimationStateTag EndStateTag { get; private set; }

            public BallAnimatorControllerParameter ControllerParameter { get; private set; }
        }
    }
}

namespace GameScene.Behaviours.Ball.Data
{
    public class CardinalPointsFillingData : ICloneable
    {
        private ISet<CardinalPoint> fullFilledCardinalPoints;

        private CardinalPointsFillingData(ISet<CardinalPoint> halfFilledCardinalPoints, ISet<CardinalPoint> fullFilledCardinalPoints)
        {
            this.fullFilledCardinalPoints = fullFilledCardinalPoints;
            HalfFilledCardinalPoints = halfFilledCardinalPoints;
        }

        public CardinalPointsFillingData()
        {
            SetupHalfFilledCardinalPoints();
            fullFilledCardinalPoints = new HashSet<CardinalPoint>();
        }

        public bool AreHalfFilled
        {
            get
            {
                return HalfFilledCardinalPoints.Count == Enum.GetValues(typeof(CardinalPoint)).Length;
            }
        }

        public bool AreUnfilled
        {
            get
            {
                return (HalfFilledCardinalPoints.Count == 0) && (fullFilledCardinalPoints.Count == 0);
            }
        }

        public ISet<CardinalPoint> HalfFilledCardinalPoints { get; private set; }

        private void SetupHalfFilledCardinalPoints()
        {
            HalfFilledCardinalPoints = new HashSet<CardinalPoint>()
            {
                CardinalPoint.East,
                CardinalPoint.North,
                CardinalPoint.South,
                CardinalPoint.West
            };
        }

        public void AddHalfFilledCardinalPoint(CardinalPoint cardinalPoint)
        {
            fullFilledCardinalPoints.Remove(cardinalPoint);
            HalfFilledCardinalPoints.Add(cardinalPoint);
        }

        public void AddFullFiledCardinalPoint(CardinalPoint cardinalPoint)
        {
            HalfFilledCardinalPoints.Remove(cardinalPoint);
            fullFilledCardinalPoints.Add(cardinalPoint);
        }

        public void RemoveFilledCardinalPoint(CardinalPoint cardinalPoint)
        {
            if (!HalfFilledCardinalPoints.Remove(cardinalPoint))
                fullFilledCardinalPoints.Remove(cardinalPoint);
        }

        public object Clone()
        {
            return new CardinalPointsFillingData(new HashSet<CardinalPoint>(HalfFilledCardinalPoints), new HashSet<CardinalPoint>(fullFilledCardinalPoints));
        }
    }

    public class PositioningData : ICloneable
    {
        private IDictionary<MoveDirection, CardinalPoint> cardinalPoints;

        private PositioningData(Orientation totalOrientation, IDictionary<MoveDirection, CardinalPoint> cardinalPoints)
        {
            TotalOrientation = totalOrientation;
            CardinalPoints = cardinalPoints;
        }

        public PositioningData()
        {
            TotalOrientation = Orientation.Direct;
        }

        public Orientation TotalOrientation { get; set; }

        public IDictionary<MoveDirection, CardinalPoint> CardinalPoints
        {
            get
            {
                if (cardinalPoints == null)
                {
                    cardinalPoints = new Dictionary<MoveDirection, CardinalPoint>() {
                        { MoveDirection.Down, CardinalPoint.South },
                        { MoveDirection.Left, CardinalPoint.West },
                        { MoveDirection.Right, CardinalPoint.East },
                        { MoveDirection.Up, CardinalPoint.North }
                    };
                }

                return cardinalPoints;
            }

            private set
            {
                cardinalPoints = value;
            }
        }

        public object Clone()
        {
            return new PositioningData(TotalOrientation, new Dictionary<MoveDirection, CardinalPoint>(CardinalPoints));
        }
    }
}

namespace GameScene.Behaviours.Ball.Enums
{
    public enum BallAnimatorControllerLayer
    {
        Eastern,

        Northern,

        Shell,

        Southern,

        Western
    }

    public enum BallAnimatorControllerParameter
    {
        EastFillingLevel,

        IsClockwiseRotation,

        IsCounterClockwiseRotation,

        IsDirectMoving,

        IsDisappearance,

        IsEastFlushing,

        IsIdle,

        IsInverseMoving,

        IsMovingEast,

        IsMovingNorth,

        IsMovingSouth,

        IsMovingWest,

        IsNorthFlushing,

        IsSouthFlushing,

        IsWestFlushing,

        NorthFillingLevel,

        SouthFillingLevel,

        WestFillingLevel
    }

    public enum BallPartHoldingAnimationType
    {
        FullFilling,

        HalfFlushing
    }

    public enum CardinalPoint
    {
        East = 1,

        North = 0,

        South = 2,

        West = 3
    }

    public enum FlushingType
    {
        Full,

        Half
    }

    public enum Orientation
    {
        Direct,

        Inverse
    }
}

namespace GameScene.Behaviours.Ball.Events
{
    public class BallPartHoldingAnimationExitedEvent : UnityEvent<BallPartHoldingAnimationType> { }
}

namespace GameScene.Behaviours.Ball.Info
{
    public class BallAnimatorInfo : BaseMinorlyMultiLayeredGeneratedFieldObjectAnimatorInfo<BallShellAnimationStateMachineBehaviour, BallPartAnimationStateMachineBehaviour, 
        BallAnimatorControllerLayer, BallAnimatorControllerParameter>
    {
        public BallAnimatorInfo(Animator animator) : base(animator) { }

        public override BallAnimatorControllerLayer GetMajorLayer()
        {
            return BallAnimatorControllerLayer.Shell;
        }

        public override BallAnimatorControllerLayer GetMinorLayer(BallPartAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            switch (minorAnimationStateMachineBehaviour.CardinalPoint)
            {
                case CardinalPoint.East:
                    return BallAnimatorControllerLayer.Eastern;
                case CardinalPoint.North:
                    return BallAnimatorControllerLayer.Northern;
                case CardinalPoint.South:
                    return BallAnimatorControllerLayer.Southern;
                default:
                    return BallAnimatorControllerLayer.Western;
            }
        }

        public override BallAnimatorControllerParameter GetDisappearanceParameter()
        {
            return BallAnimatorControllerParameter.IsDisappearance;
        }

        public override BallAnimatorControllerParameter GetIdleParameter()
        {
            return BallAnimatorControllerParameter.IsIdle;
        }
    }

    public class BallInfo : ICloneable
    {
        private BallInfo(CardinalPointsFillingData cardinalPointsFillingData, PositioningData positioningData)
        {
            CardinalPointsFillingData = cardinalPointsFillingData;
            PositioningData = positioningData;
        }

        public BallInfo()
        {
            CardinalPointsFillingData = new CardinalPointsFillingData();
            PositioningData = new PositioningData();
        }

        public CardinalPointsFillingData CardinalPointsFillingData { get; private set; }

        public PositioningData PositioningData { get; private set; }

        public object Clone()
        {
            return new BallInfo((CardinalPointsFillingData)CardinalPointsFillingData.Clone(), (PositioningData)PositioningData.Clone());
        }
    }
}