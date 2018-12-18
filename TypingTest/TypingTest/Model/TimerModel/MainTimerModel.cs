using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingTest.Model.TimerModel
{
    class MainTimerModel : BaseTimerModel
    {
        private readonly TimeSpan maximalTime;

        public MainTimerModel(TimeSpan minimalTime, TimeSpan initialTime, TimeSpan maximalTime) : 
               base(minimalTime, initialTime) 
        {
            this.maximalTime = ((maximalTime >= Time) && (maximalTime <= timeUpperInitializationBorder)) ? 
                               maximalTime : Time;
        }

        public void IncreaseTime()
        {
            TimeSpan increasedTime = Time + modificationTime;
            if (increasedTime <= maximalTime)
                Time = increasedTime;
        }
    }
}
