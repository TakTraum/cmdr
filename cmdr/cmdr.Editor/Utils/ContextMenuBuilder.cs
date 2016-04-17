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
            Category<T> root = new Category<T> { Name = "Root" };
            Category<T> lastCat = root;
            Category<T> tmpCat = null;
            string[] catPath = null;
            int lastLevel = 0;
            int level = 0;
            
            foreach (var cat in allInCategories)
            {
                catPath = cat.ToDescriptionString().Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                level = catPath.Length;

                tmpCat = new Category<T>()
                {
                    Name = catPath.Last(),
                    Elements = proxies.Where(i => i.Category == cat).ToList()
                };

                if (level > lastLevel)
                {
                    // assert path (like makedirs)
                    if (level - lastLevel > 1 || (catPath.Length > 1 && catPath.First() != lastCat.Name))
                    {
                        Category<T> walk = root;
                        Category<T> tmpWalk = null;
                        foreach (var p in catPath.Take(catPath.Length - 1))
                        {
                            tmpWalk = walk.Categories.SingleOrDefault(c => c.Name == p);
                            if (tmpWalk == null)
                            {
                                tmpWalk = new Category<T> { Name = p, Parent = walk };
                                walk.Categories.Add(tmpWalk);
                            }
                            walk = tmpWalk;
                        }
                        lastCat = walk;
                    }

                    tmpCat.Parent = lastCat;
                }
                else if (level == lastLevel)
                {
                    tmpCat.Parent = lastCat.Parent;
                }
                else if (level < lastLevel)
                {
                    var diff = lastLevel - level;
                    while (diff-- > 0)
                        lastCat = lastCat.Parent;
                    tmpCat.Parent = lastCat.Parent;
                }

                tmpCat.Parent.Categories.Add(tmpCat);
                lastCat = tmpCat;
                lastLevel = level;
            }
            return root;
        }
    }
}
