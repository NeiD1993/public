using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypingTest.Mediator;
using TypingTest.Model;
using TypingTest.Model.TextModel;
using TypingTest.Model.TimerModel;
using TypingTest.Resource.Class;

namespace TypingTest.ViewModel
{
    class ResultsViewModel : BaseViewModel
    {
        public ResultsViewModel(TestTextModel testTextModel, TestTimerModel testTimerModel)
        {
            ResultsModel = new ResultsModel(testTextModel.InputCharactersCount, testTextModel.MistypedWordsCount, testTextModel.GetOutputTestTextWordsCount(), testTimerModel.InitialTime - testTimerModel.Time);
            ShowMainViewCommand = new Command(senderObject => ResultsViewMainViewMediator.ShowMainView(senderObject));
            ShowTestViewCommand = new Command(senderObject => TestViewResultsViewMediator.ShowTestView(senderObject));
        }

        public ICommand ShowMainViewCommand { get; private set; }

        public ICommand ShowTestViewCommand { get; private set; }

        public ResultsModel ResultsModel { get; private set; }
    }
}
