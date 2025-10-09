using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphSim
{
    public partial class BuildingInstance : SiteItem
    {
        Data.Building Building;
        string ShortName;
        public int Load = 0;

        readonly Color StatusGood = new Color(0.8f, 1, 0.8f);
        readonly Color StatusLimited = new Color(1.0f, 1, 0.4f);
        readonly Color StatusStopped = new Color(1.0f, 0, 0);

        Color _StatusColor;
        Color StatusColor
        {
            get => _StatusColor;
            set
            {
                _StatusColor = value;
                QueueRedraw();
            }
        }
        Font Font;

        ProgressBar LoadBar = new ProgressBar();

        RichTextLabel ResourceStatus = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };
        RichTextLabel Status = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };

        List<LogisticsEndpoint> Catalysts = new();
        Button AddModuleButton;

        public BuildingInstance(Vector2I gridPosition, Building building) : base(gridPosition)
        {
            Building = building;
            ShortName = building.Name.Substr(0,3);

            if (building.Catalysts != null)
            {
                foreach (var kvPair in building.Catalysts)
                {
                    Catalysts.Add(
                        new LogisticsEndpoint
                        {
                            Resource = kvPair.Key,
                            Capacity = (int)(kvPair.Value * Constants.DataScale),
                            Mode = LogisticsMode.Consumes,
                            Position = new Vector2(35 + 10 * Catalysts.Count, 5)
                        });
                }
            }

            Tooltip = new VBoxContainer();

            Tooltip.AddChild(new Label { Text = building.Name });

            foreach (LogisticsEndpoint catalyst in Catalysts)
            {
                AddChild(catalyst);
                Tooltip.AddChild(new ResourceBar { Node = catalyst });
            }

            foreach (Module module in Building.Modules)
            {
                if (module.Forced)
                    AddModule(module.Instantiate());
            }

            if (Building.ModuleSlots > this.GetChildrenOfType<ModuleInstance>().Count())
            {
                AddModuleButton = new Button { Text = "Add Module" };

                PopupMenu moduleMenu = new PopupMenu();
                this.AddChild(moduleMenu);

                moduleMenu.AddRange(building.Modules, (m) => m.Name, (m) => AddModule(m.Instantiate()));

                AddModuleButton.Pressed += () =>
                {
                    moduleMenu.Position = (Vector2I)(GlobalPosition + GetLocalMousePosition());
                    moduleMenu.Show();
                };

                Tooltip.AddChild(AddModuleButton);
            }
        }

        public override Port GetPort(PortType type)
        {
            foreach (Port port in Building.Ports) // TODO: mark ports as busy
                if (port.Type == type)
                    return port;

            GD.Print("No port");
            return null;
        }

        public override IEnumerable<Rect2I> GetShape()
        {
            return Building.Shape;
        }

        public BuildingInstance AddModule(ModuleInstance module)
        {
            AddChild(module);

            Tooltip.AddChild(new Label { Text = module.BaseModule.Name });
            if (module.UI != null)
                Tooltip.AddChild(module.UI);

            if (AddModuleButton != null && this.GetChildrenOfType<ModuleInstance>().Count() >= Building.ModuleSlots)
                Tooltip.RemoveChild(AddModuleButton);

            return this;
        }

        public override void _Ready()
        {
            if (Font == null)
                Font = ThemeDB.FallbackFont;

            base._Ready();
        }

        private void AddStatus(string reason, int amount)
        {
            if (amount == 0)
            {
                Status.Text = $"Stopped by {reason}\n";
                Load = 0;
            }
            else if (amount < Load)
            {
                Status.Text = $"Limited by {reason}\n";
                Load = amount;
            }
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            ResourceStatus.Text = "";
            Status.Text = "";

            int budget = Constants.SimulationScale;
            if (Catalysts.Count > 0)
                budget = (int)(Constants.SimulationScale * Catalysts.Select(c => c.Fraction).Min());

            if (budget > Constants.SimulationScale)
                budget = Constants.SimulationScale;

            foreach (ModuleInstance module in this.GetChildrenOfType<ModuleInstance>())
                budget -= module.DoWork(budget);
        }

        public override void _Draw()
        {
            base._Draw();
            DrawCircle(new Vector2(Size.X - 5, 5), 4, StatusColor);
            DrawString(Font, new Vector2(2, Size.Y - 3), ShortName);
        }
    }

}
