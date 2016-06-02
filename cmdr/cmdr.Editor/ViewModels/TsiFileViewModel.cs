﻿using ChangeTracking;
using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.Editor.Views;
using cmdr.TsiLib;
using cmdr.TsiLib.EventArgs;
using cmdr.WpfControls.Behaviors;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            set { _selectedDevice = value; raisePropertyChanged("SelectedDevice"); }
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

        #endregion


        private TsiFileViewModel(TsiFile tsiFile)
        {
            _tsiFile = tsiFile;

            // Is new file?
            if (tsiFile.Path == null)
                IsChanged = true;
            else
            {
                foreach (var device in _tsiFile.Devices)
                {
                    var dvm = new DeviceViewModel(device);
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

        public async Task<bool> SaveAsync(string filepath)
        {
            AcceptChanges();
            App.SetStatus("Saving " + filepath + " ...");
            bool success = await Task<bool>.Factory.StartNew(() => _tsiFile.Save(filepath));

            if (success)
            {
                raisePropertyChanged("Path");
                raisePropertyChanged("Title");
            }
            else
                IsChanged = true;
            App.ResetStatus();
            return success;
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

        private void onDeviceChanged()
        {
            IsChanged = Devices.Any(d => d.IsChanged);
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
            var dvm = new DeviceViewModel(rawDevice);
            Devices.Add(dvm);
        }

        private void insertDevice(int index, Device rawDevice)
        {
            _tsiFile.InsertDevice(index, rawDevice);
            var dvm = new DeviceViewModel(rawDevice);
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

        private void removeDevice(DeviceViewModel device)
        {
            int id = device.Id;
            Devices.Remove(device);
            _tsiFile.RemoveDevice(id);
        }

        private string makeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

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
            return await Task<TsiFile>.Factory.StartNew(() => TsiFile.Load(CmdrSettings.Instance.TraktorVersion, filePath));
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
