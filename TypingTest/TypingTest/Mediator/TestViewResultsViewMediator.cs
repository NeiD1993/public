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
    static class TestViewResultsViewMediator
    {
        public static void OpenResultsView(object testViewObject)
        {
            testViewObject.TryExecuteWindowObjectAction<TestView>(testView =>
            {
                testView.DataContext.TryExecuteWindowDataContextObjectAction<TestViewModel>(testViewModel =>
                {
                    testView.IsEnabled = false;
                    testView.Hide();
                    ResultsView resultsView = new ResultsView();
                    resultsView.DataContext = new ResultsViewModel(testViewModel.TestTextModel, testViewModel.TestTimerModel);
                    resultsView.Owner = testView;
                    resultsView.Show();
                });
            });
        }

        public static void ShowTestView(object resultsViewObject)
        {
            resultsViewObject.TryExecuteWindowObjectAction<ResultsView>(resultsView =>
            {
                resultsView.Owner.TryExecuteWindowObjectAction<TestView>(testView =>
                {
                    testView.DataContext.TryExecuteWindowDataContextObjectAction<TestViewModel>(testViewModel =>
                    {
                        testView.Owner.TryExecuteWindowObjectAction<MainView>(mainView =>
                        {
                            resultsView.IsEnabled = false;
                            resultsView.Close();
                            testView.IsHitTestVisible = false;
                            testView.Close();
                            testView = new TestView();
                            testView.DataContext = testViewModel;
                            testViewModel.TestTimerModelResetTime();
                            testViewModel.TestTextModelResetInputStatistics();
                            testView.Owner = mainView;
                            testView.Show();
                        });
                    });
                });
            });
        }
    }
}
