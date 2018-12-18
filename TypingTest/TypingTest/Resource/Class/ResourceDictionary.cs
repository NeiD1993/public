using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TypingTest.Resource.Class
{
    partial class ResourceDictionary
    {
        private void resourceDictionaryCloseButton_Click(object sender, RoutedEventArgs e) { sender.ForWindowFromTemplate(window => window.Close()); }

        private void resourceDictionaryDockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { sender.ForWindowFromTemplate(window => window.DragMove()); }

        private void resourceDictionaryMinimizeButton_Click(object sender, RoutedEventArgs e) { sender.ForWindowFromTemplate(window => window.WindowState = WindowState.Minimized); }
    }
}
