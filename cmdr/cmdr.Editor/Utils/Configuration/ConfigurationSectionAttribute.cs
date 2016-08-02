using System;

namespace cmdr.Editor.Utils.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigurationSectionAttribute : Attribute
    {
        private readonly string _name;
        public string Name { get { return _name; } }


        public ConfigurationSectionAttribute(string name)
        {
            _name = name;
        }
    }
}
