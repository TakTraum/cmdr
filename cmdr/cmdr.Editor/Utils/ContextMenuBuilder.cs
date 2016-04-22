using cmdr.TsiLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace cmdr.Editor.Utils
{
    class ContextMenuBuilder<T> where T: IMenuProxy
    {
        public static ContextMenu Build(IEnumerable<T> proxies, Action<T> callback)
        {
            ContextMenu menu = new ContextMenu();
            var tree = BuildTree(proxies, callback);
            foreach (var category in tree.Categories)
                suck(category, menu, callback);
            return menu;
        }


        private static void suck(Category<T> category, ItemsControl menu, Action<T> callback)
        {
            MenuItem item = new MenuItem { Header = category.Name };
            category.Elements.ForEach(c => item.Items.Add(new MenuItem { Header = c.Name, Command = new CommandHandler<T>(callback), CommandParameter = c }));
            category.Categories.ForEach(c => suck(c, item, callback));
            menu.Items.Add(item);
        }

        private static Category<T> BuildTree(IEnumerable<T> proxies, Action<T> callback)
        {
            var allInCategories = proxies.Select(i => i.Category).Distinct().OrderBy(i => i.ToDescriptionString());

            string pathSeparator = "->";
            string[] catPath = null;
            
            Category<T> root = new Category<T> { Name = "Root" };

            foreach (var cat in allInCategories)
            {
                catPath = cat.ToDescriptionString().Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                Category<T> target = root;
                foreach (var c in catPath)
                    target = target[c];

                target.Elements = proxies.Where(i => i.Category == cat).ToList();
            }
            return root;
        }
    }
}
