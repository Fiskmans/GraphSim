using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class Trace : Node2D, IMapModifier
    {
        Site Site;
        TraceFinder Finder;

        public List<Vector2I> Path = null;
        Vector2[] Points;
        Resource Resource;
        float Alpha = 0.0f;
        bool SplitOnFound = false;

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

        public void Detach(TraceNode aNode)
        {
            if (ReferenceEquals(From, aNode))
                From = null;

            if (ReferenceEquals(To, aNode))
                To = null;
        }

        public Trace(TraceNode from, Resource resource)
        {
            From = from;
            Resource = resource;
        }

        public Trace(TraceNode from, TraceNode to, Resource resource, IEnumerable<Vector2I> path)
        {
            From = from;
            To = to;
            Resource = resource;
            SetPath([.. path]);
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
                    SetupTraceFinder();

                int budget = 100;
                while (Path == null)
                {
                    SetPath(Finder.Step());

                    if (budget-- < 0)
                        return null;
                }
            }

            return Path?.Select(p => new Rect2I { Position = p, Size = Vector2I.One });
        }

        private void SetupTraceFinder()
        {
            Finder = new TraceFinder(this.GetFirstParentOfType<Site>(), Resource);
            AddChild(Finder);

            Finder.AddEntrypoint(From);

            if (To != null)
                Finder.AddExit(To);

            foreach (Trace sibling in this.GetSiblings<Trace>())
            {
                if (sibling.Path == null)
                    continue;

                Finder.AddExits(sibling.Path);
                SplitOnFound = true;
            }

            if (!Finder.HasExit)
            {
                foreach (TraceNode node in this.GetSiblings<TraceNode>())
                {
                    if (object.ReferenceEquals(node, From))
                        continue;

                    Finder.AddExit(node);
                }
            }
        }

        public void SetPath(List<Vector2I> path)
        {
            Path = path;
            if (Path == null)
                return;

            Finder = null;

            if (path.Count == 0)
                return;

            if (SplitOnFound)
                AddSibling(new TraceNode(Path.First(), Resource));

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

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (Path != null && Alpha < 1.0f)
            {
                Alpha += (float)delta;
                QueueRedraw();
            }
        }

        public override void _Draw()
        {
            base._Draw();
            if (Points != null)
                DrawMultiline(Points, Resource.Color().OnTrace().Alpha(Alpha));
        }
    }
}
