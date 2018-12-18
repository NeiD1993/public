using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TypingTest.Resource.Class.Converter.Texts
{
    class TextsTextModelTextLengthConverter : IValueConverter
    {
        private static readonly int textsTextModelTextLengthUnitsDivisor = 1024;

        private static string ConvertTextsTextModelTextLengthToUnit(int textsTextModelTextLength)
        {
            int cycleIndex = 0;
            double doubleTextsTextModelLength = textsTextModelTextLength;
            string convertedTextsTextModelTextLength = String.Empty;
            string[] textsTextModelTextLengthUnitsNames = Enum.GetNames(typeof(TextsTextModelTextLengthUnits));
            while (doubleTextsTextModelLength > textsTextModelTextLengthUnitsDivisor && cycleIndex++ < textsTextModelTextLengthUnitsNames.Length)
                doubleTextsTextModelLength /= textsTextModelTextLengthUnitsDivisor;
            convertedTextsTextModelTextLength = doubleTextsTextModelLength.ToString() + " " + textsTextModelTextLengthUnitsNames[cycleIndex].ToString();
            if (doubleTextsTextModelLength > 1)
                convertedTextsTextModelTextLength += "s";
            return convertedTextsTextModelTextLength;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        {
            string unconvertedValue = String.Empty;
            int textsTextModelTextLength;
            try { textsTextModelTextLength = System.Convert.ToInt32(value); }
            catch { return unconvertedValue; }
            return (textsTextModelTextLength > 0) ? ConvertTextsTextModelTextLengthToUnit(textsTextModelTextLength) : unconvertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private enum TextsTextModelTextLengthUnits
        {
            Byte,

            KByte,

            MByte,

            GByte
        }
    }
}
