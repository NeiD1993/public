using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingTest.Model.TimerModel
{
    class TestTimerModel : BaseTimerModel
    {
        public TestTimerModel(TimeSpan initialTime) : base(TimeSpan.Zero, initialTime) { InitialTime = Time; }
        
        public TimeSpan InitialTime { get; private set; }

        public void ResetTime() { Time = InitialTime; }

        public bool IsTimeDecreasingEnabled() { return (Time == minimalTime) ? false : true; }
    }
}
