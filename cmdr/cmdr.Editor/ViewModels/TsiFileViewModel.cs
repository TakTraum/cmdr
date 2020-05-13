using ChangeTracking;
using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.Editor.Views;
using cmdr.Editor.ViewModels.Reports;
using cmdr.TsiLib;
using cmdr.TsiLib.EventArgs;
using cmdr.WpfControls.Behaviors;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using cmdr.WpfControls.ViewModels;
using cmdr.WpfControls.CustomDataGrid;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;

namespace cmdr.Editor.ViewModels
{
    public class TsiFileViewModel : AReversible
    {
        private readonly TsiFile _tsiFile;


        public bool IsTraktorSettings { get { return _tsiFile.IsTraktorSettings; } }

        private string defaultTitle
        {
            get
            {
                if (Devices.Count() == 1 && !String.IsNullOrWhiteSpace(Devices.Single().Comment))
                    return makeValidFileName(Devices.Single().Comment);
                return "Untitled";
            }
        }

        public string Title { get { return (_tsiFile.Path != null) ? new FileInfo(Path).Name : defaultTitle; } }

        public string Path { get { return _tsiFile.Path; } }

        private ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices ?? (_devices = new ObservableCollection<DeviceViewModel>()); }
        }

        private DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set {
                _selectedDevice = value;
                raisePropertyChanged("SelectedDevice");

                remember_cgd();

                if (CmdrSettings.Instance.ClearFilterAtPageChanges) {
                    SelectedDevice.ClearFiltering();
                }
            }
        }

        private static ObservableCollection<MenuItemViewModel> _addDeviceMenuItems;
        public static ObservableCollection<MenuItemViewModel> AddDeviceMenuItems
        {
            get { return _addDeviceMenuItems ?? (_addDeviceMenuItems = new ObservableCollection<MenuItemViewModel>(generateAddDeviceMenuItems())); }
        }


        #region Commands

        private ICommand _addDeviceCommand;
        public ICommand AddDeviceCommand
        {
            get { return _addDeviceCommand ?? (_addDeviceCommand = new CommandHandler<MenuItemViewModel>(addDevice)); }
        }

        private ICommand _removeDeviceCommand;
        public ICommand RemoveDeviceCommand
        {
            get { return _removeDeviceCommand ?? (_removeDeviceCommand = new CommandHandler(removeDevice, () => SelectedDevice != null)); }
        }

        private ICommand _dragOverCommand;
        public ICommand DragOverCommand
        {
            get { return _dragOverCommand ?? (_dragOverCommand = new CommandHandler<DragEventArgs>(dragOver)); }
        }

        private ICommand _dropCommand;
        public ICommand DropCommand
        {
            get { return _dropCommand ?? (_dropCommand = new CommandHandler<IDataObject>(drop)); }
        }

        private ICommand _showCommandsReportEditorCommand;
        public ICommand ShowCommandsReportEditorCommand
        {
            get { return _showCommandsReportEditorCommand ?? (_showCommandsReportEditorCommand = new CommandHandler(showCommandsReportEditor)); }
        }

        private ICommand _showMappingsReport;
        public ICommand ShowMappingsReport
        {
            get { return _showMappingsReport ?? (_showMappingsReport = new CommandHandler(showMappingsReport)); }
        }

        #endregion


        private TsiFileViewModel(TsiFile tsiFile)
        {
            _tsiFile = tsiFile;

            // Is new file?
            if (tsiFile.Path == null)
                IsChanged = true;
            else {
                foreach (var device in _tsiFile.Devices) {
                    var dvm = new DeviceViewModel(device, this);
                    Devices.Add(dvm);
                    dvm.DirtyStateChanged += (s, a) => onDeviceChanged();
                }

                // Set selection if possible
                SelectedDevice = Devices.FirstOrDefault();

                AcceptChanges();
            }

            Devices.CollectionChanged += Devices_CollectionChanged;
        }


        public static TsiFileViewModel Create()
        {
            return new TsiFileViewModel(TsiFile.Create(CmdrSettings.Instance.TraktorVersion));
        }

        public static async Task<TsiFileViewModel> LoadAsync(string filePath)
        {
            TsiFileViewModel result = null;
            App.SetStatus("Opening " + filePath + " ...");
            TsiFile.EffectIdentificationRequest += onEffectIdentificationRequest;

            var tsiFile = await loadTsiAsync(filePath);
            if (tsiFile != null)
                result = new TsiFileViewModel(tsiFile);
            TsiFile.EffectIdentificationRequest -= onEffectIdentificationRequest;
            App.ResetStatus();
            return result;
        }

 
        public string generate_csv_string(string sep = ",")
        {
            StringBuilder sb = new StringBuilder(Convert.ToString((char)65279));

            foreach (var dev in Devices)
                dev.SaveMetadata();

            bool header_printed = false;

            Devices.Enumerate((dev, i) =>
            {
                string dev_name = dev.Comment;

                sb.AppendFormat("#\n#Page {0}: ({1})\n#\n", i, dev_name);
                foreach (var m in dev.Mappings) {
                    var mvm = (MappingViewModel)m.Item;

                    if (!header_printed) {
                        sb.AppendFormat("{0}\n", mvm.get_csv_row(sep, i, true));
                        header_printed = true;
                    }

                    sb.AppendFormat("{0}\n", mvm.get_csv_row(sep, i));
                }
            });

            return sb.ToString();
        }

        private bool WriteToXls(string filepath, string dataToWrite )
        {
            try
            {
                FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                StreamWriter objWrite = new StreamWriter(fs);
                objWrite.Write(dataToWrite);
                objWrite.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> SaveAsyncCsv(string filepath)
        {
            AcceptChanges();
            App.SetStatus("Saving " + filepath + " ...");

            var data = generate_csv_string(",");

            bool success = WriteToXls(filepath, data);

            App.ResetStatus();
            return success;
        }

        public async Task<bool> SaveAsyncTsi(string filepath, bool backup = false)
        {
            App.SetStatus("Saving " + filepath + " ...");

            // Remove EMPTY devices at save
            if (CmdrSettings.Instance.RemoveUnusedMIDIDefinitions) {
                foreach (var dev in Devices.ToArray()) {
                    if (dev.Mappings.Any()) {
                        continue;
                    }
                    removeDevice(dev, true);
                }
            }

            AcceptChanges();

            // save metadata
            foreach (var dev in Devices) {
                dev.SaveMetadata();
            }

            bool success = await Task<bool>.Factory.StartNew(() => _tsiFile.Save(filepath, CmdrSettings.Instance.OptimizeFXList, backup));

            if (success) {
                raisePropertyChanged("Path");
                raisePropertyChanged("Title");
            } else {
                IsChanged = true;
            }
            App.ResetStatus();
            return success;
        }

        /*
        class Person
        {
            internal int id;
            internal string car;
            internal string type;
        }

        public static void test_linq_groupby()
        {
            List<Person> persons = new List<Person>();
            persons.Add(new Person { id = 1, car = "Ferrari", type = "convertible" });
            persons.Add(new Person { id = 1, car = "BMW", type = "convertible" });
            persons.Add(new Person { id = 1, car = "Audi", type = "utility" });
            persons.Add(new Person { id = 2, car = "Audi", type = "utility" });


            // https://stackoverflow.com/questions/847066/group-by-multiple-columns
            var results = from p in persons
                          group p.car 
                          by new { p.id, p.type } into g
                          select new
                          {
                              g.Key.id,
                              g.Key.type,
                              quantity = g.Count()
                          };

            foreach (var p in results) {
                Console.WriteLine("{0} {1} {2} ",
                                  p.id,
                                  p.type,
                                  p.quantity);
            }

        }
        */

        private void showCommandsReportEditor()
        {
            var rows = new List<CommandsReportViewModel>();
            foreach (var dev in Devices)
            {
                string dev_name = dev.Comment;

                // https://stackoverflow.com/questions/847066/group-by-multiple-columns
                var commands1 = dev.Mappings
                    .Select(m => (m.Item as MappingViewModel))
                    .Select(m => new { command = m.Command.Name, type = m.Type });

                var commands2 = from c in commands1
                              group c.command
                              by new { c.command, c.type } into g
                              select new
                              {
                                  g.Key.command,
                                  type = g.Key.type,
                                  count = g.Count()
                              };

                var new_rows = commands2.Select(c => new CommandsReportViewModel(dev_name, c.command, c.type, c.count));
                rows.AddRange(new_rows);
            }

            // sort by "Command"
            rows = rows.OrderBy(r => r.Command).ToList();

            var dc = new CommandsReportEditorViewModel(rows);
            var new_window = new Views.CommandsReportEditor
            {
                DataContext = dc
            };
            new_window.ShowDialog();

        }

        private void showMappingsReport()
        {
            var rows = new List<CommandsReportViewModel>();
            foreach (var dev in Devices) {
                string dev_name = dev.Comment;

                // https://stackoverflow.com/questions/847066/group-by-multiple-columns
                var commands1 = dev.Mappings
                    .Select(m => (m.Item as MappingViewModel))
                    .Select(m => new { command = m.MappedTo, type = m.Type });

                var commands2 = from c in commands1
                                group c.command
                                // by new { c.command, c.type } into g
                                by new { c.command } into g
                                select new
                                {
                                    g.Key.command,
                                    // type = g.Key.type,
                                    count = g.Count()
                                };

                var new_rows = commands2.Select(c => new CommandsReportViewModel(dev_name, c.command, null, c.count));
                rows.AddRange(new_rows);
            }

            // sort by "Command"
            rows = rows.OrderBy(r => r.Command).ToList();

            var dc = new CommandsReportEditorViewModel(rows);
            var new_window = new Views.CommandsReportEditor
            {
                DataContext = dc
            };
            new_window.ShowDialog();

        }

        protected override void Accept()
        {
            foreach (var dev in Devices)
                dev.AcceptChanges();
        }

        protected override void Revert()
        {
            foreach (var dev in Devices)
                dev.RevertChanges(); 
        }


        private void dragOver(DragEventArgs e)
        {
            IDataObject dataObject = e.Data;
            if (dataObject == null)
                return;

            var data = dataObject.GetData(typeof(DraggableRowsBehavior.Data)) as DraggableRowsBehavior.Data;
            if (data == null)
                return;

            var lbItem = VisualHelpers.FindAncestor<ListBoxItem>(e.OriginalSource);
            if (lbItem != null)
                lbItem.IsSelected = true;

            // add appropriate device for mappings, if file is empty
            if (!Devices.Any())
            {
                var copy = (data.SenderDataContext as DeviceViewModel).Copy(false);
                addDevice(copy);
                SelectedDevice = Devices.First();
            }
        }

        private void drop(IDataObject dataObject)
        {
            if (dataObject == null)
                return;

            var data = dataObject.GetData(typeof(DraggableRowsBehavior.Data)) as DraggableRowsBehavior.Data;
            if (data == null)
                return;

            if (data.TargetIndex < 0 && Devices.Any()) // // don't allow invalid targets, but allow drop on an empty grid 
                return;

            TsiFileViewModel srcFile = data.SenderDataContext as TsiFileViewModel;
            if (srcFile == null)
                return;

            DeviceViewModel selected = data.SelectedItems[0] as DeviceViewModel;

            int newIndex = Math.Max(0, data.TargetIndex);

            if (srcFile != this || !data.IsMove)
            {
                if (data.IsMove)
                    srcFile.removeDevice(selected);

                var rawDevice = selected.Copy(true);
                insertDevice(newIndex++, rawDevice);
            }
            else
            {
                var movingAction = new Action<int, int>((oi, ni) =>
                {
                    _tsiFile.MoveDevice(oi, ni);
                    _devices.Move(oi, ni);
                });
                MovingLogicHelper.Move<DeviceViewModel>(_devices, new List<DeviceViewModel> { selected }, newIndex, movingAction);
            }

            SelectedDevice = selected;
        }


        // pestrela 4 May 2020:
        //   This is a link to the datagrid.
        //   It was moved to the TSI level to be able to clear filtering with zero mappings.
        //  hcnaging TSIs reconstructs the whole thing (?) so the filters are cleared
        private CustomDataGrid _cdg_parent_selector = null;
        public CustomDataGrid CDG_ParentSelector
        {
            get
            {
                return _cdg_parent_selector;
            }
            set
            {
                //empty setter on purpose
            }
        }

        private bool remember_cgd_inner(bool debug = false)
        {
            if (debug) {
                int i = 9;
            }

            if (SelectedDevice == null) {
                return false;
            }
            if (!SelectedDevice.Mappings.Any()) {
                return false;
            }

            var first_m = SelectedDevice.Mappings.First();
            var first_p = SelectedDevice.Mappings.First().ParentSelector;

            if (first_p == null) {
                return false;
            }

            var what = SelectedDevice.Mappings.First().ParentSelector;
            if (!(what is CustomDataGrid)) {
                return false;
            }


            CustomDataGrid new_cdg = (CustomDataGrid)what;
            if (_cdg_parent_selector == null) {
                _cdg_parent_selector = new_cdg;
                return true;
            }

            if (_cdg_parent_selector != new_cdg) {
                return false;
            }

            // we remember the SAME datagrid. The only sane outcome
            return true;
        }

        public void remember_cgd()
        {
            bool sucess = remember_cgd_inner();
            if (!sucess) {
                remember_cgd_inner(true);
            }

        }

        // fixme: how to trigger this at startup?
        // how to have a two-way syncronized value? so that the datagrid reads this value instead of the opposite
        public void updateShowColumns(ShowColumns showColumns)
        {
            if (this.CDG_ParentSelector != null) {
                this.CDG_ParentSelector.updateShowColumns(showColumns);
            } else {
                var i = 9;
                // warn user?
            }
        }


        public void ClearFiltering()
        {
            if (this.CDG_ParentSelector != null) {
                this.CDG_ParentSelector.ClearFiltering();
            } else {
                var i = 9;
                // warn user?
            }
        }

        public bool HasFiltering()
        {
            if (this.CDG_ParentSelector != null) {
                return this.CDG_ParentSelector.HasFiltering();
            } else {
                return true;  // be carefull by default!
            }
        }

        public void ReApplyFiltering()
        {
            if (this.CDG_ParentSelector != null) {
                this.CDG_ParentSelector.ReApplyFiltering();
            } else {
                var i = 9;
                // warn user?
            }
        }


        private void onDeviceChanged()
        {
            IsChanged = Devices.Any(d => d.IsChanged);

            // desperation starts to kick-in.
            remember_cgd();

        }

        private static IEnumerable<MenuItemViewModel> generateAddDeviceMenuItems()
        {
            List<MenuItemViewModel> items = new List<MenuItemViewModel>
            {
                new MenuItemViewModel { Text = Device.TYPE_STRING_GENERIC_MIDI }
            };

            var groups = ControllerDefaultMappings.Instance.GroupBy(cdm => cdm.Manufacturer);
            var defaults = groups.Select(g => new MenuItemViewModel
            {
                Text = g.Key,
                Children = g.Select(c => new MenuItemViewModel { Text = c.Controller, Tag = c }).ToList()
            });

            return items.Union(defaults);
        }

        public void addMidiDevice()
        {
            addDevice(new MenuItemViewModel { Text = Device.TYPE_STRING_GENERIC_MIDI });
        }

        private async void addDevice(MenuItemViewModel item)
        {
            ControllerDefaultMappings.ControllerDefaultMappingFile defFile = item.Tag as ControllerDefaultMappings.ControllerDefaultMappingFile;

            App.SetStatus("Loading defaults for " + item.Text + " ...");

            if (item.Text.Equals(Device.TYPE_STRING_GENERIC_MIDI))
                addDevice(_tsiFile.CreateDevice(Device.TYPE_STRING_GENERIC_MIDI));
            else if (defFile != null)
            {
                TsiFile tsi = defFile.TsiFile;
                if (tsi == null)
                    tsi = await defFile.LoadAsync();
                if (tsi != null)
                {
                    bool includeMappings = MessageBoxHelper.ShowQuestion("Do you want to load the default mappings too?");
                    foreach (var d in tsi.Devices)
                    {
                        var copy = d.Copy(includeMappings);
                        if (copy.TypeStr.Equals(Device.TYPE_STRING_GENERIC_MIDI) && String.IsNullOrEmpty(copy.Comment))
                            copy.Comment = item.Text;
                        addDevice(copy);
                    }
                }
            }

            App.ResetStatus();
            if (Devices.Any())
                SelectedDevice = Devices.LastOrDefault();
        }

        private void addDevice(Device rawDevice)
        {
            _tsiFile.AddDevice(rawDevice);
            var dvm = new DeviceViewModel(rawDevice, this);
            Devices.Add(dvm);
        }

        private void insertDevice(int index, Device rawDevice)
        {
            _tsiFile.InsertDevice(index, rawDevice);
            var dvm = new DeviceViewModel(rawDevice, this);
            Devices.Insert(index, dvm);
        }


        private void removeDevice()
        {
            int index = Devices.IndexOf(SelectedDevice);

            removeDevice(SelectedDevice);
            
            int count = Devices.Count;
            if (count > 0)
                SelectedDevice = Devices[(index < count) ? index : index - 1];
            else
                SelectedDevice = null;
        }
        
        private void removeDevice(DeviceViewModel device, bool is_optimizing_tsi = false)
        {
            int id = device.Id;

            if (!is_optimizing_tsi && CmdrSettings.Instance.ConfirmDeleteDevices) {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this device?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                    return;

            };

            Devices.Remove(device);
            _tsiFile.RemoveDevice(id);
        }

        private string makeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            // replace invalid chars with "_"
            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private static void solveEffectIdentification(EffectIdentificationRequest request)
        {
            EffectIdentificationWindow eiw = new EffectIdentificationWindow(request);
            eiw.Owner = App.Current.MainWindow;
            eiw.ShowDialog();
            request.Handled = true;
        }

        private static async Task<TsiFile> loadTsiAsync(string filePath)
        {
            try {
                return await Task<TsiFile>.Factory.StartNew(() => TsiFile.Load(
                    CmdrSettings.Instance.TraktorVersion, 
                    filePath, 
                    CmdrSettings.Instance.RemoveUnusedMIDIDefinitions,
                    false
                    ));
            }
            catch (Exception e) {

                if (CmdrSettings.Instance.VerboseExceptions) {
                    MessageBoxHelper.ShowException("Error loading " + filePath, e);
                }
                return null;
            }

        }

        #region Events

        static void onEffectIdentificationRequest(object sender, EffectIdentificationRequest e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() => solveEffectIdentification(e)));
        }

        void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (DeviceViewModel dvm in e.NewItems)
                        dvm.DirtyStateChanged += (s, a) => onDeviceChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (DeviceViewModel dvm in e.OldItems)
                        dvm.DirtyStateChanged -= (s, a) => onDeviceChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (DeviceViewModel dvm in e.OldItems)
                        dvm.DirtyStateChanged -= (s, a) => onDeviceChanged();
                    foreach (DeviceViewModel dvm in e.NewItems)
                        dvm.DirtyStateChanged += (s, a) => onDeviceChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

            IsChanged = true;
        }

        #endregion
    }
}
