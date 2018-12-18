using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TypingTest.ViewModel
{
    abstract class ViewModelWithDispatcherTimer : BaseViewModel
    {
        private static readonly TimeSpan operationsExecutorIntervalHigherInitializationBorder = TimeSpan.FromMilliseconds(1200);

        private static readonly TimeSpan operationsExecutorIntervalLowerInitializationBorder = TimeSpan.FromMilliseconds(200);

        protected readonly DispatcherTimer operationsExecutor = new DispatcherTimer();

        protected readonly List<EventHandler> operationsExecutorEventHandlers = new List<EventHandler>();

        public ViewModelWithDispatcherTimer(TimeSpan operationsExecutorInterval)
        {
            if ((operationsExecutorInterval >= operationsExecutorIntervalLowerInitializationBorder) && 
                (operationsExecutorInterval <= operationsExecutorIntervalHigherInitializationBorder))
                operationsExecutor.Interval = operationsExecutorInterval;
            else
            {
                if (operationsExecutorInterval < operationsExecutorIntervalLowerInitializationBorder)
                    operationsExecutor.Interval = operationsExecutorIntervalLowerInitializationBorder;
                else
                    operationsExecutor.Interval = operationsExecutorIntervalHigherInitializationBorder;
            }
        }
        
        protected void StartOperationsExecutor()
        {
            foreach (EventHandler operationsExecutorEventHandler in operationsExecutorEventHandlers)
                operationsExecutor.Tick += operationsExecutorEventHandler;
            operationsExecutor.Start();
        }

        protected void StopOperationsExecutor()
        {
            operationsExecutor.Stop();
            foreach (EventHandler operationsExecutorEventHandler in operationsExecutorEventHandlers)
                operationsExecutor.Tick -= operationsExecutorEventHandler;
        }
    }
}
