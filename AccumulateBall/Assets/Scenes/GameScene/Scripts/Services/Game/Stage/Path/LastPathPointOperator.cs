using System.Collections.Generic;
using GameScene.Services.Platform;
using ServicesLocators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private class LastPathPointOperator
            {
                private readonly PositionService positionService;

                public LastPathPointOperator()
                {
                    positionService = SharedSceneServicesLocator.GetService<PositionService>();
                }

                private void RemoveLastPathPoint(ref Vector2Int lastPathPoint, IList<MoveDirection> pathMoveDirections)
                {
                    lastPathPoint = GetPreviousLastPathPoint(lastPathPoint, pathMoveDirections);
                    pathMoveDirections.RemoveAt(pathMoveDirections.Count - 1);
                }

                private Vector2Int GetPreviousLastPathPoint(Vector2Int currentLastPathPoint, IList<MoveDirection> pathMoveDirections)
                {
                    return positionService.MovePosition(currentLastPathPoint,
                        positionService.GetInverseMoveDirection(pathMoveDirections[pathMoveDirections.Count - 1]));
                }

                public void SetLastPathPointAsPossibleReservoirPosition(ref Vector2Int lastPathPoint, IList<MoveDirection> pathMoveDirections, 
                    ISet<Vector2Int> forbiddenReservoirPositions, ICollection<Vector2Int> possibleReservoirPositions)
                {
                    forbiddenReservoirPositions.Remove(lastPathPoint);
                    possibleReservoirPositions.Add(lastPathPoint);
                    RemoveLastPathPoint(ref lastPathPoint, pathMoveDirections);
                }
            }
        }
    }
}