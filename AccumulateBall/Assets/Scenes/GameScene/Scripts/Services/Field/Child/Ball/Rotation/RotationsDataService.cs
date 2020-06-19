using System;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Services.Ball.Data;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Ball.Info;

namespace GameScene.Services.Ball
{
    public class RotationsDataService : BaseSharedService
    {
        private static RotationsData GetDoubleRotationData()
        {
            RotationType[] possibleRotationTypes = new RotationType[] { RotationType.Clockwise, RotationType.CounterClockwise };

            return new RotationsData(2, possibleRotationTypes[UnityEngine.Random.Range(0, possibleRotationTypes.Length)]);
        }

        private static RotationsData GetSingleRotationData(int rotationDistance, Orientation totalOrientation)
        {
            IDictionary<Orientation, RotationType> orientationsRotationTypes;

            if ((rotationDistance == -1) || (rotationDistance == (Enum.GetValues(typeof(CardinalPoint)).Length - 1)))
            {
                orientationsRotationTypes = new Dictionary<Orientation, RotationType>()
                    {
                        { Orientation.Direct, RotationType.CounterClockwise },
                        { Orientation.Inverse, RotationType.Clockwise },
                    };
            }
            else
            {
                orientationsRotationTypes = new Dictionary<Orientation, RotationType>()
                    {
                        { Orientation.Direct, RotationType.Clockwise },
                        { Orientation.Inverse, RotationType.CounterClockwise },
                    };
            }

            return new RotationsData(1, orientationsRotationTypes[totalOrientation]);
        }

        public IEnumerable<KeyValuePair<CardinalPoint, RotationsData>> GetRotationsDataBetweenCardinalPointsIteratively(RotationsInfo rotationsInfo)
        {
            int cardinalPointsRotationDistance;
            RotationsData rotationsData;

            foreach (CardinalPoint unfilledCardinalPoint in rotationsInfo.CardinalPointsInfo.Unfilled)
            {
                cardinalPointsRotationDistance = (int)rotationsInfo.CardinalPointsInfo.Filled - (int)unfilledCardinalPoint;
                rotationsData = (Math.Abs(cardinalPointsRotationDistance) == 2) ? GetDoubleRotationData() : GetSingleRotationData(cardinalPointsRotationDistance,
                    rotationsInfo.TotalOrientation);

                yield return new KeyValuePair<CardinalPoint, RotationsData>(unfilledCardinalPoint, rotationsData);
            }
        }
    }
}

namespace GameScene.Services.Ball.Data
{
    public struct RotationsData
    {
        public RotationsData(int amount, RotationType type)
        {
            Amount = amount;
            Type = type;
        }

        public RotationType Type { get; private set; }

        public int Amount { get; private set; }
    }
}

namespace GameScene.Services.Ball.Info
{
    public struct CardinalPointsRotationsInfo
    {
        public CardinalPointsRotationsInfo(IEnumerable<CardinalPoint> unfilled, CardinalPoint filled)
        {
            Unfilled = unfilled;
            Filled = filled;
        }

        public CardinalPoint Filled { get; private set; }

        public IEnumerable<CardinalPoint> Unfilled { get; private set; }
    }

    public struct RotationsInfo
    {
        public RotationsInfo(CardinalPointsRotationsInfo cardinalPointsInfo, Orientation totalOrientation)
        {
            CardinalPointsInfo = cardinalPointsInfo;
            TotalOrientation = totalOrientation;
        }

        public Orientation TotalOrientation { get; private set; }

        public CardinalPointsRotationsInfo CardinalPointsInfo { get; private set; }
    }
}