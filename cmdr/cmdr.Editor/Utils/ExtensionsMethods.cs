using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        // C# extension methods: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
        // https://stackoverflow.com/questions/521687/foreach-with-index
        public static void Enumerate<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        /* 
         * example usage:
         * 
        var strings = new List<string>();
        strings.Enumerate((str, n) =>
        {
            // hooray
        });
        */
    }
}




