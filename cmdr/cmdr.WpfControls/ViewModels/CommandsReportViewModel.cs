//using cmdr.TsiLib.Conditions;
using System.Collections.Generic;
using System.Linq;


using cmdr.WpfControls.Utils;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using cmdr.WpfControls.CustomDataGrid;


namespace cmdr.WpfControls.ViewModels
{
    public class CommandsReportViewModel : ViewModelBase
    {
        internal Selector ParentSelector;

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

        //this is a second copy of command, to enable easy "OR" greps
        private string _command2;
        public string Command2
        {
            get
            {
                return _command2;
            }
            set
            {
            }
        }


        private string _type;
        public string Type
        {
            get
            {
                return _type;
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



        public CommandsReportViewModel(string device, string command, string type, int count)
        {
            _device = device;
            _command = command;
            _command2 = command;
            _type = type;
            _count = count;
        }
    }
}
