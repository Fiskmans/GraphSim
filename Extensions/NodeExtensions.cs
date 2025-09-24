using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class NodeExtensions
    {
        public static T GetFirstParentOfType<T>(this Godot.Node self) where T : Godot.Node
        {
            Godot.Node parent = self.GetParent();

            if (parent == null)
                return null;

            if (parent is T)
                return parent as T;

            return parent.GetFirstParentOfType<T>();
        }

        public static IEnumerable<T> GetSiblings<T>(this Godot.Node self) where T : Godot.Node
        {
            Godot.Node parent = self.GetParent();

            if (parent == null)
                return [];

            return
                parent.GetChildren()
                .Where(n => !ReferenceEquals(n, self))
                .Where(n => n is T)
                .Select(n => n as T);
        }

        public static IEnumerable<T> GetChildrenOfType<T>(this Godot.Node self, bool recursive = false)
            where T : Godot.Node
        {
            foreach (Godot.Node child in self.GetChildren())
            {
                if (child is T)
                {
                    yield return child as T;
                }
                else if (recursive)
                {
                    foreach (T hoisted in child.GetChildrenOfType<T>(recursive))
                        yield return hoisted;
                }
            }
        }
    }
}
