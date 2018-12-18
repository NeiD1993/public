using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TypingTest.ViewModel;
using TypingTest.Resource.Class;

namespace TypingTest.View
{
    /// <summary>
    /// Interaction logic for TextsView.xaml
    /// </summary>
    public partial class TextsView : Window
    {
        public TextsView()
        {
            InitializeComponent();
            textsViewListBox.SetWindowFrameworkElementPropertyBinding(TextsViewModel.textsTextModelsPropertyName + ".TextsTextModelsObservableCollection", 
                                                                      ListBox.ItemsSourceProperty);
            textsViewListBox.SetWindowFrameworkElementPropertyBinding(TextsViewModel.textsTextModelsPropertyName + ".SelectedTextsTextModel", 
                                                                      ListBox.SelectedItemProperty, frameworkElementBindingMode : BindingMode.TwoWay);
        }

        private void textsViewListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
