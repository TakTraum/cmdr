using ChangeTracking;
using cmdr.Editor.Views;
using cmdr.TsiLib.Controls;
using cmdr.TsiLib.Enums;
using SettingControlLibrary.SettingControls;
using SettingControlLibrary.SettingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cmdr.Editor.ViewModels.Settings
{
    public class ControlViewModel : AReversible
    {
        private readonly AControl _control;
        
        private Dictionary<Setting, System.Reflection.PropertyInfo> _propertyDict;

        public IEnumerable<BaseSettingControl> SettingControls { get; private set; }

        private ContentControl _settingsContent;
        public ContentControl SettingsContent
        {
            get { return _settingsContent; }
            set { _settingsContent = value; raisePropertyChanged("SettingsContent"); }
        }


        public ControlViewModel(AControl control)
        {
            _control = control;
            updateContent();
            AcceptChanges();
        }

       
        private void updateContent()
        {
            var settings = getSettings();
            var controls = settings.Select(setting => SettingControlLibrary.SettingControlFactory.Create(setting));
            SettingsContent = new SettingsEditor(controls);
        }


        private List<Setting> getSettings()
        {
            _propertyDict = new Dictionary<Setting, System.Reflection.PropertyInfo>();

            string[] ignored = { "Type", "AllowedInteractionModes" };

            var detailsSettings = new List<Setting>();

            Type t;

            t = _control.GetType();
            var props = t.GetProperties().Where(p => !ignored.Contains(p.Name));

            int i = 0;
            Setting s = null;
            string name;
            foreach (var p in props)
            {
                Type targetType = p.PropertyType;
                name = p.Name;

                switch (name)
                {
                    case "Invert":
                    case "Blend":
                        s = new BoolSetting(i++, name + ":");
                        break;
                    case "SoftTakeOver":
                        s = new BoolSetting(i++, "Soft Takeover:");
                        break;
                    case "AutoRepeat":
                        s = new BoolSetting(i++, "Auto Repeat:");
                        break;
                    case "RotaryAcceleration":
                        s = new IntSetting(i++, "Rotary Acceleration:", 0, 100);
                        break;
                    case "RotarySensitivity":
                        s = new IntSetting(i++, "Rotary Sensitivity:", 0, 300);
                        break;
                    case "Mode":
                        s = new EnumSetting<MidiEncoderMode>(i++, name + ":");
                        break;
                    case "MidiRangeMin":
                        s = new IntSetting(i++, "MIDI Range Min:", 0, 127);
                        break;
                    case "MidiRangeMax":
                        s = new IntSetting(i++, "MIDI Range Max:", 0, 127);
                        break;
                    case "Resolution":
                        s = new EnumSetting<MappingResolution>(i++, name + ":");
                        break;
                    default:
                        s = null;
                        break;
                }

                if (s != null)
                {
                    object rawValue = p.GetValue(_control, null);
                    
                    s.TryParse(rawValue.ToString());
                    s.AcceptChanges();
                    detailsSettings.Add(s);
                    _propertyDict.Add(s, p);
                    s.PropertyChanged += s_PropertyChanged;
                }
                else
                {
                    // Datatype not supported
                }
            }
            return detailsSettings;
        }

        void s_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var setting = sender as Setting;

            var val = setting.GetType().GetProperty("Value").GetValue(setting);
            _propertyDict[setting].SetValue(_control, val);

            IsChanged = true;
        }

        protected override void Accept()
        {

        }

        protected override void Revert()
        {

        }
    }
}
