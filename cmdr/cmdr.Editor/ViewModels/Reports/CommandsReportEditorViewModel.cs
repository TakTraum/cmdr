using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cmdr.Editor.ViewModels.Reports
{
    public class CommandsReportEditorViewModel : ViewModelBase
    {  
        public ObservableCollection<CommandsReportViewModel> Rows { get; private set; }

        public CommandsReportEditorViewModel(List<CommandsReportViewModel> ret)
        {

            Rows = new ObservableCollection<CommandsReportViewModel>(ret);
        }
    }
}
