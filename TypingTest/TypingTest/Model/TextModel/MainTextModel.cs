using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.Service;

namespace TypingTest.Model.TextModel
{
    class MainTextModel
    {
        private static readonly string wordNumberUnderscoreCharacterPattern = "(\\w| |\\.)";

        public static readonly string textDirectoryNameDefaultValue = "Directory:";

        public static readonly string textNameValuePattern = wordNumberUnderscoreCharacterPattern + "+";

        public static readonly string textDirectoryNameValuePattern = "[A-Z]:\\\\(" + textNameValuePattern + 
                                                                      "\\\\)*" + wordNumberUnderscoreCharacterPattern + "*";

        public static readonly string textNameDefaultValue = "Text";

        private string textDirectoryName;

        private string textName;

        static MainTextModel() 
        {
            string directoryServiceTextsExtension = DirectoryService.textsExtension;
            textNameValuePattern += directoryServiceTextsExtension;
            textNameDefaultValue += directoryServiceTextsExtension;
        }

        public MainTextModel(string textName, string textDirectoryName, DateTime textCreationTime) 
        {
            TextName = textName;
            TextDirectoryName = textDirectoryName;
            TextCreationTime = textCreationTime;
        }
        
        public string TextName
        {
            get { return textName; }

            set { textName = (RegularExpressionsService.HasInputStringPatternStringFormat(textNameValuePattern, value)) ? value : textNameDefaultValue; }
        }

        public string TextDirectoryName
        {
            get { return textDirectoryName; }

            set { textDirectoryName = (RegularExpressionsService.HasInputStringPatternStringFormat(textDirectoryNameValuePattern, value)) ? value : textDirectoryNameDefaultValue; }
        }

        public DateTime TextCreationTime { get; private set; }

        public override bool Equals(object obj)
        {
            MainTextModel mainTextModel = obj as MainTextModel;
            if (mainTextModel != null)
                return ((TextName == mainTextModel.TextName) && (TextDirectoryName == mainTextModel.TextDirectoryName) && (TextCreationTime == mainTextModel.TextCreationTime));
            else return false;
        }

        public override int GetHashCode()
        {
            return (TextName.GetHashCode() + TextDirectoryName.GetHashCode() + TextCreationTime.GetHashCode());
        }
    }
}
