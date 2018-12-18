using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TypingTest.Model.TextModel.TextsTextModel;

namespace TypingTest.Resource.Class.Converter.Texts
{
    class TextsTextsModelNumberConverter : IMultiValueConverter
    {
        private static string GetTextsTextsModelElementStringNumber(int textsTextViewModelNumber)
        {
            string textsTextViewModelStringNumber;
            switch (textsTextViewModelNumber)
            {
                case 0:
                    textsTextViewModelStringNumber = "1st";
                    break;
                case 1:
                    textsTextViewModelStringNumber = "2nd";
                    break;
                case 2:
                    textsTextViewModelStringNumber = "3rd";
                    break;
                default:
                    textsTextViewModelStringNumber = (textsTextViewModelNumber + 1).ToString() + "th";
                    break;
            }
            return textsTextViewModelStringNumber;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string convertedValue = String.Empty;
            TextsTextModel textsTextModel = values[0] as TextsTextModel;
            ObservableCollection<TextsTextModel> textsTextModels = values[1] as ObservableCollection<TextsTextModel>;
            if ((textsTextModel != null) && (textsTextModels != null))
            {
                int textsTextModelIndex = textsTextModels.IndexOf(textsTextModel);
                if (textsTextModelIndex > -1)
                    convertedValue = GetTextsTextsModelElementStringNumber(textsTextModelIndex);
            }
            return convertedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
