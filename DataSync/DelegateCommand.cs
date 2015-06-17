using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataSync
{
    public class DelegateCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _method;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> method)
            : this(method, null)
        {
        }

        public DelegateCommand(Action<object> method, Predicate<object> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            bool canExecute = true;

            if (canExecute)
            {
                _method.Invoke(parameter);
            }
        }

        public virtual void OnCanExecuteChanged(EventArgs e)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, e);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }
    }
}
