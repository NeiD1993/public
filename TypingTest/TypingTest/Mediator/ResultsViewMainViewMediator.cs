using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.View;
using TypingTest.ViewModel;
using TypingTest.Resource.Class;

namespace TypingTest.Mediator
{
    static class ResultsViewMainViewMediator
    {
        public static void ShowMainView(object resultsViewObject)
        {
            resultsViewObject.TryExecuteWindowObjectAction<ResultsView>(resultsView =>
            {
                if (resultsView.IsEnabled)
                {
                    resultsView.Owner.TryExecuteWindowObjectAction<TestView>(testView =>
                    {
                        testView.Owner.TryExecuteWindowObjectAction<MainView>(mainView =>
                        {
                            mainView.DataContext.TryExecuteWindowDataContextObjectAction<MainViewModel>(mainViewModel =>
                            {
                                mainViewModel.MainTextModel = null;
                                testView.Close();
                            });
                        });
                    });
                }
            });
        }
    }
}
