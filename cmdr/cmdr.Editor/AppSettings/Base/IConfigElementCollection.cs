using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings.Base
{
    public interface IConfigElementCollection<T> : IEnumerable<T>
    {
        T this[int index] { get; }
        T this[string key] { get; }
        void Add(T element);
        void Remove(string key);
        void Clear();
    }
}
