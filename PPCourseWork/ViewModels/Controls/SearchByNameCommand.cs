using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PPCourseWork.ViewModels.Controls
{
    public class SearchByNameCommand : ICommand
    {
        private readonly MainVM _mainVM;

        public SearchByNameCommand(MainVM mainVM)
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
            _mainVM.SearchByName();
        }
    }
}
