using cmdr.Editor.Utils;
using cmdr.WpfControls.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
     public class FilterCmdViewModel : ViewModelBase
    {
         private readonly DeviceViewModel _dvm;
         private int _lastSearchPos;
         private IEnumerable<MappingViewModel> _lastSearchResult;

         public string SearchText { get; set; }

         private ICommand _searchCommand;
         public ICommand SearchCommand
         {
             get { return _searchCommand ?? (_searchCommand = new CommandHandler(search)); }
         }

         private bool _isFound = true;
         public bool IsFound
         {
             get { return _isFound; }
             set { _isFound = value; raisePropertyChanged("IsFound"); }
         }


         public FilterCmdViewModel(DeviceViewModel dvm)
         {
             _dvm = dvm;
         }


         public bool Continue()
         {
            return true;
            
         }

     

        private void search()
         {
            string to_search = SearchText.ToLower();
            _dvm.limit_add_mapping_menus(to_search);
        }


    }
}
