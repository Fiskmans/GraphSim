using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public interface IMapModifier
    {
        IEnumerable<Rect2I> GetBlockedRegions();
    }
}
