using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ModBusTPU.Commands
{
    public class RemoteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _executeFunction;

        public RemoteCommand(Action ExecuteFunc)
        {
            _executeFunction = ExecuteFunc;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _executeFunction?.Invoke();
        }
    }
}
