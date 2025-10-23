using Godot;
using GraphSim.Data;
using GraphSim.Enums;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class ConstructionSite : SiteItem
    {
        Building Building;
        List<ResourceBar> UI = new();
        int PortCounter = 0;

        public override Port GetPort(PortType type)
        {
            int corner = PortCounter++ % 4;

            switch (corner)
            {
            case 0: return new Port { Position = new Vector2I(0, 0), Direction = Direction.NorthWest, Type = PortType.Input };
            case 1: return new Port { Position = new Vector2I(GridSize.X-1, 0), Direction = Direction.NorthEast, Type = PortType.Input };
            case 2: return new Port { Position = new Vector2I(GridSize.X-1, GridSize.Y-1), Direction = Direction.SouthEast, Type = PortType.Input };
            case 3: return new Port { Position = new Vector2I(0, GridSize.Y-1), Direction = Direction.SouthWest, Type = PortType.Input };
            }

            return new Port { Position = new Vector2I(0, 0), Direction = Direction.North, Type = PortType.Input };
        }

        public override IEnumerable<Rect2I> GetShape()
        {
            return Building.Shape;
        }

        public ConstructionSite(Vector2I gridPosition, Building building) : base(gridPosition)
        {
            Building = building;

            Tooltip = new VBoxContainer();

            Tooltip.AddChild(new Label { Text = Building.Name });

            FillColor.A = 0.5f;

            Name = $"{Building.Name}_Construction";
        }

        public override void _Process(double delta)
        {
            if (this.GetChildrenOfType<LogisticsEndpoint>().All(l => l.Full))
            {
                BuildingInstance instance = new BuildingInstance(GridPosition, Building);
                AddSibling(instance);
                instance.Position = Position;
                QueueFree();
            }
        }

        public override void _Ready()
        {
            base._Ready();

            foreach (var kvPair in Building.Cost)
            {
                LogisticsEndpoint supplies = new LogisticsEndpoint
                {
                    Resource = kvPair.Key,
                    Capacity = (int)(kvPair.Value * Constants.DataScale),
                    Mode = LogisticsMode.Consumes
                };
                AddChild(supplies);

                ResourceBar bar = new ResourceBar { Node = supplies };

                bar.Node.OnChange += (a, d) => { QueueRedraw(); };

                UI.Add(bar);
                Tooltip.AddChild(bar);
            }
        }

        public override void _Draw()
        {
            base._Draw();

            int index = 0;

            float width = ((Size.X * 0.5f) - 3.0f) / UI.Count;

            foreach (ResourceBar bar in UI)
            {
                bar.DrawAsInternalArc(this, Size * 0.5f, (index + 0.5f) * width, width - 1.0f);
                index++;
                DrawCircle(Size * 0.5f, index * width, new Color(1, 1, 1, 0.4f), filled: false, antialiased: true, width: 0.4f);
            }

            //foreach(Port port in Building.Ports)
            //    port.Draw(this);
        }
    }
}
