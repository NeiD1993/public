using System.Collections.Generic;

namespace GameScene.Services.Status
{
    public abstract class BaseSummaryStatusFormer<T1, T2, T3> where T2 : IEnumerable<T1> where T3 : struct
    {
        public abstract T3 FormSummaryStatus(T2 elements);
    }
}