using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class Trace : Node2D
    {
        Site Site;

        List<Vector2I> Path;

        public Trace(Site site, TraceFinder.TraceCoord entryPoint)
        {
            TraceFinder finder = new TraceFinder(site, entryPoint);

            finder.OnCompleted += (p) => Path = p;

            AddChild(finder);
        }
    }
}
