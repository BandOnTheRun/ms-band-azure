using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataSync
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value) == true)
                return false;
            field = value;
            RaisePropertyChanged(propName);
            return true;
        }

        protected void RaisePropertyChanged(string propName)
        {
            var pc = PropertyChanged;
            if (pc == null)
                return;
            pc(this, new PropertyChangedEventArgs(propName));
        }
    }
}
