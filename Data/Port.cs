using Godot;
using GraphSim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{

    public enum PortType
    {
        Input,
        Output,
        InOut
    }

    public class Port
    {
        public Vector2I Position { get; set; }

        public Direction Direction { get; set; }

        public PortType Type { get; set; }

        public TraceFinder.TraceCoord Out(Vector2I relativeTo)
        {
            return new TraceFinder.TraceCoord
            {
                Pos = Position + relativeTo + Direction.Offset(),
                D = Direction
            };
        }

        public TraceFinder.TraceCoord In(Vector2I relativeTo)
        {
            return new TraceFinder.TraceCoord
            {
                Pos = Position + relativeTo + Direction.Offset(),
                D = Direction.Reversed()
            };
        }

        public void Draw(Node2D onto)
        {
            const float size = 2.1f;
            Vector2 p = ((Vector2)Position + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;
            Vector2 d = Direction.Offset();

            onto.DrawCircle(p, size, new Color(1, 1, 1), width: 0.5f, filled: false, antialiased: true);
            onto.DrawLine(p + d.Normalized() * size, p + d * Constants.NodeSpacing, new Color(1, 1, 1));
        }

        public void Draw(Control onto)
        {
            const float size = 2.1f;
            Vector2 p = ((Vector2)Position + new Vector2(0.5f, 0.5f)) * Constants.NodeSpacing;
            Vector2 d = Direction.Offset();

            onto.DrawCircle(p, size, new Color(1, 1, 1), width: 0.5f, filled: false, antialiased: true);
            onto.DrawLine(p + d.Normalized() * size, p + d * Constants.NodeSpacing, new Color(1, 1, 1));
        }
    }
}
