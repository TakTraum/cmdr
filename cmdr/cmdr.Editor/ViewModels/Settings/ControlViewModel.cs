using ChangeTracking;
using cmdr.Editor.Views;
using cmdr.TsiLib.Controls;
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
        private AControl _control;

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

        private Dictionary<Setting, System.Reflection.PropertyInfo> _propertyDict;

        private List<Setting> getSettings()
        {
            _propertyDict = new Dictionary<Setting, System.Reflection.PropertyInfo>();

            string[] ignored = { "Type" };

            var detailsSettings = new List<Setting>();

            Type t;

            t = _control.GetType();
            var props = t.GetProperties();

            int i = 0;
            Setting s = null;
            string name;
            foreach (var p in props)
            {
                Type targetType = p.PropertyType;
                name = p.Name + ":";

                if (targetType == null || ignored.Contains(p.Name))
                    continue;
                else if (targetType == typeof(string))
                    s = new StringSetting(i++, name);
                else if (targetType == typeof(int))
                    s = new IntSetting(i++, name);
                else if (targetType == typeof(bool))
                    s = new BoolSetting(i++, name);
                else if (targetType == typeof(Single))
                    s = new SingleSetting(i++, name);
                else if (targetType == typeof(object))
                    s = new StringSetting(i++, name);
                else if (targetType.IsEnum)
                {
                    var d1 = typeof(EnumSetting<>);
                    Type[] typeArgs = { targetType };
                    var makeme = d1.MakeGenericType(typeArgs);
                    s = Activator.CreateInstance(makeme, i++, name) as Setting;
                }
                else
                    s = null;

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
