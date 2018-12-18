using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TypingTest.View
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : Window
    {
        private bool isMouseLeftButtonDown = false;
        
        public TestView()
        {
            InitializeComponent();
            testViewOutputTextBox.Focus();
        }

        private void testViewInputTextBox_LostFocusPreviewMouseRightButtonDownPreviewMouseRightButtonUp_testViewOutputTextBox_LostFocusPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void testViewInputTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = true;
            if (testViewInputTextBox.Text.Length > 0)
            {
                object eOriginalSource = e.OriginalSource;
                if (!((eOriginalSource is Rectangle) || (eOriginalSource is Polygon)))
                {
                    switch (e.LeftButton)
                    {
                        case MouseButtonState.Pressed:
                            e.Handled = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void testViewInputTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMouseLeftButtonDown)
            {
                int testViewInputTextBoxTextLength = testViewInputTextBox.Text.Length;
                if (testViewInputTextBoxTextLength == 0)
                    testViewInputTextBox.CaretIndex = 0;
                else
                {
                    object eOriginalSource = e.OriginalSource;
                    if (!((eOriginalSource is Thumb) || (eOriginalSource is RepeatButton) || (eOriginalSource is Rectangle) || (eOriginalSource is Grid)))
                    {
                        testViewInputTextBox.CaretIndex = testViewInputTextBoxTextLength;
                        e.Handled = true;
                    }
                }
                isMouseLeftButtonDown = false;
            }
        }

        private void testViewOutputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
                e.Handled = true;
        }

        private void testViewOutputTextBox_ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
                testViewOutputTextBox.CaretIndex = testViewOutputTextBox.SelectionStart;
            else e.Handled = true;
        }
    }
}
