using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Services.Ball.Data;
using GameScene.Services.Field.Data;
using GameScene.Services.Field.Info;
using UnityEngine;

namespace GameScene.Services.Field
{
    public class FieldEntityBindingService : BaseSharedService
    {
        private static CardinalPoint GetCardinalPointBySubstanceColorType(SubstanceColorType substanceColorType)
        {
            switch (substanceColorType)
            {
                case SubstanceColorType.Blue:
                    return CardinalPoint.North;
                case SubstanceColorType.Green:
                    return CardinalPoint.East;
                case SubstanceColorType.Orange:
                    return CardinalPoint.West;
                default:
                    return CardinalPoint.South;
            }
        }

        public SubstanceColorType GetSubstanceColorTypeByCardinalPoint(CardinalPoint cardinalPoint)
        {
            switch (cardinalPoint)
            {
                case CardinalPoint.East:
                    return SubstanceColorType.Green;
                case CardinalPoint.North:
                    return SubstanceColorType.Blue;
                case CardinalPoint.South:
                    return SubstanceColorType.Red;
                default:
                    return SubstanceColorType.Orange;
            }
        }

        public RotationsData? GetBallRotationsDataByReservoirInfo(IDictionary<BallRotationsDataKey, RotationsData> ballRotationsDataDictionary, 
            ReservoirInfoKey reservoirInfoKey)
        {
            RotationsData? ballRotationsData = null;
            BallRotationsDataKey ballRotationsDataKey = new BallRotationsDataKey(reservoirInfoKey.Position, GetCardinalPointBySubstanceColorType(reservoirInfoKey.SubstanceColorType));

            if (ballRotationsDataDictionary.ContainsKey(ballRotationsDataKey))
                ballRotationsData = ballRotationsDataDictionary[ballRotationsDataKey];

            return ballRotationsData;
        }
    }
}

namespace GameScene.Services.Field.Data
{
    public struct BallRotationsDataKey
    {
        public BallRotationsDataKey(Vector2Int position, CardinalPoint cardinalPoint)
        {
            Position = position;
            CardinalPoint = cardinalPoint;
        }

        public CardinalPoint CardinalPoint { get; private set; }

        public Vector2Int Position { get; private set; }
    }
}

namespace GameScene.Services.Field.Info
{
    public struct ReservoirInfoKey
    {
        public ReservoirInfoKey(Vector2Int position, SubstanceColorType substanceColorType)
        {
            Position = position;
            SubstanceColorType = substanceColorType;
        }

        public SubstanceColorType SubstanceColorType { get; private set; }

        public Vector2Int Position { get; private set; }
    }
}