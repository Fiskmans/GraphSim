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

        internal TraceNode(Vector2I gridPosition)
        {
            GridPosition = gridPosition;
            Name = $"TraceNode_{gridPosition.X}_{GridPosition.Y}";
        }

        public override void _Draw()
        {
            DrawCircle(GridPosition.ScaledToGrid(), 1f, new Color(1,1,1), antialiased: true);
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
        public EndpointTraceNode(LogisticsEndpoint attachedTo) : base(attachedTo.Entry.Pos)
        {
            attachedTo.TreeExiting += Detach;
            Direction = attachedTo.Entry.D;
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            AddSibling(new Trace(this));
        }
        public override void _Draw()
        {
            DrawCircle(GridPosition.ScaledToGrid(), 2f, new Color(1, 1, 1), filled: false, width:0.5f, antialiased: true);
            DrawLine(GridPosition.ScaledToGrid() + ((Vector2)Direction.Offset()).Normalized() * 2f, (GridPosition + Direction.Offset()).ScaledToGrid(), new Color(1,1,1));
        }
    }
}
