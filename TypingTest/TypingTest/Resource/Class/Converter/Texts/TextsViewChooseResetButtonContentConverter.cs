using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypingTest.Resource.Class.Converter.Texts
{
    class TextsViewChooseResetButtonContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnedValue = String.Empty;
            string stringParameter = parameter as string;
            if (stringParameter != null)
            {
                string[] stringParameters = stringParameter.Split(' ');
                if (stringParameters.Count() == 2)
                    returnedValue = values[0].TryReturnPotentiallyEqualTextsTextModelsResult(values[1], stringParameters[0], stringParameters[1]);
            }
            return returnedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


