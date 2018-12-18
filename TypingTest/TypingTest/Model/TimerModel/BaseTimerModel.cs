using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingTest.Model.TimerModel
{
    abstract class BaseTimerModel
    {
        private static readonly TimeSpan timeLowerInitializationBorder = TimeSpan.FromMinutes(1);

        protected static readonly TimeSpan modificationTime = TimeSpan.FromSeconds(1);

        public static readonly TimeSpan minimalTimeLowerInitializationBorder = TimeSpan.Zero;

        public static readonly TimeSpan timeUpperInitializationBorder = TimeSpan.FromSeconds(599);

        protected readonly TimeSpan minimalTime;

        public BaseTimerModel(TimeSpan minimalTime, TimeSpan initialTime)
        {
            this.minimalTime = ((minimalTime >= minimalTimeLowerInitializationBorder) && (minimalTime <= timeLowerInitializationBorder)) ? minimalTime : minimalTimeLowerInitializationBorder;
            Time = ((initialTime >= timeLowerInitializationBorder) && (initialTime <= timeUpperInitializationBorder)) ? initialTime : this.minimalTime;
        }
        
        public TimeSpan Time { get; protected set; }

        public void DecreaseTime()
        {
            TimeSpan decreasedTime = Time - modificationTime;
            if (decreasedTime >= minimalTime)
                Time = decreasedTime;
        }
    }
}
