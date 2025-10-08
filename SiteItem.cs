using Godot;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public abstract partial class SiteItem : Control
    {
        TooltipRegion TooltipRegion;
        public Control Tooltip;
        [Export]
        public Vector2I GridPosition;

        bool _Hovered = false;
        bool Hovered
        {
            get => _Hovered;
            set
            {
                if (_Hovered != value)
                {
                    if (value)
                        TooltipRegion.Show(Tooltip, this);
                    else
                        TooltipRegion.Close(Tooltip);

                    _Hovered = value;
                }
            }
        }

        bool _ShowingTooltip;
        public bool ShowingTooltip
        {
            get => _ShowingTooltip;
            set
            {
                if (_ShowingTooltip != value)
                    QueueRedraw();
                _ShowingTooltip = value;
            }
        }

        public Color FillColor = new Color(0.2f, 0.2f, 0.2f);

        public abstract IEnumerable<Rect2I> GetShape();
        public IEnumerable<Rect2> WorldShape()
        {
            return GetShape().Select(r => new Rect2((Vector2)r.Position * Constants.NodeSpacing, (Vector2)r.Size * Constants.NodeSpacing));
        }

        public SiteItem(Vector2I gridPosition)
        {
            GridPosition = gridPosition;
            Position = (Vector2)GridPosition * Constants.NodeSpacing;
        }

        public override void _Ready()
        {
            Site site = this.GetFirstParentOfType<Site>();
            TooltipRegion = this.GetFirstParentOfType<TooltipRegion>();

            int w = 0;
            int h = 0;

            foreach (Rect2I rect in GetShape())
            {
                w = int.Max(w, rect.Position.X + rect.Size.X);
                h = int.Max(h, rect.Position.Y + rect.Size.Y);

                site.Block(GridPosition, rect);
            }

            Size = new Vector2(w * Constants.NodeSpacing, h * Constants.NodeSpacing);

            MouseExited += () => Hovered = false;
        }

        public override void _Draw()
        {
            foreach (Rect2 rect in WorldShape())
                DrawRect(rect, FillColor);

            if (ShowingTooltip)
                foreach (Rect2 rect in WorldShape())
                    DrawRect(rect, new Color(1, 1, 1), filled: false);
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (TooltipRegion == null)
                return;

            if (@event is InputEventMouseMotion mouse)
                Hovered = WorldShape().Any((r) => r.Contains(mouse.Position));

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
