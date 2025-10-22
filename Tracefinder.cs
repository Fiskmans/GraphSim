using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using GraphSim.Ui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = Godot.Color;

namespace GraphSim
{
    public partial class TraceFinder : Node2D
    {
        #region SubClasses

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

        public class NoPathException : Exception
        {
            public NoPathException(string message) : base(message)
            {

            }
        }

        struct TraceNode
        {
            public TraceNode() { }

            public TraceCoord Back;
            public bool Seen = false;
            public bool Start = false;
            public bool Exit = true;
            public bool Blocked = false;
        }
        struct Tip : IComparable<Tip>
        {
            public TraceCoord From;
            public TraceCoord To;

            public int W;
            public int StepsSinceTurn;
            public float H;
            public Tip()
            {
                W = 0;
                StepsSinceTurn = 0;
                H = float.MaxValue;
            }

            public Tip Offset(Direction d, bool blocked)
            {
                int turnPenality;

                switch (StepsSinceTurn)
                {
                    case 0:
                        turnPenality = SharpTurnPenalty;
                        break;
                    case 1:
                        turnPenality = ShortTurnPenalty;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        turnPenality = TurnPenalty + 4 - StepsSinceTurn;
                        break;
                    default:
                        turnPenality = TurnPenalty;
                        break;
                }

                if (To.D == d)
                    turnPenality = 0;

                int penalities = BasePenalty + turnPenality;

                if (blocked)
                    penalities += BlockPenalty;

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
        #endregion

        #region Constants
        public const int BasePenalty = 1;
        public const int BlockPenalty = 50;
        public const int SharpTurnPenalty = 26;
        public const int ShortTurnPenalty = 20;
        public const int TurnPenalty = 8;
        public const int MaxPathLength = 1000;
        #endregion

        enum PathMode
        {
            Djikstra,
            AStar
        }

        Resource Resource;
        bool IsSetup = false;
        TraceNode[,,] Map = null;
        Rect2I Bounds;
        double Budget;

        PathMode Mode = PathMode.AStar;

        List<Vector2> Lines = new();
        List<Tip> Queue = new();
        public bool HasExit { get; private set; }  = false;
        List<TraceCoord> To = new();
        bool HasResult = false;

        TraceCoord _Result;
        TraceCoord Result {
            get => _Result;
            set
            {
                _Result = value;
                HasResult = true;
            }
        }

        #region Shorthands
        bool InBounds(TraceCoord coord) => Bounds.Contains(coord.Pos);
        ref TraceNode this[TraceCoord coord] => ref Map[coord.Pos.X, coord.Pos.Y, (int)coord.D];

        public IEnumerable<Vector2I> GetResult() => Bake(Result);
        #endregion

        public TraceFinder(Site site, Resource resource)
        {
            Resource = resource;
            Setup(site);
        }

        public void AddEntrypoint(Ui.TraceNode entryPoint)
        {
            if (entryPoint is Ui.EndpointTraceNode end)
                AddEntrypoint(new TraceCoord { Pos = end.GridPosition, D = end.Direction.Reversed() });
            else
                foreach (Direction direction in Enum.GetValues<Direction>())
                    AddEntrypoint(new TraceCoord { Pos = entryPoint.GridPosition, D = direction });

        }

        public void AddEntrypoint(TraceCoord entryPoint)
        {
            if (!IsSetup)
                throw new InvalidOperationException("Cannot add entry before setup");

            if (!InBounds(entryPoint))
                throw new NoPathException($"Tracing from outside the map {entryPoint}");

            this[entryPoint].Start = true;
            Enqueue(new Tip { To = entryPoint });
        }

        public void AddExit(TraceCoord exit)
        {
            if (!IsSetup)
                throw new InvalidOperationException("Cannot add exits before setup");

            if (!InBounds(exit))
                throw new NoPathException($"Tracing to outside the map {exit}");

            this[exit].Exit = true;
            To.Add(exit);
            HasExit = true;
        }

        public void AddExit(Ui.TraceNode exit)
        {
            if (exit is Ui.EndpointTraceNode end)
                AddExit(new TraceCoord { Pos = end.GridPosition, D = end.Direction });
            else
                foreach (Direction direction in Enum.GetValues<Direction>())
                    AddExit(new TraceCoord { Pos = exit.GridPosition, D = direction });
        }

        public void AddExits(IEnumerable<Vector2I> exits)
        {
            if (!IsSetup)
                throw new InvalidOperationException("Cannot add exits before setup");

            Mode = PathMode.Djikstra;
            foreach (Vector2I pos in exits)
            {
                foreach (Direction direction in Enum.GetValues<Direction>())
                {
                    TraceCoord coord = new TraceCoord
                    {
                        Pos = pos,
                        D = direction
                    };

                    if (!InBounds(coord))
                        throw new NoPathException($"Tracing to outside the map {coord}");

                    this[coord].Exit = true;
                }
            }

            HasExit = true;
        }

        private List<Vector2I> Bake(TraceCoord from)
        {
            TraceCoord at = from;
            List<Vector2I> res = new List<Vector2I>();

            while (!this[at].Start)
            {
                res.Add(at.Pos);
                at = this[at].Back;

                if (!InBounds(at))
                    return res;

                if (res.Count > MaxPathLength)
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

            DrawMultiline(lines, color, width:0.3f, antialiased: true);
        }

        public override void _Draw()
        {
            if (Lines.Count > 0)
                DrawMultiline(Lines.ToArray(), Resource.Color().OnTrace().Alpha(0.01f));

            /*

            List<Vector2I> gridLines = new();

            int count = 0;

            const int full = 100;
            const int point = 100;

            foreach (Tip tip in Queue)
            {
                count++;

                TraceCoord coord = tip.To;

                if (count > point)
                    continue;

                if (count > full)
                {
                    if (this[coord].Seen)
                        DrawCoord(coord, new Color(0.3f, 0.3f, 0.3f, 0.3f * (point - (float)count) / (point - full)));
                    else
                        DrawCoord(coord, new Color(1, 1, 0, (point - (float)count) / (point - full)));
                    continue;
                }

                List<Vector2I> trace = Bake(tip.From);

                if (trace.Count() == 0)
                {
                    DrawCoord(coord, new Color(1, 0, 0));
                    continue;
                }

                //DrawCoord(coord, new Color(1, 1, 1, (full - (float)count) / full));

                gridLines.Add(trace.First());

                for (int i = 1; i < trace.Count - 1; i++)
                {
                    gridLines.Add(trace[i]);
                    gridLines.Add(trace[i]);
                }
                gridLines.Add(trace.Last());
            }

            if (gridLines.Count > 0)
                DrawMultiline(gridLines.Select(scale).ToArray(), new Color(1, 1, 1, 0.04f));

            foreach (TraceCoord target in To)
                DrawCoord(target, new Color(0.5f, 1, 0.5f));*/
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (!IsSetup)
                return;

            Lines.RemoveRange(0, (Lines.Count() / 500) * 2);

            Func<Vector2I, Vector2> scale = (v) => (v + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;

            if (!HasResult)
            {
                Vector2[] points = Bake(Queue.FirstOrDefault().From).Select(p => scale(p)).ToArray();

                Lines.Add(points.FirstOrDefault());
                for (int i = 1; i < points.Length - 1; i++)
                {
                    Lines.Add(points[i]);
                    Lines.Add(points[i]);
                }
                Lines.Add(points.LastOrDefault());
            }
            else
            {
                if (Lines.Count == 0)
                    return;

                if (Lines.Count > 0)
                    Lines.RemoveRange(0, 2);
            }
            QueueRedraw();
        }


        float Heuristic(TraceCoord pos)
        {
            float best = float.MaxValue;
            ref TraceNode n = ref this[pos];

            if (Mode == PathMode.Djikstra)
                return 0;

            foreach (TraceCoord t in To)
            {
                Vector2I delta = t.Pos - pos.Pos;
                Vector2I step = pos.D.Offset();

                float distance = Vector2.One.Dot(delta.Abs());
                int absDeltaRotation = pos.D.StepsTo(t.D);

                float distancePenalty = distance;
                int movesToAlign = int.MaxValue;

                if (step.X != 0)
                    movesToAlign = int.Min(movesToAlign, delta.X * step.X);
                if (step.Y != 0)
                    movesToAlign = int.Min(movesToAlign, delta.Y * step.Y);

                float val = 0;

                val += 0.4f *
                    float.Min(
                        float.Max(0, ((Vector2)t.D.Next().Offset()).Normalized().Dot(-delta) + 4),
                        float.Max(0, ((Vector2)t.D.Prev().Offset()).Normalized().Dot(-delta) + 4));

                val += 1.0f * float.Max(0, ((Vector2)t.D.Offset()).Normalized().Dot(-delta) + 5);

                val += 1.0f * distancePenalty;

                if (n.Blocked)
                    val += BlockPenalty;

                best = float.Min(val, best);
            }

            return best;
        }

        void Enqueue(Tip tip)
        {
            if (!InBounds(tip.To))
                return;

            ref TraceNode n = ref this[tip.To];

            if (n.Seen)
                return;

            tip.H = Heuristic(tip.To);

            int index = Queue.BinarySearch(tip);

            if (index < 0) index = ~index;

            Queue.Insert(index, tip);
        }

        public List<Vector2I> Step()
        {
            if (!IsSetup)
                throw new InvalidOperationException("Cannot step before setup");

            if (!HasExit)
                throw new NoPathException("No exits specified");

            QueueRedraw();

            Tip at = Queue.First();
            Queue.RemoveAt(0);
            ref TraceNode n = ref this[at.To];

            n.Back = at.From;
            n.Seen = true;

            if (n.Exit)
            {
                Result = at.To;
                return Bake(Result);
            }

            Enqueue(at.Offset(at.To.D, n.Blocked));
            Enqueue(at.Offset(at.To.D.Next(), n.Blocked));
            Enqueue(at.Offset(at.To.D.Prev(), n.Blocked));

            if (Queue.Count == 0)
                throw new NoPathException("No path");
            
            return null;
        }

        void Setup(Site site)
        {
            Bounds = new Rect2I(0, 0, site.MapWidth, site.MapHeight);
            Map = new TraceNode[site.MapWidth, site.MapHeight, 8];

            for (int x = 0; x < site.MapWidth; x++)
            {
                for (int y = 0; y < site.MapHeight; y++)
                {
                    if (site.Map[x, y] != Site.GridNode.Stable)
                    {
                        foreach (Direction direction in Enum.GetValues<Direction>())
                        {
                            Map[x, y, (int)direction].Blocked = true;
                        }
                    }
                }
            }

            IsSetup = true;
        }
    }
}
