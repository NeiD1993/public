using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TypingTest.Mediator;
using TypingTest.Model.TextModel;
using TypingTest.Model.TimerModel;
using TypingTest.Resource.Class;

namespace TypingTest.ViewModel
{
    class TestViewModel : ViewModelWithDispatcherTimer
    {
        public static readonly string openResultsViewCommandString = "Results loaded";
         
        public static readonly string testTextModelPropertyName = "TestTextModel";

        public static readonly string testTimerModelPropertyName = "TestTimerModel";

        private readonly Action<object> testTextModelCommandAction;

        private readonly EventHandler testTimerModelTimeDecreasingOperationEventHandler;
        
        public TestViewModel(MainTimerModel mainTimerModel, string testTextModelsOutputTextModel) : base(TimeSpan.FromSeconds(1))
        {
            TestTimerModel = (mainTimerModel != null) ? new TestTimerModel(mainTimerModel.Time) : new TestTimerModel(TestTimerModel.minimalTimeLowerInitializationBorder);
            TestTextModel = new TestTextModel(testTextModelsOutputTextModel);
            testTextModelCommandAction = (nullObject => OnPropertyChanged(testTextModelPropertyName));
            testTimerModelTimeDecreasingOperationEventHandler = new EventHandler((senderObject, eventArgsObject) =>
            {
                TestTimerModel.DecreaseTime();
                OnPropertyChanged(testTimerModelPropertyName);
            });
            TestTextModelAddNonWhitespaceCharacterCommand = new Command(nullObject => { }, parameterObject =>
            {
                TestTextModel.AddNonWhitespaceCharacter((char)parameterObject);
                testTextModelCommandAction.Invoke(null);
                return !TestTextModel.IsTextInputFinished;
            });
            TestTextModelAddWhitespaceCharacterCommand = new Command(testTextModelCommandAction, nullObject => { return TestTextModel.TryAddWhitespaceCharacter(); });
            TestTextModelRemoveCharacterCommand = new Command(testTextModelCommandAction, nullObject => { return TestTextModel.TryRemoveCharacter(); });
            OpenResultsViewCommand = new Command(senderObject =>
            {
                if (senderObject != null)
                    TestViewResultsViewMediator.OpenResultsView(senderObject);
            });
            ShowMainViewCommand = new Command(senderObject => MainViewTestViewMediator.ShowMainView(senderObject));
            StartStopTestTimerModelCommand = new Command(parameterObject =>
            {
                bool boolParameter = (bool)parameterObject;
                if (!boolParameter)
                {
                    operationsExecutorEventHandlers.Add(testTimerModelTimeDecreasingOperationEventHandler);
                    StartOperationsExecutor();
                }
                else if (boolParameter)
                {
                    StopOperationsExecutor();
                    operationsExecutorEventHandlers.Remove(testTimerModelTimeDecreasingOperationEventHandler);
                }
            });
        }
        
        public ICommand TestTextModelAddNonWhitespaceCharacterCommand { get; private set; }

        public ICommand TestTextModelAddWhitespaceCharacterCommand { get; private set; }

        public ICommand TestTextModelRemoveCharacterCommand { get; private set; }

        public ICommand OpenResultsViewCommand { get; private set; }

        public ICommand ShowMainViewCommand { get; private set; }

        public ICommand StartStopTestTimerModelCommand { get; private set; }

        public TestTextModel TestTextModel { get; private set; }

        public TestTimerModel TestTimerModel { get; private set; }

        public void TestTimerModelResetTime() { TestTimerModel.ResetTime(); }

        public void TestTextModelResetInputStatistics() { TestTextModel.ResetTestTextModelInputStatistics(); }
    }
}
