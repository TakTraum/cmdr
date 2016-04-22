using System;
using System.Linq;
using System.Collections.Generic;

namespace cmdr.Editor.Utils
{
    public class Category<T>
    {
        public Category<T> Parent { get; set; }

        /// <summary>
        /// Name of Category. Must be unique.
        /// </summary>
        public string Name { get; set; }
        public List<T> Elements { get; set; }
        public List<Category<T>> Categories { get; set; }


        public Category()
        {
            Name = String.Empty;
            Elements = new List<T>();
            Categories = new List<Category<T>>();
        }

        
        public Category<T> this[string name]
        {
            get
            {
                if (!Categories.Any(c => c.Name == name))
                    Categories.Add(new Category<T> { Name = name, Parent = this });
                return Categories.Single(c => c.Name == name);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Category<T>;
            if (other == null)
                throw new ArgumentException("Object must be of type Category");

            return Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
