using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypingTest.Resource.Class.Converter
{
    class WPMConverter : IMultiValueConverter
    {
        private static readonly int periodDuration = 60;

        private static readonly int standardizedWordCharactersCount = 5;

        private string GetStringWPM(int inputCharactersCount, int elapsedTimeSeconds)
        {
            return ((int)(((double)inputCharactersCount / (standardizedWordCharactersCount * elapsedTimeSeconds)) * periodDuration)).ToString();
        }
                
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string convertedValue = "0";
            try 
            {
                int inputCharactersCount = System.Convert.ToInt32(values.Last());
                if (inputCharactersCount >= 0)
                {
                    int valuesLength = values.Length;
                    if (valuesLength == 2)
                    {
                        int elapsedTimeSeconds = System.Convert.ToInt32(values[0]);
                        if (elapsedTimeSeconds > 0)
                            convertedValue = GetStringWPM(inputCharactersCount, elapsedTimeSeconds);
                    }
                    else if (valuesLength == 3)
                    {
                        int initialTimeSeconds = System.Convert.ToInt32(values[0]), timeSeconds = System.Convert.ToInt32(values[1]);
                        if ((initialTimeSeconds >= 0) && (timeSeconds >= 0) && (initialTimeSeconds > timeSeconds))
                            convertedValue = GetStringWPM(inputCharactersCount, initialTimeSeconds - timeSeconds);
                    }
                }
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
