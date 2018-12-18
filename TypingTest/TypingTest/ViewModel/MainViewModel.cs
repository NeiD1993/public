using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypingTest.Mediator;
using TypingTest.Model.TextModel;
using TypingTest.Model.TimerModel;
using TypingTest.Resource.Class;

namespace TypingTest.ViewModel
{
    class MainViewModel : ViewModelWithDispatcherTimer
    {
        public static readonly string mainTextModelPropertyName = "MainTextModel";

        public static readonly string mainTimerModelPropertyName = "MainTimerModel";

        private static readonly TimeSpan mainTimerModelTimeModificationOperationExecutorIntervalDecrementLowerInitializationBorder = TimeSpan.FromMilliseconds(1);

        private static readonly TimeSpan mainTimerModelTimeModificationOperationExecutorMinimalIntervalLowerInitializationBorder = 
                                         mainTimerModelTimeModificationOperationExecutorIntervalDecrementLowerInitializationBorder;

        private readonly EventHandler mainTimerModelTimeDecreasingOperationEventHandler;

        private readonly EventHandler mainTimerModelTimeIncreasingOperationEventHandler;

        private readonly TimeSpan mainTimerModelTimeModificationOperationExecutorIntervalDecrement;

        private readonly TimeSpan mainTimerModelTimeModificationOperationExecutorInitialInterval;

        private readonly TimeSpan mainTimerModelTimeModificationOperationExecutorMinimalInterval;

        private MainTextModel mainTextModel;

        private MainTimerModel mainTimerModel;

        public MainViewModel(MainTimerModel mainTimerModel, TimeSpan mainTimerModelTimeModificationOperationExecutorMinimalInterval,
                             TimeSpan mainTimerModelTimeModificationOperationExecutorInitialInterval, TimeSpan mainTimerModelTimeModificationOperationExecutorIntervalDecrement) :
               base(mainTimerModelTimeModificationOperationExecutorInitialInterval)
        {
            MainTimerModel = mainTimerModel;
            this.mainTimerModelTimeModificationOperationExecutorInitialInterval = operationsExecutor.Interval;
            this.mainTimerModelTimeModificationOperationExecutorMinimalInterval = (mainTimerModelTimeModificationOperationExecutorMinimalInterval >=
                                                                                   mainTimerModelTimeModificationOperationExecutorMinimalIntervalLowerInitializationBorder &&
                                                                                   mainTimerModelTimeModificationOperationExecutorMinimalInterval < operationsExecutor.Interval) ?
                                                                                  mainTimerModelTimeModificationOperationExecutorMinimalInterval :
                                                                                  mainTimerModelTimeModificationOperationExecutorMinimalIntervalLowerInitializationBorder;
            this.mainTimerModelTimeModificationOperationExecutorIntervalDecrement = ((mainTimerModelTimeModificationOperationExecutorIntervalDecrement >=
                                                                                      mainTimerModelTimeModificationOperationExecutorIntervalDecrementLowerInitializationBorder) &&
                                                                                     (this.mainTimerModelTimeModificationOperationExecutorInitialInterval -
                                                                                      mainTimerModelTimeModificationOperationExecutorIntervalDecrement >=
                                                                                      this.mainTimerModelTimeModificationOperationExecutorMinimalInterval)) ?
                                                                                    mainTimerModelTimeModificationOperationExecutorIntervalDecrement :
                                                                                    mainTimerModelTimeModificationOperationExecutorIntervalDecrementLowerInitializationBorder;
            mainTimerModelTimeDecreasingOperationEventHandler = new EventHandler((senderObject, eventArgsObject) => ResumeChangeMainTimerModelTime(MainTimerModel.DecreaseTime));
            mainTimerModelTimeIncreasingOperationEventHandler = new EventHandler((senderObject, eventArgsObject) => ResumeChangeMainTimerModelTime(MainTimerModel.IncreaseTime));
            DecreaseMainTimerModelTimeCommand = new Command(nullObject => ModifyMainTimerModelTime(MainTimerModel.DecreaseTime));
            IncreaseMainTimerModelTimeCommand = new Command(nullObject => ModifyMainTimerModelTime(MainTimerModel.IncreaseTime));
            StartDecreaseMainTimerModelTimeCommand = new Command(nullObject => StartMainTimerModelTimeModificationOperationExecutor(mainTimerModelTimeDecreasingOperationEventHandler));
            StartIncreaseMainTimerModelTimeCommand = new Command(nullObject => StartMainTimerModelTimeModificationOperationExecutor(mainTimerModelTimeIncreasingOperationEventHandler));
            StopDecreaseMainTimerModelTimeCommand = new Command(nullObject => StopMainTimerModelTimeModificationOperationExecutor(mainTimerModelTimeDecreasingOperationEventHandler));
            StopIncreaseMainTimerModelTimeCommand = new Command(nullObject => StopMainTimerModelTimeModificationOperationExecutor(mainTimerModelTimeIncreasingOperationEventHandler));
            OpenTextsViewCommand = new Command(senderObject => MainViewTextsViewMediator.OpenTextsView(senderObject));
            OpenTestViewCommand = new Command(senderObject => MainViewTestViewMediator.OpenTestView(senderObject));
            ShowOpenFileDialogCommand = new Command(senderObject => MainViewOpenFileDialogMediator.ShowOpenFileDialog(senderObject));
        }

        public ICommand DecreaseMainTimerModelTimeCommand { get; private set; }

        public ICommand IncreaseMainTimerModelTimeCommand { get; private set; }

        public ICommand OpenTestViewCommand { get; private set; }

        public ICommand OpenTextsViewCommand { get; private set; }

        public ICommand ShowOpenFileDialogCommand { get; private set; }

        public ICommand StartDecreaseMainTimerModelTimeCommand { get; private set; }

        public ICommand StartIncreaseMainTimerModelTimeCommand { get; private set; }

        public ICommand StopDecreaseMainTimerModelTimeCommand { get; private set; }

        public ICommand StopIncreaseMainTimerModelTimeCommand { get; private set; }

        public MainTextModel MainTextModel
        {
            get { return mainTextModel; }

            set
            {
                mainTextModel = value;
                OnPropertyChanged(mainTextModelPropertyName);
            }
        }

        public MainTimerModel MainTimerModel
        {
            get { return mainTimerModel; }

            set
            {
                if (value != null)
                    mainTimerModel = value;
                else
                {
                    TimeSpan mainTimerModelMinimalTimeLowerInitializationBorder = MainTimerModel.minimalTimeLowerInitializationBorder;
                }
            }
        }

        private void DecreaseMainTimerModelTimeModificationOperationExecutorInterval()
        {
            TimeSpan TimeModificationTimeExecutorInterval = operationsExecutor.Interval - mainTimerModelTimeModificationOperationExecutorIntervalDecrement;
            if (TimeModificationTimeExecutorInterval >= mainTimerModelTimeModificationOperationExecutorMinimalInterval)
                operationsExecutor.Interval = TimeModificationTimeExecutorInterval;
        }

        private void ModifyMainTimerModelTime(Action mainTimerModelTimeModificationAction)
        {
            mainTimerModelTimeModificationAction();
            OnPropertyChanged(mainTimerModelPropertyName);
        }

        private void ResumeChangeMainTimerModelTime(Action mainTimerModelTimeModificationAction)
        {
            ModifyMainTimerModelTime(mainTimerModelTimeModificationAction);
            DecreaseMainTimerModelTimeModificationOperationExecutorInterval();
        }

        private void StartMainTimerModelTimeModificationOperationExecutor(EventHandler mainTimerModelTimeModificationOperationEventHandler)
        {
            operationsExecutorEventHandlers.Add(mainTimerModelTimeModificationOperationEventHandler);
            StartOperationsExecutor();
        }

        private void StopMainTimerModelTimeModificationOperationExecutor(EventHandler mainTimerModelTimeModificationOperationEventHandler)
        {
            StopOperationsExecutor();
            operationsExecutorEventHandlers.Remove(mainTimerModelTimeModificationOperationEventHandler);
            if (operationsExecutor.Interval < mainTimerModelTimeModificationOperationExecutorInitialInterval)
                operationsExecutor.Interval = mainTimerModelTimeModificationOperationExecutorInitialInterval;
        }
    }
}
