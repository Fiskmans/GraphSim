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
    public abstract partial class SiteItem : Control, IMapModifier
    {
        TooltipRegion TooltipRegion;
        public Control Tooltip;
        [Export]
        public Vector2I GridPosition;
        public Vector2I GridSize;

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
        public abstract Port GetPort(PortType type);

        public IEnumerable<Rect2> WorldShape()
        {
            return GetShape().Select(r => new Rect2((Vector2)r.Position * Constants.NodeSpacing, (Vector2)r.Size * Constants.NodeSpacing));
        }

        public IEnumerable<Rect2I> GetBlockedRegions()
        {
            return GetShape().Select(r => new Rect2I { Position = r.Position + GridPosition, Size = r.Size });
        }

        public SiteItem(Vector2I gridPosition)
        {
            GridPosition = gridPosition;
            Position = (Vector2)GridPosition * Constants.NodeSpacing;
        }

        public override void _Ready()
        {
            TooltipRegion = this.GetFirstParentOfType<TooltipRegion>();

            foreach (Rect2I rect in GetShape())
                GridSize = GridSize.Max(rect.Position + rect.Size);

            Size = (Vector2)GridSize * Constants.NodeSpacing;

            MouseExited += () => Hovered = false;

            this.GetFirstParentOfType<Site>().AddModification(this);
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
