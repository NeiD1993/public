using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TypingTest.Mediator;
using TypingTest.Model.TextModel;
using TypingTest.Model.TextModel.TextsTextModel;
using TypingTest.Resource.Class;

namespace TypingTest.ViewModel
{
    class TextsViewModel : BaseViewModel
    {
        public static readonly string textsTextModelsPropertyName = "TextsTextModels";

        public TextsViewModel(MainTextModel chosenMainTextModel)
        {
            TextsTextModels = new TextsTextModels(chosenMainTextModel);
            LoadRefreshTextsTextModelsCommand = new Command(senderObject =>
            {
                if (TextsTextModels.IsTextsTextViewModelsSuccesfullyRefreshed())
                    MainViewTextsViewMediator.UpdateMainView(senderObject);
                else
                    MainViewTextsViewMediator.CloseTextsView(senderObject);
            });
            ChooseResetTextsTextModelCommand = new Command(senderObject =>
            {
                TextsTextModel textsTextModelsSelectedTextsTextModel = TextsTextModels.SelectedTextsTextModel;
                if ((textsTextModelsSelectedTextsTextModel != null) && 
                    !textsTextModelsSelectedTextsTextModel.Equals(TextsTextModels.ChosenTextsTextModel))
                    TextsTextModels.ChooseTextsTextModel();
                else
                    TextsTextModels.ResetChosenTextsTextModel();
                OnPropertyChanged(textsTextModelsPropertyName);
                MainViewTextsViewMediator.UpdateMainView(senderObject);
            });
        }

        public ICommand ChooseResetTextsTextModelCommand { get; private set; }

        public ICommand LoadRefreshTextsTextModelsCommand { get; private set; }
        
        public TextsTextModels TextsTextModels { get; private set; }
    }
}
