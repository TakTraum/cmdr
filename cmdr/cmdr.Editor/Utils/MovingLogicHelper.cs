using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.Editor.Utils
{
    class MovingLogicHelper
    {
        public static void Move<T>(IList<T> all, IList<T> selected, int newIndex, Action<int, int> moveAction)
        {
            int oldIndex;

            for (int i = 0; i < selected.Count(); i++)
            {
                oldIndex = all.IndexOf(selected[i]);

                if (oldIndex < newIndex || newIndex == all.Count) // if drag is downwards
                    newIndex--;

                if (i > 0)
                {
                    newIndex = all.IndexOf(selected[i - 1]);
                    if (oldIndex > newIndex) // if drag is upwards
                        newIndex++;
                }

                if (oldIndex == newIndex)
                    continue;

                moveAction(oldIndex, newIndex);
            }
        }
    }
}
