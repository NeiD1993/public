using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private class VariativePathPointsMoveDirectionsItemsPickingService : BaseVariativePathPointsDataPickingService<Vector2Int, MoveDirection>
            {
                protected override Func<KeyValuePair<Vector2Int, MoveDirection>, Vector2Int> CreateVariativePathPointPositionExtractor()
                {
                    return (variativePathPointMoveDirectionItemParameter) => variativePathPointMoveDirectionItemParameter.Key;
                }
            }
        }
    }
}