using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class Trace : Node2D
    {
        Site Site;

        Vector2[] Points;
        Color Color = new Color(1, 1, 1);

        public Trace(Site site, TraceFinder.TraceCoord entryPoint, TraceFinder.TraceCoord exitPoint)
        {
            TraceFinder finder = new TraceFinder(site, entryPoint, exitPoint);

            finder.OnCompleted += SetPath;

            AddChild(finder);
        }

        public void SetPath(List<Vector2I> path)
        {
            if (path.Count == 0)
                return;

            Func<Vector2I, Vector2> scale = (v) => (v + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;

            List<Vector2> res = [scale(path.First())];

            for (int i = 1; i < path.Count - 1; i++)
            {
                res.Add(scale(path[i]));
                res.Add(scale(path[i]));
            }
            res.Add(scale(path.Last()));

            Points = res.ToArray();

            QueueRedraw();
        }

        public override void _Draw()
        {
            base._Draw();
            if (Points != null)
                DrawMultiline(Points, Color);
        }
    }
}
