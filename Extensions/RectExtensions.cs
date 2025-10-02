using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class RectExtensions
    {
        public static bool Contains(this Rect2 self, Vector2 p)
        {
            if (p.X < self.Position.X)
                return false;
            if (p.Y < self.Position.Y)
                return false;
            if (p.X > self.Position.X + self.Size.X)
                return false;
            if (p.Y > self.Position.Y + self.Size.Y)
                return false;

            return true;
        }
    }
}
