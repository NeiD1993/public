using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TypingTest.ViewModel;

namespace TypingTest.Resource.Class.Converter.TestView
{
    class TestViewLoadingResultsTextBlockCommandParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object convertedValue = null;
            string stringValue = values[0] as string;
            {
                View.TestView testView = values[1] as View.TestView;
                if ((testView != null) && (stringValue == TestViewModel.openResultsViewCommandString))
                    convertedValue = testView;
            }
            return convertedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
