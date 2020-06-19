using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Services.Ball.Enums;
using UnityEngine.EventSystems;

namespace GameScene.Services.Ball
{
    public class RotationService : BaseSharedService
    {
        private static void Rotate(IList<MoveDirectionRotationAccordanceData> moveDirectionsRotationAccordance, IDictionary<MoveDirection, CardinalPoint> cardinalPoints)
        {
            MoveDirection moveDirectionAfterRotation;
            MoveDirectionRotationAccordanceData moveDirectionRotationAccordanceData;
            CardinalPoint cachedCardinalPointBeforeRotation = default;

            for (int i = 0; i < moveDirectionsRotationAccordance.Count; i++)
            {
                moveDirectionRotationAccordanceData = moveDirectionsRotationAccordance[i];
                moveDirectionAfterRotation = moveDirectionRotationAccordanceData.MoveDirectionAfterRotation;

                if (i == 0)
                    cachedCardinalPointBeforeRotation = cardinalPoints[moveDirectionAfterRotation];

                cardinalPoints[moveDirectionAfterRotation] = (i == (moveDirectionsRotationAccordance.Count - 1)) ? cachedCardinalPointBeforeRotation :
                    cardinalPoints[moveDirectionRotationAccordanceData.MoveDirectionBeforeRotation];
            }
        }

        public void Rotate(RotationType rotationType, IDictionary<MoveDirection, CardinalPoint> cardinalPoints)
        {
            IList<MoveDirectionRotationAccordanceData> moveDirectionsRotationAccordance = null;

            switch (rotationType)
            {
                case RotationType.Clockwise:
                    moveDirectionsRotationAccordance = new List<MoveDirectionRotationAccordanceData>()
                    {
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Down, MoveDirectionAfterRotation = MoveDirection.Left },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Right, MoveDirectionAfterRotation = MoveDirection.Down },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Up, MoveDirectionAfterRotation = MoveDirection.Right },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Left, MoveDirectionAfterRotation = MoveDirection.Up }
                    };
                    break;
                case RotationType.CounterClockwise:
                    moveDirectionsRotationAccordance = new List<MoveDirectionRotationAccordanceData>()
                    {
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Up, MoveDirectionAfterRotation = MoveDirection.Left },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Right, MoveDirectionAfterRotation = MoveDirection.Up },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Down, MoveDirectionAfterRotation = MoveDirection.Right },
                        new MoveDirectionRotationAccordanceData() { MoveDirectionBeforeRotation = MoveDirection.Left, MoveDirectionAfterRotation = MoveDirection.Down }
                    };
                    break;
            }

            if (moveDirectionsRotationAccordance != null)
                Rotate(moveDirectionsRotationAccordance, cardinalPoints);
        }

        public RotationType GetInverseRotationType(RotationType rotationType)
        {
            switch (rotationType)
            {
                case RotationType.Clockwise:
                    return RotationType.CounterClockwise;
                case RotationType.CounterClockwise:
                    return RotationType.Clockwise;
                default:
                    return RotationType.None;
            }
        }

        private struct MoveDirectionRotationAccordanceData
        {
            public MoveDirection MoveDirectionBeforeRotation { get; set; }

            public MoveDirection MoveDirectionAfterRotation { get; set; }
        }
    }
}

namespace GameScene.Services.Ball.Enums
{
    public enum RotationType
    {
        Clockwise,

        CounterClockwise,

        None
    }
}