using ChangeTracking;
using cmdr.Editor.AppSettings;
using cmdr.Editor.Utils;
using cmdr.TsiLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class TsiFileViewModel : AReversible
    {
        public bool IsTraktorSettings { get { return _tsiFile.IsTraktorSettings; } }


        private readonly TsiFile _tsiFile;
        private string defaultTitle
        {
            get
            {
                if (Devices.Count() == 1 && !String.IsNullOrWhiteSpace(Devices.Single().Comment))
                    return makeValidFileName(Devices.Single().Comment);
                return "New Mapping";
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
            set
            {
                _selectedDevice = value; 
                raisePropertyChanged("SelectedDevice");

                // Set selection if possible
                if (_selectedDevice != null)
                {
                    _selectedDevice.SelectedMapping = _selectedDevice.Mappings.FirstOrDefault();
                    if (_selectedDevice.SelectedMapping != null)
                        _selectedDevice.SelectItemsCommand.Execute(new[] { _selectedDevice.SelectedMapping });
                }
            }
        }

        #region Commands

        private ICommand _addDeviceCommand;
        public ICommand AddDeviceCommand
        {
            get { return _addDeviceCommand ?? (_addDeviceCommand = new CommandHandler(addDevice, () => true)); }
        }

        private ICommand _removeDeviceCommand;
        public ICommand RemoveDeviceCommand
        {
            get { return _removeDeviceCommand ?? (_removeDeviceCommand = new CommandHandler(removeDevice, () => SelectedDevice != null)); }
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
                    dvm.DirtyStateChanged += (s, a) => updateDevsChanged();
                }

                // Set selection if possible
                SelectedDevice = Devices.FirstOrDefault();

                AcceptChanges();
            }

            Devices.CollectionChanged += Devices_CollectionChanged;
        }

        public static TsiFileViewModel Create()
        {
            var fxSettings = (TraktorSettings.Initialized) ? TraktorSettings.Instance.FxSettings : null;
            return new TsiFileViewModel(new TsiFile(CmdrSettings.Instance.TraktorVersion, fxSettings));
        }

        public static async Task<TsiFileViewModel> LoadAsync(string filePath)
        {
            TsiFileViewModel result = null;
            App.SetStatus("Opening " + filePath + " ...");
            var tsiFile = await Task<TsiFile>.Factory.StartNew(() => TsiFile.Load(CmdrSettings.Instance.TraktorVersion, filePath));
            if (tsiFile != null)
                result = new TsiFileViewModel(tsiFile);
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


        private void updateDevsChanged()
        {
            IsChanged = Devices.Any(d => d.IsChanged);
        }

        private void addDevice()
        {
            var device = _tsiFile.CreateDevice(Device.TYPE_STRING_GENERIC_MIDI);
            _tsiFile.AddDevice(device);
            Devices.Add(new DeviceViewModel(device));
            SelectedDevice = Devices.Last();
        }

        private void removeDevice()
        {
            int id = SelectedDevice.Id;
            int index = Devices.IndexOf(SelectedDevice);
            Devices.Remove(SelectedDevice);
            _tsiFile.RemoveDevice(id);
            int count = Devices.Count;
            if (count > 0)
                SelectedDevice = Devices[(index < count) ? index : index - 1];
            else
                SelectedDevice = null;
        }

        private string makeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        #region Events

        void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (DeviceViewModel dvm in e.NewItems)
                        dvm.DirtyStateChanged += (s, a) => updateDevsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (DeviceViewModel dvm in e.OldItems)
                        dvm.DirtyStateChanged -= (s, a) => updateDevsChanged();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (DeviceViewModel dvm in e.OldItems)
                        dvm.DirtyStateChanged -= (s, a) => updateDevsChanged();
                    foreach (DeviceViewModel dvm in e.NewItems)
                        dvm.DirtyStateChanged += (s, a) => updateDevsChanged();
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
