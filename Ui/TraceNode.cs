using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class TraceNode : Node2D
    {
        List<Trace> Traces = new();
        internal Vector2I GridPosition;
        internal Resource Resource;

        internal TraceNode(Vector2I gridPosition, Resource resource)
        {
            GridPosition = gridPosition;
            Resource = resource;
        }

        public override void _Draw()
        {
            DrawCircle(GridPosition.ScaledToGrid(), 1.5f, Resource.Color().OnTrace(), antialiased: true);
        }

        internal void Detach()
        {
            Traces.ForEach(t => t.Detach(this));
        }

        public void AttachTrace(Trace trace)
        {
            Traces.Add(trace);

            trace.TreeExiting += () => Traces.Remove(trace);

        }
    }

    public partial class EndpointTraceNode : TraceNode
    {
        public Direction Direction;
        public EndpointTraceNode(LogisticsEndpoint attachedTo) : base(attachedTo.Entry.Pos, attachedTo.Resource)
        {
            attachedTo.TreeExiting += Detach;
            Direction = attachedTo.Entry.D;
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            foreach (TraceNode sibling in this.GetSiblings<TraceNode>())
            {
                AddSibling(new Trace(this, Resource));
            }
        }
        public override void _Draw()
        {
            DrawLine(GridPosition.ScaledToGrid(), (GridPosition + Direction.Offset()).ScaledToGrid(), Resource.Color().OnTrace());
        }
    }
}
