using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PPCourseWork.ViewModels.Controls
{
    public class LoadCSVCommand : ICommand
    {
        private readonly MainVM _mainVM;

        public LoadCSVCommand(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            _mainVM.LoadCSV();
        }
    }
}
