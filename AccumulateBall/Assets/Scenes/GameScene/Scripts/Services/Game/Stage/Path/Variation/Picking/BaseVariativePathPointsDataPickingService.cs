using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Services.Game.Descriptions;
using GameScene.Services.Platform;
using MoreLinq;
using ServicesLocators;
using UnityEngine;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private abstract partial class BaseVariativePathPointsDataPickingService<T1, T2>
            {
                private readonly NonTrivialFactualVariationDataQueriesGenerator nonTrivialFactualVariationDataQueriesGenerator;

                private readonly PositionService positionService;

                public BaseVariativePathPointsDataPickingService()
                {
                    nonTrivialFactualVariationDataQueriesGenerator = new NonTrivialFactualVariationDataQueriesGenerator();
                    positionService = SharedSceneServicesLocator.GetService<PositionService>();
                    VariativePathPointPositionExtractor = CreateVariativePathPointPositionExtractor();
                }

                private Func<KeyValuePair<T1, T2>, Vector2Int> VariativePathPointPositionExtractor { get; set; }

                private IEnumerator FilterVariationDataIteratively(VariativePathPointsDataPickingInfo pickingInfo, Action<PickedVariativePathPointsDataDescription<T1, T2>?> pickedDataExtractor)
                {
                    IEnumerable<Func<KeyValuePair<T1, T2>>> GetFactualVariationDataQueriesIteratively(Vector2Int factualityValidatingPosition)
                    {
                        IEnumerable<Func<KeyValuePair<T1, T2>>> GetTrivialFactualVariationDataQueriesIteratively()
                        {
                            yield return () => pickingInfo.PotentialVariationData.Single();
                        }

                        IDictionary<T1, T2> potentialVariationData = pickingInfo.PotentialVariationData;

                        return (potentialVariationData.Count == 1) ? GetTrivialFactualVariationDataQueriesIteratively() : nonTrivialFactualVariationDataQueriesGenerator.GenerateNonTrivialFactualVariationDataQueriesIteratively(potentialVariationData, (potentialVariationDataItemParameter) => positionService.GetDistanceBetweenPositions(VariativePathPointPositionExtractor(potentialVariationDataItemParameter), factualityValidatingPosition));
                    }

                    Vector2Int optimalReachableNonVariativePathPoint;
                    KeyValuePair<T1, T2> factualVariationDataItem;
                    IDictionary<T1, T2> factualVariationData = new Dictionary<T1, T2>();

                    yield return optimalReachableNonVariativePathPoint = pickingInfo.ReachableNonVariativePathPointsData.FindOptimalPoint(reachableNonVariativePathPointParameter => positionService.GetDistanceBetweenPositions(pickingInfo.CurrentVariativePoint, reachableNonVariativePathPointParameter));

                    foreach (Func<KeyValuePair<T1, T2>> factualVariationDataQuery in GetFactualVariationDataQueriesIteratively(optimalReachableNonVariativePathPoint))
                    {
                        yield return factualVariationDataItem = factualVariationDataQuery();

                        factualVariationData.Add(factualVariationDataItem);
                    }

                    pickedDataExtractor(new PickedVariativePathPointsDataDescription<T1, T2>(factualVariationData, true));
                }

                private IEnumerator PickVariationDataIteratively(VariativePathPointsDataPickingInfo pickingInfo, Action<PickedVariativePathPointsDataDescription<T1, T2>?> pickedDataExtractor)
                {
                    IDictionary<T1, T2> potentialVariationData = pickingInfo.PotentialVariationData;
                    bool isNonVariativePathPointReached = pickingInfo.ReachableNonVariativePathPointsData.TryExclude(pickingInfo.CurrentVariativePoint);

                    if (potentialVariationData.Count > 0)
                    {
                        if (isNonVariativePathPointReached)
                            pickedDataExtractor(new PickedVariativePathPointsDataDescription<T1, T2>(potentialVariationData, false));
                        else
                            yield return FilterVariationDataIteratively(pickingInfo, pickedDataExtractor);
                    }
                    else
                        pickedDataExtractor(null);
                }

                protected abstract Func<KeyValuePair<T1, T2>, Vector2Int> CreateVariativePathPointPositionExtractor();

                public Func<Vector2Int, IDictionary<T1, T2>, Action<PickedVariativePathPointsDataDescription<T1, T2>?>, IEnumerator> CreateVariationDataPickingRoutineExtractor(ISet<Vector2Int> reachableNonVariativePathPoints)
                {
                    VariativePathPointsLinksData variativePathPointsLinksData = new VariativePathPointsLinksData();

                    return (currentVariativePathPointParameter, potentialVariationDataParameter, pickedDataExtractor) =>
                    {
                        IDictionary<T1, T2> ExcludeExcessPotentialVariationData()
                        {
                            ISet<Vector2Int> excessPotentialVariativePathPoints = variativePathPointsLinksData[variativePathPointsLinksData.CachedVariativePoint];

                            return potentialVariationDataParameter.Where((potentialVariationDataItemParameter) => !excessPotentialVariativePathPoints.Contains(VariativePathPointPositionExtractor(potentialVariationDataItemParameter))).ToDictionary();
                        }

                        variativePathPointsLinksData.CachedVariativePoint = currentVariativePathPointParameter;

                        return PickVariationDataIteratively(new VariativePathPointsDataPickingInfo(currentVariativePathPointParameter, ExcludeExcessPotentialVariationData(), reachableNonVariativePathPoints), pickedDataExtractor);
                    };
                }

                private struct VariativePathPointsDataPickingInfo
                {
                    public VariativePathPointsDataPickingInfo(Vector2Int currentVariativePoint, IDictionary<T1, T2> potentialVariationData, ISet<Vector2Int> reachableNonVariativePoints) : this()
                    {
                        CurrentVariativePoint = currentVariativePoint;
                        ReachableNonVariativePathPointsData = new ReachableNonVariativePathPointsData(reachableNonVariativePoints);
                        PotentialVariationData = potentialVariationData;
                    }

                    public Vector2Int CurrentVariativePoint { get; private set; }

                    public ReachableNonVariativePathPointsData ReachableNonVariativePathPointsData { get; private set; }

                    public IDictionary<T1, T2> PotentialVariationData { get; private set; }
                }

                private class ReachableNonVariativePathPointsData
                {
                    public ReachableNonVariativePathPointsData(ISet<Vector2Int> points)
                    {
                        Points = points;
                    }

                    private ISet<Vector2Int> Points { get; set; }

                    public bool TryExclude(Vector2Int point)
                    {
                        bool isContained = Points.Contains(point);

                        if (isContained)
                            Points.Remove(point);

                        return isContained;
                    }

                    public Vector2Int FindOptimalPoint(Func<Vector2Int, float> valueToCompareExtractor)
                    {
                        return Points.MinBy(valueToCompareExtractor).First();
                    }
                }

                private class VariativePathPointsLinksData
                {
                    private Vector2Int? cachedVariativePoint;

                    private IDictionary<Vector2Int, ISet<Vector2Int>> variativePointsLinks;

                    public VariativePathPointsLinksData()
                    {
                        cachedVariativePoint = null;
                        variativePointsLinks = null;
                    }

                    private IDictionary<Vector2Int, ISet<Vector2Int>> VariativePointsLinks
                    {
                        get
                        {
                            if (variativePointsLinks == null)
                                variativePointsLinks = new Dictionary<Vector2Int, ISet<Vector2Int>>();

                            return variativePointsLinks;
                        }
                    }

                    public Vector2Int CachedVariativePoint
                    {
                        get
                        {
                            return cachedVariativePoint.Value;
                        }

                        set
                        {
                            if (cachedVariativePoint != null)
                                this[cachedVariativePoint.Value].Add(value);

                            cachedVariativePoint = value;
                        }
                    }

                    public ISet<Vector2Int> this[Vector2Int variativePoint]
                    {
                        get
                        {
                            if (!VariativePointsLinks.ContainsKey(variativePoint))
                                VariativePointsLinks.Add(variativePoint, new HashSet<Vector2Int>());

                            return VariativePointsLinks[variativePoint];
                        }
                    }
                }
            }
        }
    }
}

namespace GameScene.Services.Game.Descriptions
{
    public struct PickedVariativePathPointsDataDescription<T1, T2>
    {
        public PickedVariativePathPointsDataDescription(IDictionary<T1, T2> value, bool isFiltered)
        {
            IsFiltered = isFiltered;
            Value = value;
        }

        public bool IsFiltered { get; private set; }

        public IDictionary<T1, T2> Value { get; private set; }
    }
}