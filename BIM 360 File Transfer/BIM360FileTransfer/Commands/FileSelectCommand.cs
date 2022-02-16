using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM360FileTransfer.Commands
{
    public class FileSelectCommand : ICommand
    {
        public FileSelectCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            FileSelectExecute = execute;
            FileSelectCanExecute = canExecute;
        }

        protected Func<object, bool> FileSelectCanExecute { get; }

        protected Action<object> FileSelectExecute { get; }

        public virtual bool CanExecute(object parameter)
        {
            bool result;
            try
            {
                var fileSelectCanExecute = FileSelectCanExecute;
                result = fileSelectCanExecute == null || fileSelectCanExecute(parameter);
            }
            catch (Exception o)
            {
                result = false;
            }

            return result;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            try
            {
                var fileSelectExecute = FileSelectExecute;
                fileSelectExecute?.Invoke(parameter);
            }
            catch (Exception o)
            {

            }
        }
    }
}
