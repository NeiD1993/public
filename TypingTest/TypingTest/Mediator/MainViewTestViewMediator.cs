using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.View;
using TypingTest.ViewModel;
using TypingTest.Resource.Class;
using TypingTest.Service;

namespace TypingTest.Mediator
{
    static class MainViewTestViewMediator
    {
        public static void ShowMainView(object testViewObject)
        {
            testViewObject.TryExecuteWindowObjectAction<TestView>(testView =>
            {
                if (testView.IsHitTestVisible)
                {
                    testView.Owner.TryExecuteWindowObjectAction<MainView>(mainView =>
                    {
                        mainView.IsEnabled = true;
                        mainView.Show();
                    });
                }
            });
        }

        public static void OpenTestView(object mainViewObject)
        {
            mainViewObject.TryExecuteWindowObjectAction<MainView>(mainView =>
            {
                mainView.DataContext.TryExecuteWindowDataContextObjectAction<MainViewModel>(mainViewModel =>
                {
                    string textsDirectoryFileContent = DirectoryService.ReturnTextsDirectoryFileContent(mainViewModel.MainTextModel);
                    if (textsDirectoryFileContent != String.Empty)
                    {
                        mainView.TryFindWindowInOwnerWindowOwnedWindows<TextsView>().TryExecuteWindowObjectAction<TextsView>(textsView => textsView.Close());
                        mainView.IsEnabled = false;
                        mainView.Hide();
                        TestView testView = new TestView();
                        testView.DataContext = new TestViewModel(mainViewModel.MainTimerModel, textsDirectoryFileContent);
                        testView.Owner = mainView;
                        testView.Show();
                    }
                    else mainViewModel.MainTextModel = null;
                });
            });
        }
    }
}
