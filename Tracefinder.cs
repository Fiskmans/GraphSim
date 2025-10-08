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
        public struct TraceCoord : IComparable<TraceCoord>
        {
            public int X;
            public int Y;
            public Direction D;
            public int W;

            public Vector2I Flattened => new Vector2I(X, Y);

            public TraceCoord Offset(Direction d, int w)
            {
                return new TraceCoord 
                { 
                    X = X + d.Offset().X, 
                    Y = Y + d.Offset().Y, 
                    D = d, 
                    W = W + w 
                };
            }

            int IComparable<TraceCoord>.CompareTo(TraceCoord other)
            {
                return W.CompareTo(other.W);
            }
        }

        public event Action<List<Vector2I>> OnCompleted;

        struct Node
        {
            public Node() { }

            public TraceCoord Back;
            public bool Seen = false;
            public bool Start = false;
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

            for (int x = 0; x < site.MapWidth; x++)
            {
                for (int y = 0; y < site.MapHeight; y++)
                {
                    if (site.Map[x,y] != Site.GridNode.Stable)
                    {
                        foreach (Direction direction in Enum.GetValues<Direction>())
                        {
                            this[new TraceCoord { X = x, Y = y, D = direction }].Seen = true; // Works ish, should be blocked or smth instead
                        }
                    }
                }
            }
            

            GD.Print($"Starting trace at ({entryPoint.X},{entryPoint.Y}) going {entryPoint.D}");

            this[entryPoint].Start = true;
            Check(entryPoint, entryPoint);
        }

        private List<Vector2I> Bake(TraceCoord coord)
        {
            TraceCoord at = coord;
            List<Vector2I> res = new List<Vector2I>();

            while (!this[at].Start)
            {
                res.Add(at.Flattened);
                at = this[at].Back;

                if (!InBounds(at))
                    return res;

                if (res.Count > 1000)
                    return res;
            }

            res.Add(at.Flattened);

            return res;
        }

        public override void _Draw()
        {
            Func<Vector2I, Vector2> scale = (v) => (v + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;

            List<Vector2I> gridLines = new();

            foreach (TraceCoord coord in Queue)
            {
                List<Vector2I> trace = Bake(coord);

                if (trace.Count() == 0)
                {
                    DrawCircle(scale(coord.Flattened), Constants.NodeSpacing * 0.3f, new Color(1, 0, 0), antialiased: true);
                    continue;
                }

                DrawCircle(scale(coord.Flattened), Constants.NodeSpacing * 0.3f, new Color(1, 1, 1), antialiased: true);

                gridLines.Add(trace.First());

                for (int i = 1; i < trace.Count - 1; i++)
                {
                    gridLines.Add(trace[i]);
                    gridLines.Add(trace[i]);
                }
                gridLines.Add(trace.Last());
            }

            if (gridLines.Count > 0)
                DrawMultiline(gridLines.Select(scale).ToArray(), new Color(1, 1, 1, 0.1f));
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            Budget += delta * 100;
            if (Budget > 1)
            {
                Budget -= 1;
                QueueRedraw();
                for (int i = 0; i < 1000 && Queue.Count > 0; i++)
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
            yield return coord.Offset(coord.D, 1);
            yield return coord.Offset(coord.D.Prev(), 3);
            yield return coord.Offset(coord.D.Next(), 3);
        }

        void Check(TraceCoord coord, TraceCoord from)
        {
            ref Node n = ref this[coord];

            if (n.Seen)
                return;

            n.Back = from;
            n.Seen = true;

            int index = Queue.BinarySearch(coord);

            if (index < 0) index = ~index;

            Queue.Insert(index, coord);
        }

        public void Step()
        {
            TraceCoord at = Queue.First();
            Queue.RemoveAt(0);

            foreach (TraceCoord coord in Neighboors(at).Where(InBounds))
                Check(coord, at);

            if (Queue.Count == 0)
            {
                OnCompleted?.Invoke([]);
                QueueFree();
            }
        }
    }
}
