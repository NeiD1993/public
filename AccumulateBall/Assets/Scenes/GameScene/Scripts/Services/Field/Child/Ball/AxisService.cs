using GameScene.Behaviours.Ball.Data;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Services.Ball.Enums;
using UnityEngine.EventSystems;

namespace GameScene.Services.Ball
{
    public class AxisService : BaseSharedService
    {
        private static AxisMoveDirections GetMoveDirections(AxisType axisType)
        {
            if (axisType == AxisType.Horizontal)
                return new AxisMoveDirections() { Direct = MoveDirection.Left, Inverse = MoveDirection.Right };
            else
                return new AxisMoveDirections() { Direct = MoveDirection.Down, Inverse = MoveDirection.Up };
        }

        public void Reverse(AxisType axisType, PositioningData positioningData)
        {
            AxisMoveDirections axisMoveDirections = GetMoveDirections(axisType);
            CardinalPoint tempCardinalPoint = positioningData.CardinalPoints[axisMoveDirections.Direct];

            positioningData.CardinalPoints[axisMoveDirections.Direct] = positioningData.CardinalPoints[axisMoveDirections.Inverse];
            positioningData.CardinalPoints[axisMoveDirections.Inverse] = tempCardinalPoint;
            positioningData.TotalOrientation = (positioningData.TotalOrientation == Orientation.Inverse) ? Orientation.Direct : Orientation.Inverse;
        }

        public AxisType GetTypeByMoveDirection(MoveDirection moveDirection)
        {
            return ((moveDirection == MoveDirection.Down) || (moveDirection == MoveDirection.Up)) ? AxisType.Vertical : AxisType.Horizontal;
        }

        private struct AxisMoveDirections
        {
            public MoveDirection Direct { get; set; }

            public MoveDirection Inverse { get; set; }
        }
    }
}

namespace GameScene.Services.Ball.Enums
{
    public enum AxisType
    {
        Horizontal,

        Vertical
    }
}