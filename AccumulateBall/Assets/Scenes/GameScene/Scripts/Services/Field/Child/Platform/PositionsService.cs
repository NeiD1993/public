using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameScene.Services.Platform
{
    public class PositionService : BaseSharedService
    {
        private static Vector2Int GetDisplacementByMoveDirection(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.Down:
                    return Vector2Int.up;
                case MoveDirection.Left:
                    return Vector2Int.right;
                case MoveDirection.Right:
                    return Vector2Int.left;
                case MoveDirection.Up:
                    return Vector2Int.down;
                default:
                    return default;
            }
        }

        public float GetDistanceBetweenPositions(Vector2Int startPosition, Vector2Int endPosition)
        {
            return Vector2Int.Distance(startPosition, endPosition);
        }

        public MoveDirection GetMoveDirectionBetweenPositions(Vector2Int startPosition, Vector2Int endPosition)
        {
            MoveDirection moveDirection = MoveDirection.None;

            if (startPosition.x == endPosition.x)
            {
                if (startPosition.y == endPosition.y - 1)
                    moveDirection = MoveDirection.Down;
                else if (startPosition.y == endPosition.y + 1)
                    moveDirection = MoveDirection.Up;
            }
            else if (startPosition.y == endPosition.y)
            {
                if (startPosition.x == endPosition.x - 1)
                    moveDirection = MoveDirection.Left;
                else if (startPosition.x == endPosition.x + 1)
                    moveDirection = MoveDirection.Right;
            }

            return moveDirection;
        }

        public MoveDirection GetInverseMoveDirection(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.Down:
                    return MoveDirection.Up;
                case MoveDirection.Left:
                    return MoveDirection.Right;
                case MoveDirection.Right:
                    return MoveDirection.Left;
                default:
                    return MoveDirection.Down;
            }
        }

        public Vector2Int MovePosition(Vector2Int position, MoveDirection moveDirection)
        {
            return position + GetDisplacementByMoveDirection(moveDirection);
        }
    }
}

namespace GameScene.Services.Platform.Events
{
    public class MoveEvent : UnityEvent<MoveDirection> { }
}