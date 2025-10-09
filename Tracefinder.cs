using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using GraphSim.Ui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Godot.Color;

namespace GraphSim
{
    public partial class TraceFinder : Node2D
    {
        public const int BasePenality = 1;

        public struct TraceCoord : IEquatable<TraceCoord>
        {
            public Vector2I Pos;
            public Direction D;

            public override string ToString()
            {
                return $"{Pos.X}, {Pos.Y}, {D}";
            }

            public bool Equals(TraceCoord other)
            {
                return Pos == other.Pos
                    && D == other.D;
            }

            public TraceCoord Offset(Direction d)
            {
                return new TraceCoord
                {
                    Pos = Pos + d.Offset(),
                    D = d
                };
            }
        }

        public event Action<List<Vector2I>> OnCompleted;

        struct Node
        {
            public Node() { }

            public TraceCoord Back;
            public bool Seen = false;
            public bool Start = false;
            public bool Blocked = false;
        }

        Site Site;
        Node[,,] Map;
        Rect2I Bounds;
        double Budget;

        struct Tip : IComparable<Tip>
        {
            public TraceCoord From;
            public TraceCoord To;

            public int W;
            public int StepsSinceTurn;
            public int H;
            public Tip()
            {
                W = 0;
                StepsSinceTurn = 0;
                H = int.MaxValue;
            }

            public Tip Offset(Direction d)
            {
                int turnPenality;

                switch (StepsSinceTurn)
                {
                    case 0:
                        turnPenality = 60;
                        break;
                    case 1:
                        turnPenality = 30;
                        break;
                    case 2:
                        turnPenality = 10;
                        break;
                    case 3:
                    case 4:
                        turnPenality = 3;
                        break;
                    case 5:
                    case 6:
                        turnPenality = 2;
                        break;
                    default:
                        turnPenality = 1;
                        break;
                }

                if (To.D == d)
                    turnPenality = 0;

                int penalities = BasePenality + turnPenality;

                return new Tip
                {
                    From = To,
                    To = To.Offset(d),
                    W = W + penalities,
                    StepsSinceTurn = d == To.D ? StepsSinceTurn + 1 : 0
                };
            }

            int IComparable<Tip>.CompareTo(Tip other)
            {
                return (W + H).CompareTo(other.W + other.H);
            }
        }

        List<Tip> Queue = new();
        List<TraceCoord> Targets = new();

        ref Node this[TraceCoord coord]
        {
            get => ref Map[coord.Pos.X, coord.Pos.Y, (int)coord.D];
        }

        public TraceFinder(Site site, TraceCoord entryPoint, TraceCoord target)
        {
            Site = site;
            Map = new Node[site.MapWidth, site.MapHeight, 8];
            Bounds = new Rect2I(0, 0, site.MapWidth, site.MapHeight);

            if (!InBounds(entryPoint))
            {
                GD.PrintErr($"Tracing from outside the map {entryPoint}");
                QueueFree();
                return;
            }
            if (!InBounds(target))
            {
                GD.PrintErr($"Tracing to outside the map {target}");
                QueueFree();
                return;
            }

            for (int x = 0; x < site.MapWidth; x++)
            {
                for (int y = 0; y < site.MapHeight; y++)
                {
                    if (site.Map[x,y] != Site.GridNode.Stable)
                    {
                        foreach (Direction direction in Enum.GetValues<Direction>())
                        {
                            Map[x,y,(int)direction].Blocked = true;
                        }
                    }
                }
            }

            Targets.Add(target);

            GD.Print($"Starting trace at {entryPoint} targeting {target}");

            this[entryPoint].Start = true;

            Enqueue(new Tip
            {
                To = entryPoint
            });
        }

        private List<Vector2I> Bake(TraceCoord coord)
        {
            TraceCoord at = coord;
            List<Vector2I> res = new List<Vector2I>();

            while (!this[at].Start)
            {
                res.Add(at.Pos);
                at = this[at].Back;

                if (!InBounds(at))
                    return res;

                if (res.Count > 1000)
                    return res;
            }

            res.Add(at.Pos);

            return res;
        }

        public void DrawCoord(TraceCoord coord, Color color)
        {
            Vector2 center = (coord.Pos + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;
            Vector2 tip = center + ((Vector2)coord.D.Offset()).Normalized() * 5;
            Vector2[] lines = [
                center,
                tip,

                tip,
                center + ((Vector2)coord.D.Next().Offset()).Normalized() * 3,

                tip,
                center + ((Vector2)coord.D.Prev().Offset()).Normalized() * 3,
            ];

            DrawMultiline(lines, color, width:0.8f, antialiased: true);
        }

        public override void _Draw()
        {
            Func<Vector2I, Vector2> scale = (v) => (v + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;

            List<Vector2I> gridLines = new();

            int count = 0;

            const int full = 50;
            const int point = 500;

            foreach (Tip tip in Queue)
            {
                count++;

                TraceCoord coord = tip.From;

                if (count > point)
                    continue;

                if (count > full)
                {
                    if (this[coord].Seen)
                        DrawCoord(coord, new Color(1, 0, 0, (point - (float)count) / (point - full)));
                    else
                        DrawCoord(coord, new Color(1, 1, 0, (point - (float)count) / (point - full)));
                    continue;
                }

                List<Vector2I> trace = Bake(coord);

                if (trace.Count() == 0)
                {
                    DrawCoord(coord, new Color(1, 0, 0));
                    continue;
                }

                DrawCoord(coord, new Color(1, 1, (full - (float)count) / full));

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

            foreach (TraceCoord target in Targets)
                DrawCoord(target, new Color(0.5f, 1, 0.5f));
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            Budget += delta * 100;
            while (Budget > 1)
            {
                Budget -= 1;
                QueueRedraw();
                Step();
            }
        }

        bool InBounds(TraceCoord coord)
        {
            return Bounds.Contains(coord.Pos);
        }

        void UpdateHeuristic(ref Tip tip)
        {
            int best = int.MaxValue;
            ref Node n = ref this[tip.To];

            foreach (TraceCoord t in Targets)
            {
                TraceCoord next = tip.To;

                Vector2I delta = t.Pos - next.Pos;
                
                int distance = int.Max(int.Abs(delta.X), int.Abs(delta.Y));
                int rot = next.D.StepsTo(t.D);

                int val = 0;
                val += distance * BasePenality;
                if (distance < 10)
                    val += rot * BasePenality * (9 - distance);
                else
                    val += 9 * BasePenality;

                if (!next.Equals(t) && n.Blocked)
                    tip.H += 1000;

                best = int.Min(val, best);
            }
            tip.H = 1;
        }

        bool Visit(Tip tip)
        {
            ref Node n = ref this[tip.To];

            if (n.Seen)
                return false;

            n.Back = tip.From;
            n.Seen = true;

            foreach (TraceCoord t in Targets)
            {
                if (tip.To.Equals(t))
                {
                    OnCompleted?.Invoke(Bake(tip.To));
                    QueueFree();
                    return false;
                }
            }
            return true;
        }

        void Enqueue(Tip tip)
        {
            if (!InBounds(tip.To))
                return;

            ref Node n = ref this[tip.To];

            if (n.Seen)
            {
                GD.Print("skip");
                return;
            }

            UpdateHeuristic(ref tip);

            int index = Queue.BinarySearch(tip);

            if (index < 0) index = ~index;

            Queue.Insert(index, tip);
        }

        public void Step()
        {
            Tip at = Queue.First();
            Queue.RemoveAt(0);

            if (Visit(at))
            {
                Enqueue(at.Offset(at.To.D));
                Enqueue(at.Offset(at.To.D.Next()));
                Enqueue(at.Offset(at.To.D.Prev()));
            }

            if (Queue.Count == 0)
            {
                OnCompleted?.Invoke([]);
                QueueFree();
            }
        }
    }
}
