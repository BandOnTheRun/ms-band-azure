using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
