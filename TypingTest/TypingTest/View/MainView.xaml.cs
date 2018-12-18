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
using TypingTest.Model.TimerModel;
using TypingTest.ViewModel;
using TypingTest.Resource.Class;
using TypingTest.Resource.Class.Converter;

namespace TypingTest.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new MainTimerModel(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(9)), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(250),
                                            TimeSpan.FromMilliseconds(8));
            string mainTimerModelTimePropertyPath = MainViewModel.mainTimerModelPropertyName + ".Time";
            DependencyProperty textblockTextProperty = TextBlock.TextProperty;
            mainViewSingleDigitTextBlock.SetWindowFrameworkElementPropertyBinding(mainTimerModelTimePropertyPath, textblockTextProperty, "%m");
            mainViewDoubleDigitTextBlock.SetWindowFrameworkElementPropertyBinding(mainTimerModelTimePropertyPath, textblockTextProperty, "ss");
            mainViewCheckBox.SetWindowFrameworkElementPropertyBinding(MainViewModel.mainTextModelPropertyName, CheckBox.IsCheckedProperty, frameworkElementConverter : new MainTextModelConverter());
        }
    }
}
