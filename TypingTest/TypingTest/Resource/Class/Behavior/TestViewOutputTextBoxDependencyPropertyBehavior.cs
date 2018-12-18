using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TypingTest.Resource.Class.Behavior
{
    class TestViewOutputTextBoxDependencyPropertyBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(TestViewOutputTextBoxDependencyPropertyBehavior),
                                                                                                new FrameworkPropertyMetadata { BindsTwoWayByDefault = false });

        private PropertyInfo propertyInfo;

        public string PropertyName { get; set; }

        public object Binding
        {
            get { return GetValue(BindingProperty); }

            set { SetValue(BindingProperty, value); }
        }

        protected override void OnAttached()
        {
            if (PropertyName != null)
                propertyInfo = AssociatedObject.GetType().GetProperty(PropertyName);
            base.OnAttached();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Binding")
            {
                if (AssociatedObject != null)
                {
                    if (propertyInfo.CanWrite)
                        propertyInfo.SetValue(AssociatedObject, e.NewValue, null);
                    base.OnPropertyChanged(e);
                }
            }
        }
    }
}
