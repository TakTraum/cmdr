using cmdr.TsiLib.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.Editor.ViewModels.Reports
{
    public class CommandsReportViewModel : ViewModelBase
    {
        
        private string _device;
        public string Device
        {
            get
            {
                return _device;
            }
            set
            {

            }

        }

        private string _command;
        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
            }
        }

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
        }


        public CommandsReportViewModel(string device, string command, int count)
        {
            _device = device;
            _command = command;
            _count = count;
        }
    }
}
