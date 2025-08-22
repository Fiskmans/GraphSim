using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
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
    }
}
