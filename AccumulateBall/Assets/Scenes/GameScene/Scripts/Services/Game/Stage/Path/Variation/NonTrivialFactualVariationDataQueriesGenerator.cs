using System;
using System.Collections.Generic;
using MoreLinq;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private abstract partial class BaseVariativePathPointsDataPickingService<T1, T2>
            {
                private class NonTrivialFactualVariationDataQueriesGenerator
                {
                    private NonTrivialFactualVariationDataQueriesKind? generatedNonTrivialQueriesKind;

                    public NonTrivialFactualVariationDataQueriesGenerator()
                    {
                        generatedNonTrivialQueriesKind = null;
                    }

                    private NonTrivialFactualVariationDataQueriesKind GeneratedNonTrivialQueriesKind
                    {
                        get
                        {
                            generatedNonTrivialQueriesKind = (generatedNonTrivialQueriesKind == NonTrivialFactualVariationDataQueriesKind.Unindulgent) ? 
                                NonTrivialFactualVariationDataQueriesKind.Indulgent : (NonTrivialFactualVariationDataQueriesKind)UnityEngine.Random.Range(0, Enum.GetValues(typeof(NonTrivialFactualVariationDataQueriesKind)).Length);

                            return generatedNonTrivialQueriesKind.Value;
                        }
                    }

                    public IEnumerable<Func<KeyValuePair<T1, T2>>> GenerateNonTrivialFactualVariationDataQueriesIteratively(IDictionary<T1, T2> potentialVariationData, Func<KeyValuePair<T1, T2>, float> filteringValueExtractor)
                    {
                        Func<KeyValuePair<T1, T2>> GetNonTrivialFactualVariationDataQuery(NonTrivialFactualVariationDataQueryType nonTrivialQueryType)
                        {
                            switch (nonTrivialQueryType)
                            {
                                case NonTrivialFactualVariationDataQueryType.MaxBy:
                                    return () => potentialVariationData.MaxBy(filteringValueExtractor).Last();
                                default:
                                    return () => potentialVariationData.MinBy(filteringValueExtractor).First();
                            }
                        }

                        Array nonTrivialQueriesTypes = Enum.GetValues(typeof(NonTrivialFactualVariationDataQueryType));
                        
                        switch (GeneratedNonTrivialQueriesKind)
                        {
                            case NonTrivialFactualVariationDataQueriesKind.Indulgent:
                                {
                                    foreach (NonTrivialFactualVariationDataQueryType nonTrivialQueryType in nonTrivialQueriesTypes)
                                        yield return GetNonTrivialFactualVariationDataQuery(nonTrivialQueryType);
                                }
                                break;
                            default:
                                yield return GetNonTrivialFactualVariationDataQuery((NonTrivialFactualVariationDataQueryType)UnityEngine.Random.Range(0, nonTrivialQueriesTypes.Length));
                                break;
                        }
                    }

                    private enum NonTrivialFactualVariationDataQueriesKind
                    {
                        Indulgent = 0,

                        Unindulgent = 1
                    }

                    private enum NonTrivialFactualVariationDataQueryType
                    {
                        MaxBy = 0,

                        MinBy = 1
                    }
                }
            }
        }
    }
}