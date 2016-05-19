using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSBandAzure.Mvvm
{
    public class AsyncDelegateCommand<TArgType> : IAsyncCommand
    {
        Func<TArgType, Task<TArgType>> _executeAsyncDelegate;
        Func<TArgType, bool> _canExecuteDelegate;

        public AsyncDelegateCommand(Func<TArgType, Task<TArgType>> ExecuteAsyncDelegate, Func<TArgType, bool> CanExecuteDelegate = null)
        {
            _executeAsyncDelegate = ExecuteAsyncDelegate;
            _canExecuteDelegate = CanExecuteDelegate;
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null)
                return;
            handler(this, new EventArgs());
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecuteDelegate == null ? true : _canExecuteDelegate((TArgType)parameter);
        }

        public async void Execute(object parameter)
        {
            await AsyncRunner(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            await AsyncRunner(parameter);
        }

        protected virtual async Task AsyncRunner(object parameter)
        {
            await _executeAsyncDelegate((TArgType)parameter);
        }
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
