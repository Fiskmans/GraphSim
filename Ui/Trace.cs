using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class Trace : Node2D, IMapModifier
    {
        TraceFinder Finder;

        public List<Vector2I> Path;
        Vector2[] Points;
        Color c = new Color(GD.Randf(), GD.Randf(), GD.Randf());
        float Alpha = 0.0f;
        bool Unraveling = false;

        TraceNode _From;
        TraceNode From
        {
            get => _From;
            set
            {
                _From = value;
                if (_From != null)
                    _From.AttachTrace(this);
            }
        }
        TraceNode _To;
        TraceNode To
        {
            get => _To;
            set
            {
                _To = value;
                if (_To != null)
                    _To.AttachTrace(this);
            }
        }

        private void Unravel()
        {
            Unraveling = true;
            Path = null;
        }

        public void Detach(TraceNode aNode)
        {
            if (ReferenceEquals(From, aNode))
                From = null;

            if (ReferenceEquals(To, aNode))
                To = null;
        }

        public Trace(TraceNode from)
        {
            From = from;
            Name = $"Trace_{from.GridPosition.X}_{from.GridPosition.Y}";
        }

        public Trace(TraceNode from, TraceNode to, IEnumerable<Vector2I> path)
        {
            From = from;
            To = to;
            Path = [.. path];
            Name = $"Subtrace_{from.GridPosition.X}_{from.GridPosition.Y}";
            BakePoints();
        }

        public override void _Ready()
        {
            base._Ready();
            this.GetFirstParentOfType<Site>().AddModification(this);
        }

        public IEnumerable<Rect2I> GetBlockedRegions()
        {
            if (Path == null)
            {
                if (Finder == null)
                    if (!SetupTraceFinder())
                        return null;

                int budget = 30;
                while (Path == null)
                {
                    TrySetPath(Finder.Step());

                    if (budget-- < 0)
                        return null;
                }
            }

            return Path?.Select(p => new Rect2I { Position = p, Size = Vector2I.One });
        }

        private bool SetupTraceFinder()
        {
            if (Path != null)
                throw new Exception();

            Finder = new TraceFinder(this.GetFirstParentOfType<Site>());

            Finder.AddEntrypoint(From);

            if (To != null)
                Finder.AddExit(To);

            foreach (Trace sibling in this.GetSiblings<Trace>())
            {
                if (sibling.Path == null)
                    continue;

                Finder.AddExits(sibling.Path);
            }

            if (!Finder.HasExit)
            {
                foreach (TraceNode node in this.GetSiblings<TraceNode>())
                {
                    if (object.ReferenceEquals(node, From))
                        continue;

                    Finder.AddExit(node);
                    break;
                }
            }

            if (!Finder.HasExit)
            {
                Finder = null;
                this.GetFirstParentOfType<Site>().RemoveModification(this);
                QueueFree();
                return false;
            }

            AddChild(Finder);

            return true;
        }

        public void TrySetPath(List<Vector2I> path)
        {
            if (path == null)
                return;

            Path = path;
            BakePoints();

            RemoveChild(Finder);
            Finder = null;

            Connect();
        }

        private void BakePoints()
        {
            Points = null;

            if (Path == null)
                return;

            if (Path.Count == 0)
                return;

            List<Vector2> res = [Path.First().ScaledToGrid()];

            for (int i = 1; i < Path.Count - 1; i++)
            {
                res.Add(Path[i].ScaledToGrid());
                res.Add(Path[i].ScaledToGrid());
            }
            res.Add(Path.Last().ScaledToGrid());

            Points = res.ToArray();

            QueueRedraw();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (Path != null && Alpha < 1.0f)
            {
                Alpha += (float)delta;
                QueueRedraw();
            }
            if (Unraveling)
            {
                if (Alpha > 0.0f)
                {
                    Alpha -= (float)delta;
                    QueueRedraw();
                }
                else
                {
                    QueueFree();
                }
            }
        }

        private void Connect()
        {
            Vector2I point = Path.First();

            foreach (TraceNode present in this.GetSiblings<TraceNode>())
                if (present.GridPosition.Equals(point))
                    return;

            To = new TraceNode(point);
            AddSibling(To);

            foreach (Trace sibling in this.GetSiblings<Trace>())
            {
                if (sibling.Path == null)
                    continue;

                int index = sibling.Path.IndexOf(point);

                if (index == -1)
                    continue;

                if (index == 0)
                    return;

                if (index == sibling.Path.Count - 1)
                    return;

                AddSibling(new Trace(sibling.From, To, sibling.Path.Take(index + 1)));
                AddSibling(new Trace(To, sibling.To, sibling.Path.Skip(index)));

                sibling.Unravel();
                break;
            }
        }
        public override void _Draw()
        {
            if (Points != null)
                DrawMultiline(Points, new Color(1,1,1).Alpha(Alpha));
        }
    }
}
