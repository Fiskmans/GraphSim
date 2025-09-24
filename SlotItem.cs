using Godot;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class SlotItem : Control
    {
        TooltipRegion TooltipRegion;
        public Control Tooltip;

        bool _ShowingTooltip;
        public bool ShowingTooltip
        {
            get => _ShowingTooltip;
            set
            {
                if(_ShowingTooltip != value)
                    QueueRedraw();
                _ShowingTooltip = value;
            }
        }

        public SlotItem()
        {
            Size = new Vector2(50, 50);
        }

        public override void _Ready()
        {
            TooltipRegion = this.GetFirstParentOfType<TooltipRegion>();

            MouseEntered += () =>
            {
                TooltipRegion.Show(Tooltip,this);
            };

            MouseExited += () =>
            {
                TooltipRegion.Close(Tooltip);
            };
        }

        public override void _Draw()
        {
            DrawBorder();
            DrawCircle(Size * 0.5f, Size.X * 0.25f, new Color(1, 1, 1));
        }

        public void DrawBorder()
        {
            if (ShowingTooltip)
                DrawRect(new Rect2(new Vector2(1,1), Size), new Color(1,1,1), filled: false);
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (TooltipRegion == null)
                return;

            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                {
                    TooltipRegion.ToggleLock(Tooltip, this);
                }
            }
        }
    }
}
