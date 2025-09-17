using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class SlotItem : Control
    {
        private PanelContainer Popup = new PanelContainer
        {
            Visible = false
        };

        public Control _Tooltip;
        public Control Tooltip {
            get => _Tooltip;
            set {
                if (_Tooltip != null)
                    Popup.RemoveChild(_Tooltip);

                Popup.AddChild(value);
                _Tooltip = value;
            }
        }

        public bool Selected = false;

        public SlotItem()
        {
            Size = new Vector2(50, 50);
            Popup.Position = Size;
            Popup.ZIndex = 1;
            AddChild(Popup);

            MouseEntered += () =>
            {
                foreach (Node sibling in GetParent().GetChildren())
                {
                    if (sibling is SlotItem)
                    {
                        (sibling as SlotItem).Popup.Visible = (sibling as SlotItem).Selected;
                    }
                }

                Popup.ResetSize();
                Popup.Visible = true;
            };
            MouseExited += () => {
                if (!Selected)
                {
                    Popup.Visible = false;
                }
            };
        }

        public override void _Draw()
        {
            DrawCircle(Size * 0.5f, Size.X * 0.25f, new Color(1, 1, 1));
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                {
                    Selected = !Selected;
                    Popup.Visible = Selected;
                }
            }
        }
    }
}
