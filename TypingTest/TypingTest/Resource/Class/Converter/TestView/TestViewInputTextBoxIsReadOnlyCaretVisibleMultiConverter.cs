using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TypingTest.Model.TimerModel;

namespace TypingTest.Resource.Class.Converter.TestView
{
    class TestViewInputTextBoxIsReadOnlyCaretVisibleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool convertedValue = true;
            try { convertedValue = ((values[1] as TestTimerModel).IsTimeDecreasingEnabled() && !(System.Convert.ToBoolean(values[0]))); }
            catch { return convertedValue; }
            return !convertedValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
