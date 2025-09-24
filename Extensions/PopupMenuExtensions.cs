using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class PopupMenuExtensions
    {
        public struct SubMenuMapping<T>
        {
            public T Obj;
            public PopupMenu Menu;
        }

        public static IEnumerable<SubMenuMapping<T>> AddSubmenuRange<T>(this PopupMenu self, IEnumerable<T> options, Func<T, string> nameGetter)
        {
            if (options != null)
            {
                foreach (T option in options)
                {
                    PopupMenu subMenu = new PopupMenu();
                    self.AddSubmenuNodeItem(nameGetter(option), subMenu);

                    yield return new SubMenuMapping<T> { Obj = option, Menu = subMenu };
                }
            }
        }


        public static void AddRange<T>(this PopupMenu self, IEnumerable<T> options, Func<T, string> nameGetter, Action<T> selection)
        {
            if (selection == null)
                return;

            T[] cache = options.ToArray();

            int start = self.ItemCount;
            int at = start;
            
            foreach (T option in options)
            {
                self.AddItem(nameGetter(option), at);
                at++;
            }

            self.IdPressed += (id) =>
            {
                if (id < start || id >= at)
                    return;

                T pressed = cache[(int)id - start];

                selection(pressed);
            };
        }

    }
}
