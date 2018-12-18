using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TypingTest.Resource.Class.Converter.Texts
{
    class TextsViewListBoxSourceItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double convertedValue = 0;
            ListBox textsViewListBox = value as ListBox;
            if (textsViewListBox != null)
            {
                ItemCollection textsViewListBoxItems = textsViewListBox.Items as ItemCollection;
                if (textsViewListBoxItems != null)
                {
                    double doubleParameter;
                    string stringParameter = parameter as string;
                    if ((stringParameter != null) && Double.TryParse(stringParameter, out doubleParameter) && (doubleParameter > 0))
                    {
                        Thickness textsViewListBoxBorderThickness = textsViewListBox.BorderThickness;
                        convertedValue = (textsViewListBoxItems.Count > 1) ? doubleParameter : (textsViewListBox.Height - (textsViewListBoxBorderThickness.Top + textsViewListBoxBorderThickness.Bottom + 2));
                    }
                }
            }
            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

