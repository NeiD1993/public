using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TypingTest.Resource.Class
{
    class Command : ICommand
    {
        private readonly Action<object> execute;

        private readonly Predicate<object> canExecute;

        public Command(Action<object> execute) : this(execute, null) { }

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter) { execute(parameter); }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter) { return (canExecute == null) ? true : canExecute(parameter); }

        public event EventHandler CanExecuteChanged;
    }
}
