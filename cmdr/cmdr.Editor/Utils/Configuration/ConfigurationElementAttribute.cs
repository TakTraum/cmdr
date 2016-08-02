using System;

namespace cmdr.Editor.Utils.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigurationElementAttribute : Attribute
    {
        private readonly string _name;
        public string Name { get { return _name; } }


        public ConfigurationElementAttribute(string name)
        {
            _name = name;
        }
    }
}
