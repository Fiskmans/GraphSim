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
        string _Label = "";
        public string Label
        {
            get => _Label;
            set {
                _Label = value;

                QueueRedraw();
                UpdateMinimumSize();
            }
        }

        float _Value;
        public float Value
        {
            get => _Value;
            set
            {
                _Value = value;
                QueueRedraw();
            }
        }

        public float Max;
        public float Fraction
        {
            get
            {
                if (Max < 0.000001f)
                    return 0;

                return Value / Max;
            }
        }

        public float Missing { get => Max - Value; }

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

            UpdateMinimumSize();
        }

        public override Vector2 _GetMinimumSize()
        {
            return
                Font?.GetStringSize($"{Label} 000.00/{Max}") ?? new Vector2(0,0) + 
                new Vector2(3, 4);
        }


        public override void _Draw()
        {
            DrawRect(new Rect2(1, 1, Size.X * Fraction, Size.Y), BarColor, true);
            DrawRect(new Rect2(1, 1, Size.X, Size.Y), BorderColor, false);

            DrawString(Font, new Vector2(3, Size.Y - 4), Label, HorizontalAlignment.Left, Size.X);
            DrawString(Font, new Vector2(0, Size.Y - 4), $"{Value:0.00}/{Max}", HorizontalAlignment.Right, Size.X - 2);
        }


        public void DrawAsInternalArc(Control onto, Vector2 center, float radius, float width)
        {
            onto.DrawArc(center, radius, 0, Fraction * float.Tau, 30, BarColor, width);
        }

    }
}
