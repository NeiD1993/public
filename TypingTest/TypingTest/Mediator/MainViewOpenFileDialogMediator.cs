using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.View;
using TypingTest.Resource.Class;
using TypingTest.Service;

namespace TypingTest.Mediator
{
    static class MainViewOpenFileDialogMediator
    {
        public static void ShowOpenFileDialog(object mainViewObject)
        {
            mainViewObject.TryExecuteWindowObjectAction<MainView>(mainView =>
            {
                if (DirectoryService.AreFilesSuccessfullyDownloadedToTextsDirectory(OpenFileDialogService.ShowOpenFileDialog()))
                    MainViewTextsViewMediator.UpdateTextsView(mainView);
            });
        }
    }
}
