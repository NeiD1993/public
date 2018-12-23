﻿#pragma checksum "..\..\..\View\TestView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0D3DE354C453DA9AA649AD798678D136"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TypingTest.Resource.Class.Behavior;
using TypingTest.Resource.Class.Converter;
using TypingTest.Resource.Class.Converter.TestView;


namespace TypingTest.View {
    
    
    /// <summary>
    /// TestView
    /// </summary>
    public partial class TestView : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 45 "..\..\..\View\TestView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox testViewOutputTextBox;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\..\View\TestView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock testViewMessageTextBlock;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\View\TestView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox testViewInputTextBox;
        
        #line default
        #line hidden
        
        
        #line 327 "..\..\..\View\TestView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button testViewCancelButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TypingTest;component/view/testview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\TestView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            this.testViewOutputTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 45 "..\..\..\View\TestView.xaml"
            this.testViewOutputTextBox.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.testViewOutputTextBox_PreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 45 "..\..\..\View\TestView.xaml"
            this.testViewOutputTextBox.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.testViewInputTextBox_LostFocusPreviewMouseRightButtonDownPreviewMouseRightButtonUp_testViewOutputTextBox_LostFocusPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.testViewMessageTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.testViewInputTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 133 "..\..\..\View\TestView.xaml"
            this.testViewInputTextBox.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.testViewInputTextBox_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 133 "..\..\..\View\TestView.xaml"
            this.testViewInputTextBox.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.testViewInputTextBox_PreviewMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 133 "..\..\..\View\TestView.xaml"
            this.testViewInputTextBox.PreviewMouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.testViewInputTextBox_LostFocusPreviewMouseRightButtonDownPreviewMouseRightButtonUp_testViewOutputTextBox_LostFocusPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 133 "..\..\..\View\TestView.xaml"
            this.testViewInputTextBox.PreviewMouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.testViewInputTextBox_LostFocusPreviewMouseRightButtonDownPreviewMouseRightButtonUp_testViewOutputTextBox_LostFocusPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.testViewCancelButton = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 1:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.LostFocusEvent;
            
            #line 32 "..\..\..\View\TestView.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.testViewInputTextBox_LostFocusPreviewMouseRightButtonDownPreviewMouseRightButtonUp_testViewOutputTextBox_LostFocusPreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 3:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.ScrollViewer.ScrollChangedEvent;
            
            #line 48 "..\..\..\View\TestView.xaml"
            eventSetter.Handler = new System.Windows.Controls.ScrollChangedEventHandler(this.testViewOutputTextBox_ScrollViewerScrollChanged);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}
