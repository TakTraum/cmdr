using cmdr.Editor.Utils;
using cmdr.WpfControls.CustomDataGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
     public class SearchViewModel : ViewModelBase
    {
         private DeviceViewModel _dvm;
         private int _lastSearchPos;
         private IEnumerable<MappingViewModel> _lastSearchResult;

         public string SearchText { get; set; }

         private ICommand _searchCommand;
         public ICommand SearchCommand
         {
             get { return _searchCommand ?? (_searchCommand = new CommandHandler(search, _dvm.Mappings.Any)); }
         }


         public SearchViewModel(DeviceViewModel dvm)
         {
             _dvm = dvm;
         }


         public bool Continue()
         {
             int count = _lastSearchResult.Count();
             if (count > _lastSearchPos)
             {
                 var select = _dvm.Mappings.Single(m => m.Item == _lastSearchResult.ElementAt(_lastSearchPos));
                 highlight(select);
                 _lastSearchPos++;
             }
             bool hasNext = (count > _lastSearchPos);
             if (!hasNext)
                 _lastSearchPos = 0;
             return hasNext;
         }

         private void search()
         {
             if (SearchText.Length < 2)
                 return;

             var comparer = CultureInfo.CurrentCulture.CompareInfo;
             // TODO: Sorting!
             _lastSearchResult = _dvm.Mappings.Select(m => m.Item as MappingViewModel)
                 .Where(m => comparer.IndexOf(m.Command.Name.ToLower(), SearchText.ToLower(), CompareOptions.IgnoreCase) >= 0);

             if (_lastSearchResult.Any())
             {
                 _lastSearchPos = 1;
                 var select = _dvm.Mappings.Single(m => m.Item == _lastSearchResult.First());
                 highlight(select);
             }
         }

         private void highlight(RowItemViewModel item)
         {
             item.BringIntoView();
             _dvm.SelectedMappings.Clear();
             _dvm.SelectedMappings.Add(item);
         }
    }
}
