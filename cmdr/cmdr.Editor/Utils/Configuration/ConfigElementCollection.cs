using System;
using System.Collections.Generic;
using System.Configuration;

namespace cmdr.Editor.Utils.Configuration
{
    public sealed class ConfigElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        public ConfigElementCollection()
        {
            AddElementName = getChildElementName();
        }


        public T this[int index]
        {
            get
            {
                return base.BaseGet(index) as T;
            }
        }

        public new T this[string key]
        {
            get
            {
                return base.BaseGet(key) as T;
            }
        }

        public void Add(T element)
        {
            base.BaseAdd(element);
        }

        public void Remove(string key)
        {
            base.BaseRemove(key);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            foreach (PropertyInformation p in element.ElementInformation.Properties)
            {
                if (p.IsKey)
                    return p.Value;
            }

            try
            {
                return element.ElementInformation.Properties[element.ElementInformation.Properties.Keys[0]].Value;
            }
            catch (Exception)
            {
                return new InvalidOperationException("Configuration Element has no Key");
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (string key in base.BaseGetAllKeys())
                yield return (this[key] as T);
        }

        
        private string getChildElementName()
        {
            ConfigurationElementAttribute cea = Attribute.GetCustomAttribute(typeof(T), typeof(ConfigurationElementAttribute)) as ConfigurationElementAttribute;
            return (cea != null) ? cea.Name : typeof(T).Name;
        }
    }
}
