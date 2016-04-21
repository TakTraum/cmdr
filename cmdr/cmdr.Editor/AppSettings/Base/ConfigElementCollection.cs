using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings.Base
{
    public class ConfigElementCollection<T> : ConfigurationElementCollection, IConfigElementCollection<T> where T : ConfigElement, new()
    {
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
            return ((T)element).Key;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (string key in base.BaseGetAllKeys())
                yield return (this[key] as T);
        }
    }
}
