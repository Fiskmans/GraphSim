using Godot;
using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class Vector2IExtensions
    {
        public static Vector2 ScaledToGrid(this Vector2I self)
        {
            return (self + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;
        }
    }
}
