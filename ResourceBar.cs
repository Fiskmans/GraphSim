using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class ResourceBar : Control
    {
        LogisticsEndpoint _Node;
        public LogisticsEndpoint Node
        {
            get => _Node;
            set 
            {
                if (_Node != null)
                {
                    GD.PrintErr("Resource bar already has a node set");
                    QueueFree();
                    return;
                }
                _Node = value;
                _Node.OnChange += (value, delta) => QueueRedraw();
            }
        }

        public Color BarColor;
        public Color BorderColor;

        public Font Font;

        public ResourceBar()
        {            
            BarColor = new Color(0.2f,0.4f,0.2f);
            BorderColor = new Color(1,1,1);
        }

        public override void _Ready()
        {
            if (Font == null)
                Font = ThemeDB.FallbackFont;

            if (Node == null)
            {
                GD.PrintErr("You need to set a node for resource bar before its added");
                QueueFree();
                return;
            }

            UpdateMinimumSize();
        }

        public override Vector2 _GetMinimumSize()
        {
            return
                Font?.GetStringSize($"{Node.Resource.ToString()} 00000.00/{Node.Capacity}") ?? new Vector2(0,0) + 
                new Vector2(3, 4);
        }


        public override void _Draw()
        {
            DrawRect(new Rect2(1, 1, Size.X * Node.Fraction, Size.Y), BarColor, true);
            DrawRect(new Rect2(1, 1, Size.X, Size.Y), BorderColor, false);

            DrawString(Font, new Vector2(3, Size.Y - 4), Node.Resource.ToString(), HorizontalAlignment.Left, Size.X);
            DrawString(Font, new Vector2(0, Size.Y - 4), $"{Node.Amount:0.00}/{Node.Capacity}", HorizontalAlignment.Right, Size.X - 2);
        }


        public void DrawAsInternalArc(Control onto, Vector2 center, float radius, float width)
        {
            onto.DrawArc(center, radius, 0, Node.Fraction * float.Tau, 30, BarColor, width);
        }
    }
}
