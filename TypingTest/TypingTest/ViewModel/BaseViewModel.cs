using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingTest.ViewModel
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string changedPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(changedPropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
