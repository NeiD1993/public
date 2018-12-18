using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TypingTest.Model.TextModel;
using TypingTest.Model.TextModel.TextsTextModel;
using TypingTest.ViewModel;

namespace TypingTest.Resource.Class
{
    static class ExtensionMethods
    {
        public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
        {
            Window window = ((FrameworkElement)templateFrameworkElement).TemplatedParent as Window;
            if ((window != null) && (action != null)) action(window);
        }

        public static void SetWindowFrameworkElementPropertyBinding(this FrameworkElement frameworkElement, string frameworkElementPropertyPath, 
                                                                    DependencyProperty frameworkElementProperty, string stringFormat = null, 
                                                                    BindingMode frameworkElementBindingMode = BindingMode.OneWay, IValueConverter frameworkElementConverter = null)
        {
            Binding frameworkElementPropertyBinding = new Binding();
            frameworkElementPropertyBinding.Path = new PropertyPath(frameworkElementPropertyPath);
            frameworkElementPropertyBinding.StringFormat = stringFormat;
            frameworkElementPropertyBinding.Mode = frameworkElementBindingMode;
            frameworkElementPropertyBinding.Converter = frameworkElementConverter;
            frameworkElement.SetBinding(frameworkElementProperty, frameworkElementPropertyBinding);
        }

        public static void TryExecuteWindowDataContextObjectAction<WindowDataContextClass>(this object windowDataContextObject, Action<WindowDataContextClass> windowDataContextAction)
                           where WindowDataContextClass : BaseViewModel
        {
            WindowDataContextClass windowDataContext = windowDataContextObject as WindowDataContextClass;
            if ((windowDataContext != null) && (windowDataContextAction != null))
                windowDataContextAction(windowDataContext);
        }

        public static void TryExecuteWindowObjectAction<WindowClass>(this object windowObject, Action<WindowClass> windowAction) where WindowClass : Window
        {
            WindowClass window = windowObject as WindowClass;
            if ((window != null) && (windowAction != null))
                windowAction(window);
        }

        public static bool EqualsMainTextModel(this FileInfo file, MainTextModel mainTextModel)
        {
            if (mainTextModel != null)
                return ((file.Name == mainTextModel.TextName) && (file.DirectoryName == mainTextModel.TextDirectoryName) && (file.LastAccessTime == mainTextModel.TextCreationTime)) ? true : false;
            else return false;
        }

        public static WindowClass TryFindWindowInOwnerWindowOwnedWindows<WindowClass>(this Window ownerWindow) where WindowClass : Window
        {
            WindowClass window = null;
            foreach (Window ownedWindow in ownerWindow.OwnedWindows)
            {
                window = ownedWindow as WindowClass;
                if (window != null)
                    break;
            }
            return window;
        }

        public static object TryReturnPotentiallyEqualTextsTextModelsResult(this object firstTextsTextModelObject, object secondTextsTextModelObject, object firstReturnedResult, 
                                                                            object secondReturnedResult)
        {
            object returnedResult = null;
            TextsTextModel textsTextModel = firstTextsTextModelObject as TextsTextModel;
            TextsTextModel chosenTextsTextModel = secondTextsTextModelObject as TextsTextModel;
            returnedResult = ((firstTextsTextModelObject != null) && (secondTextsTextModelObject != null) && firstTextsTextModelObject.Equals(secondTextsTextModelObject) &&
                              (firstReturnedResult != null) && (secondReturnedResult != null)) ? firstReturnedResult : secondReturnedResult;
            return returnedResult;
        }
    }
}
