using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypingTest.Resource.Class.Converter
{
    class ResultsViewMistypedWordsCountWordsCountConverter : IMultiValueConverter
    {
        private static readonly string percentCharacterString = "%";

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string convertedValue = "0" + percentCharacterString;
            try
            {
                int resultsModelMistypedWordsCount = System.Convert.ToInt32(values[0]), resultsModelWordsCount = System.Convert.ToInt32(values[1]);
                if (resultsModelMistypedWordsCount >= 0 && (resultsModelWordsCount >= 1) && (resultsModelMistypedWordsCount <= resultsModelWordsCount))
                    convertedValue = (((double)resultsModelMistypedWordsCount / resultsModelWordsCount) * 100).ToString() + percentCharacterString;
            }
            catch { return convertedValue; }
            return convertedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
