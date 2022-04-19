using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CollectionTest1
{
    public class RelayCommand : ICommand
    {
        Action execute;
        Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute)
        {
            this.execute = execute;
            this.canExecute = (_) => { return true; };
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public bool CanExecute(object param)
        {
            return canExecute(param);
        }

    }
}
