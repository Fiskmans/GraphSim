using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class TraceFinder : Node2D
    {
        public struct TraceCoord
        {
            public int X;
            public int Y;
            public Direction D;

            public TraceCoord Offset(Direction d)
            {
                return new TraceCoord { X = X + d.Offset().X, Y = Y + d.Offset().Y, D = d };
            }
        }

        public event Action<List<Vector2I>> OnCompleted;

        struct Node
        {
            public Node() { }

            public Direction Back = Direction.East;
            public bool Seen = false;
        }

        Site Site;
        Node[,,] Map;
        double Budget;

        List<TraceCoord> Queue = new();

        ref Node this[TraceCoord coord]
        {
            get => ref Map[coord.X, coord.Y, (int)coord.D];
        }

        public TraceFinder(Site site, TraceCoord entryPoint)
        {
            Site = site;
            Map = new Node[site.MapWidth, site.MapHeight, 8];

            GD.Print($"Starting trace at ({entryPoint.X},{entryPoint.Y}) going {entryPoint.D}");

            Check(entryPoint);
        }

        public override void _Draw()
        {
            List<Vector2I> gridLines = new();

            for (int x = 0; x < Site.MapWidth; x++)
            {
                for (int y = 0; y < Site.MapHeight; y++)
                {
                    foreach(Direction d in Enum.GetValues<Direction>())
                    {
                        Node n = this[new TraceCoord { X = x, Y = y, D = d }];

                        if (n.Seen)
                        {
                            gridLines.Add(new Vector2I(x, y));
                            gridLines.Add(new Vector2I(x, y) + d.Reversed().Offset());
                        }
                    }
                }
            }

            DrawMultiline(gridLines.Select(v => (v + new Vector2(0.5f,0.5f)) * Constants.NodeSpacing).ToArray(), new Color(1, 1, 1));
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            Budget += delta;
            if (Budget > 1)
            {
                Budget -= 1;
                QueueRedraw();
                for (int i = 0; i < 100; i++)
                    Step();
            }
        }

        bool InBounds(TraceCoord coord)
        {
            if (coord.X < 0 || coord.Y < 0)
                return false;

            if (coord.X >= Site.MapWidth)
                return false;

            if (coord.Y >= Site.MapHeight)
                return false;

            return true;
        }

        IEnumerable<TraceCoord> Neighboors(TraceCoord coord)
        {
            yield return coord.Offset(coord.D);
            yield return coord.Offset(coord.D.Prev());
            yield return coord.Offset(coord.D.Next());
        }

        void Check(TraceCoord coord)
        {
            ref Node n = ref this[coord];

            if (n.Seen)
                return;

            n.Back = coord.D.Reversed();
            n.Seen = true;

            Queue.Add(coord);
        }

        public void Step()
        {
            TraceCoord at = Queue.First();
            Queue.RemoveAt(0);

            foreach (TraceCoord coord in Neighboors(at).Where(InBounds))
                Check(coord);

            if (Queue.Count == 0)
            {
                OnCompleted?.Invoke([]);
                QueueFree();
            }
        }
    }
}
