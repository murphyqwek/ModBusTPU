using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestMODBUS.Commands
{
    public class RemoteCommandWithParameter : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _executeFunction;

        public RemoteCommandWithParameter(Action<object> ExecuteFunc)
        {
            _executeFunction = ExecuteFunc;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _executeFunction?.Invoke(parameter);
        }
    }
}
