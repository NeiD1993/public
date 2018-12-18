using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.Service;
using TypingTest.Resource.Class;

namespace TypingTest.Model.TextModel.TextsTextModel
{
    class TextsTextModels
    {
        private TextsTextModel selectedTextsTextViewModel;

        public TextsTextModels(MainTextModel chosenTextsTextModel)
        {
            TextsTextModelsObservableCollection = new ObservableCollection<TextsTextModel>();
            ChosenTextsTextModel = chosenTextsTextModel;
        }

        public MainTextModel ChosenTextsTextModel { get; private set; }

        public TextsTextModel SelectedTextsTextModel
        {
            get { return selectedTextsTextViewModel; }

            set
            {
                if (TextsTextModelsObservableCollection.Contains(value))
                    selectedTextsTextViewModel = value;
            }
        }

        public ObservableCollection<TextsTextModel> TextsTextModelsObservableCollection { get; private set; }

        public void ChooseTextsTextModel()
        {
            if (SelectedTextsTextModel != null)
                ChosenTextsTextModel = SelectedTextsTextModel;
        }

        public void ResetChosenTextsTextModel() { ChosenTextsTextModel = null; }

        public bool IsTextsTextViewModelsSuccesfullyRefreshed()
        {
            if (TextsTextModelsObservableCollection.Count > 0)
                TextsTextModelsObservableCollection.Clear();
            bool isChosenTextsTextModelExist = false;
            List<FileInfo> textsDirectoryFiles = DirectoryService.GetTextsDirectoryFiles();
            if ((textsDirectoryFiles != null) && (textsDirectoryFiles.Count > 0))
            {
                TextsTextModel textsTextModel;
                foreach (FileInfo textsDirectoryFile in textsDirectoryFiles)
                {
                    textsTextModel = new TextsTextModel(textsDirectoryFile.Name, textsDirectoryFile.DirectoryName, textsDirectoryFile.Length, textsDirectoryFile.LastAccessTime);
                    if ((ChosenTextsTextModel != null) && !isChosenTextsTextModelExist && textsDirectoryFile.EqualsMainTextModel(ChosenTextsTextModel))
                    {
                        isChosenTextsTextModelExist = true;
                        ChosenTextsTextModel = textsTextModel;
                    }
                    TextsTextModelsObservableCollection.Add(textsTextModel);
                }
            }
            if (!isChosenTextsTextModelExist)
                ResetChosenTextsTextModel();
            return (TextsTextModelsObservableCollection.Count != 0);
        }
    }
}
