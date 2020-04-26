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
            /* 
             * example usage:
             * 
            var strings = new List<string>();
            strings.Enumerate((str, n) =>
            {
                // hooray
            });
            */
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }




        //Enum.GetValues(typeof(ModifierValue)).Cast<ModifierValue>().Max();

        // https://stackoverflow.com/questions/203377/getting-the-max-value-of-an-enum 
        // https://www.davidyardy.com/blog/visual-studio%E2%80%93how-to-target-differentlatest-c-version-net-core-3-and-c-8/
        // https://devblogs.microsoft.com/dotnet/a-belated-welcome-to-c-7-3/
        public static TEnum GetMaxValue<TEnum>()
        where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Max();
        }

        public static TEnum GetMinValue<TEnum>()
        where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Min();
        }


        public static TEnum GetMaxValueOld<TEnum>()
        where TEnum : Enum
        {
            Type type = typeof(TEnum);

            if (!type.IsSubclassOf(typeof(Enum)))
                throw new
                    InvalidCastException
                        ("Cannot cast '" + type.FullName + "' to System.Enum.");

            return (TEnum)Enum.ToObject(
                type,
                Enum.GetValues(type).Cast<int>().Last()
                );
        }

        // https://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp
        // return eRat.B.Next();
        public static TEnum EnumRotate<TEnum>(this TEnum src, int step) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(TEnum).FullName));

            TEnum[] Arr = (TEnum[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<TEnum>(Arr, src);

            int count = Arr.Length;
            j = j + step;
            while (true) {
                if (j < 0) {
                    j = j + count;

                } else if (j >= count) {
                    j = j - count;

                } else {
                    break;
                }
            }

            TEnum ret = Arr[j];

            return ret;
        }

        // todo: move to "typeextensions.c"
        // https://stackoverflow.com/questions/3519539/how-to-check-if-a-string-contains-any-of-some-strings
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            // usage: bool anyLuck = s.ContainsAny("a", "b", "c");
            foreach (string needle in needles) {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

    }
}




