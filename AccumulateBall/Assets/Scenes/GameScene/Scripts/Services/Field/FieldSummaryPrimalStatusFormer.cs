using System;
using System.Collections.Generic;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Services.Status;

namespace GameScene.Services.Field
{
    public class FieldSummaryPrimalStatusFormer : BaseSummaryStatusFormer<FieldEntityPrimalStatusData, IEnumerable<FieldEntityPrimalStatusData>, FieldEntityPrimalStatusData>
    {
        public override FieldEntityPrimalStatusData FormSummaryStatus(IEnumerable<FieldEntityPrimalStatusData> elements)
        {
            (bool availability, bool finalization) summaryBooleans = (true, true);
            T booleanStatusConversionFunction<T>(bool booleanStatus) where T : Enum => (T)Enum.ToObject(typeof(T), booleanStatus);
            Func<Enum, bool, bool> statusesSummationFunction = (status, summaryStatus) => summaryStatus && Convert.ToBoolean(status);

            foreach (FieldEntityPrimalStatusData statusData in elements)
            {
                summaryBooleans.availability = statusesSummationFunction(statusData.Availability, summaryBooleans.availability);
                summaryBooleans.finalization = statusesSummationFunction(statusData.Finalization, summaryBooleans.finalization);
            }

            return new FieldEntityPrimalStatusData(booleanStatusConversionFunction<FieldEntityAvailabilityStatus>(summaryBooleans.availability), 
                booleanStatusConversionFunction<FieldEntityFinalizationStatus>(summaryBooleans.finalization));
        }
    }
}