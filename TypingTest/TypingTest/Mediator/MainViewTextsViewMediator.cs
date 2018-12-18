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
    static class MainViewTextsViewMediator
    {
        private static void UpdateMainView(TextsView textsView)
        {
            textsView.Owner.TryExecuteWindowObjectAction<MainView>(mainView =>
            {
                mainView.DataContext.TryExecuteWindowDataContextObjectAction<MainViewModel>(mainViewModel =>
                {
                    textsView.DataContext.TryExecuteWindowDataContextObjectAction<TextsViewModel>(textsViewModel => mainViewModel.MainTextModel = textsViewModel.TextsTextModels.ChosenTextsTextModel);
                });
            });
        }

        public static void CloseTextsView(object textsViewObject)
        {
            textsViewObject.TryExecuteWindowObjectAction<TextsView>(textsView =>
            {
                UpdateMainView(textsView);
                textsView.Close();
            });
        }

        public static void OpenTextsView(object mainViewObject)
        {
            mainViewObject.TryExecuteWindowObjectAction<MainView>(mainView =>
            {
                if (mainView.TryFindWindowInOwnerWindowOwnedWindows<TextsView>() == null)
                {
                    mainView.DataContext.TryExecuteWindowDataContextObjectAction<MainViewModel>(mainViewModel =>
                    {
                        TextsView textsView = new TextsView();
                        textsView.DataContext = new TextsViewModel(mainViewModel.MainTextModel);
                        textsView.Owner = mainView;
                        textsView.Show();
                    });
                }
            });
        }

        public static void UpdateMainView(object textsViewObject) { textsViewObject.TryExecuteWindowObjectAction<TextsView>(textsView => UpdateMainView(textsView)); }

        public static void UpdateTextsView(object mainViewObject)
        {
            mainViewObject.TryExecuteWindowObjectAction<MainView>(mainView =>
            {
                mainView.TryFindWindowInOwnerWindowOwnedWindows<TextsView>().TryExecuteWindowObjectAction<TextsView>(textsView =>
                {
                    textsView.DataContext.TryExecuteWindowDataContextObjectAction<TextsViewModel>(textsViewModel => textsViewModel.TextsTextModels.IsTextsTextViewModelsSuccesfullyRefreshed());
                });
            });
        }
    }
}
