using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Services.Platform.Data;
using ServicesLocators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Services.Platform
{
    public class NeighboringPlatformsFinder : BaseSharedService
    {
        private readonly PositionService positionService;

        public NeighboringPlatformsFinder()
        {
            positionService = SharedSceneServicesLocator.GetService<PositionService>();
        }

        private static SortedSet<MoveDirection> GetPositionMoveDirections(Vector2Int position, int dimensionHalfPlatformsCount)
        {
            SortedSet<MoveDirection> moveDirections = new SortedSet<MoveDirection>();

            if ((position.x == -dimensionHalfPlatformsCount) || (position.x == dimensionHalfPlatformsCount))
            {
                if (position.x == -dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Left);
                else if (position.x == dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Right);

                if (position.y == -dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Down);
                else if (position.y == dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Up);
                else
                {
                    moveDirections.Add(MoveDirection.Down);
                    moveDirections.Add(MoveDirection.Up);
                }
            }
            else if ((position.y == -dimensionHalfPlatformsCount) || (position.y == dimensionHalfPlatformsCount))
            {
                if (position.y == -dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Down);
                else if (position.y == dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Up);

                if (position.x == -dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Left);
                else if (position.x == dimensionHalfPlatformsCount)
                    moveDirections.Add(MoveDirection.Right);
                else
                {
                    moveDirections.Add(MoveDirection.Left);
                    moveDirections.Add(MoveDirection.Right);
                }
            }
            else
            {
                moveDirections.Add(MoveDirection.Down);
                moveDirections.Add(MoveDirection.Left);
                moveDirections.Add(MoveDirection.Right);
                moveDirections.Add(MoveDirection.Up);
            }

            return moveDirections;
        }

        private IEnumerable<T> GetNeighboringPlatformsDataObjectIteratively<T>(Vector2Int position, int dimensionHalfPlatformsCount, 
            Func<MoveDirection, Vector2Int, T> neighboringPlatformDataObjectCreatingFunction, Func<Vector2Int, bool> filteringConditionFunction)
        {
            Vector2Int movePosition;
            ISet<MoveDirection> moveDirections = GetPositionMoveDirections(position, dimensionHalfPlatformsCount);

            foreach (MoveDirection moveDirection in moveDirections)
            {
                movePosition = positionService.MovePosition(position, moveDirection);

                if (filteringConditionFunction(movePosition))
                    yield return neighboringPlatformDataObjectCreatingFunction(moveDirection, movePosition);
            }
        }

        private IEnumerable<T> GetNeighboringPlatformsDataObjectIteratively<T>(Vector2Int position, int dimensionHalfPlatformsCount, 
            Func<MoveDirection, Vector2Int, T> neighboringPlatformDataObjectCreatingFunction, ISet<Vector2Int> forbiddenPositions)
        {
            return GetNeighboringPlatformsDataObjectIteratively(position, dimensionHalfPlatformsCount, neighboringPlatformDataObjectCreatingFunction, 
                (movePositionParameter) => !forbiddenPositions.Contains(movePositionParameter));
        }

        public IEnumerable<Vector2Int> GetNeighboringPlatformsPositionsIteratively(Vector2Int position, int dimensionHalfPlatformsCount, ISet<Vector2Int> forbiddenPositions)
        {
            return GetNeighboringPlatformsDataObjectIteratively(position, dimensionHalfPlatformsCount, (moveDirection, movePosition) => movePosition, forbiddenPositions);
        }

        public IList<NeighboringPlatformData> GetNeighboringPlatformsData(Vector2Int position, int dimensionHalfPlatformsCount, ISet<Vector2Int> forbiddenPositions)
        {
            return GetNeighboringPlatformsDataObjectIteratively(position, dimensionHalfPlatformsCount, 
                (moveDirection, movePosition) => new NeighboringPlatformData(moveDirection, movePosition), forbiddenPositions).ToList();
        }

        public IDictionary<Vector2Int, MoveDirection> GetNeighboringPlatformsMoveDirectionsItems(Vector2Int position, int dimensionHalfPlatformsCount, 
            Func<Vector2Int, bool> filteringConditionFunction)
        {
            return GetNeighboringPlatformsDataObjectIteratively(position, dimensionHalfPlatformsCount, 
                (moveDirection, movePosition) => new KeyValuePair<Vector2Int, MoveDirection>(movePosition, moveDirection), filteringConditionFunction)
                .ToDictionary(neighboringPlatformMoveDirectionItemParameter => neighboringPlatformMoveDirectionItemParameter.Key, 
                neighboringPlatformMoveDirectionItemParameter => neighboringPlatformMoveDirectionItemParameter.Value);
        }
    }
}

namespace GameScene.Services.Platform.Data
{
    public struct NeighboringPlatformData
    {
        public NeighboringPlatformData(MoveDirection moveDirection, Vector2Int position)
        {
            MoveDirection = moveDirection;
            Position = position;
        }

        public MoveDirection MoveDirection { get; private set; }

        public Vector2Int Position { get; private set; }
    }
}