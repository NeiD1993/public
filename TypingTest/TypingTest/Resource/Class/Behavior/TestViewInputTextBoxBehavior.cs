using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace TypingTest.Resource.Class.Behavior
{
    class TestViewInputTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty PreviewKeyDownAddCommandProperty = DependencyProperty.Register("PreviewKeyDownAddCommand", typeof(ICommand), typeof(TestViewInputTextBoxBehavior),
                                                                                                                 new FrameworkPropertyMetadata { BindsTwoWayByDefault = false });

        public static readonly DependencyProperty PreviewKeyDownRemoveCommandProperty = DependencyProperty.Register("PreviewKeyDownRemoveCommand", typeof(ICommand), typeof(TestViewInputTextBoxBehavior),
                                                                                                                    new FrameworkPropertyMetadata { BindsTwoWayByDefault = false });

        public static readonly DependencyProperty PreviewTextInputCommandProperty = DependencyProperty.Register("PreviewTextInputCommand", typeof(ICommand), typeof(TestViewInputTextBoxBehavior),
                                                                                                                new FrameworkPropertyMetadata { BindsTwoWayByDefault = false });

        public ICommand PreviewKeyDownAddCommand
        {
            get { return (ICommand)GetValue(PreviewKeyDownAddCommandProperty); }

            set { SetValue(PreviewKeyDownAddCommandProperty, value); }
        }

        public ICommand PreviewKeyDownRemoveCommand
        {
            get { return (ICommand)GetValue(PreviewKeyDownRemoveCommandProperty); }

            set { SetValue(PreviewKeyDownRemoveCommandProperty, value); }
        }

        public ICommand PreviewTextInputCommand
        {
            get { return (ICommand)GetValue(PreviewTextInputCommandProperty); }

            set { SetValue(PreviewTextInputCommandProperty, value); }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!AssociatedObject.IsReadOnly)
            {
                Key eKey = e.Key;
                if ((Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) || (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)))
                    e.Handled = true;
                else
                {
                    switch (e.Key)
                    {
                        case Key.Down:
                        case Key.End:
                        case Key.Home:
                        case Key.Insert:
                        case Key.Left:
                        case Key.PageDown:
                        case Key.PageUp:
                        case Key.Right:
                        case Key.Tab:
                        case Key.Up:
                            e.Handled = true;
                            break;
                        case Key.Enter:
                        case Key.Space:
                            TryExecuteTestViewInputTextBoxBehaviorCommand(PreviewKeyDownAddCommand, e);
                            break;
                        case Key.Back:
                            TryExecuteTestViewInputTextBoxBehaviorCommand(PreviewKeyDownRemoveCommand, e);
                            break;
                        default:
                            break;
                    }
                }
            }
            else e.Handled = true;
        }

        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e) 
        {
            string eText = e.Text;
            if (eText.Length == 1)
                TryExecuteTestViewInputTextBoxBehaviorCommand(PreviewTextInputCommand, e, eText.ToCharArray().First());
            else e.Handled = true;
        }

        private void TryExecuteTestViewInputTextBoxBehaviorCommand(ICommand testViewInputTextBoxBehaviorCommand, RoutedEventArgs e, object canExecuteParameter = null)
        {
            if (testViewInputTextBoxBehaviorCommand != null)
            {
                if (testViewInputTextBoxBehaviorCommand.CanExecute(canExecuteParameter))
                    testViewInputTextBoxBehaviorCommand.Execute(null);
                else e.Handled = true;
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += new KeyEventHandler(AssociatedObject_PreviewKeyDown);
            AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(AssociatedObject_PreviewTextInput);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= new KeyEventHandler(AssociatedObject_PreviewKeyDown);
            AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(AssociatedObject_PreviewTextInput);
            base.OnDetaching();
        }
    }
}
