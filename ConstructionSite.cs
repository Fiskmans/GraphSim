using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class ConstructionSite : SlotItem
    {
        Func<SlotItem> OnBuild;
        List<ResourceBar> UI = new();

        public ConstructionSite(string name, Dictionary<GraphSim.Resource, float> costs, Func<SlotItem> onBuild)
        {
            Tooltip = new VBoxContainer();

            Tooltip.AddChild(new Label { Text = name });

            foreach (var kvPair in costs)
            {
                LogisticsEndpoint supplies = new LogisticsEndpoint
                {
                    Resource = kvPair.Key,
                    Capacity = kvPair.Value,
                    Mode = LogisticsMode.Consumes
                };
                AddChild(supplies);

                ResourceBar bar = new ResourceBar { Node = supplies };

                bar.Node.OnChange += (a,d) => { QueueRedraw(); };

                UI.Add(bar);
                Tooltip.AddChild(bar);
            }
            OnBuild = onBuild;
        }

        public override void _Process(double delta)
        {
            if (this.GetChildrenOfType<LogisticsEndpoint>().All(l => l.Full))
            {
                this.GetFirstParentOfType<WorldSlot>().Content = OnBuild();
                QueueFree();
            }
        }

        public override void _Draw()
        {
            int index = 0;

            float width = ((Size.X * 0.5f) - 3.0f) / UI.Count;

            foreach (ResourceBar bar in UI)
            {
                bar.DrawAsInternalArc(this, Size * 0.5f, (index + 0.5f) * width, width - 1.0f);
                index++;
                DrawCircle(Size * 0.5f, index * width, new Color(1, 1, 1, 0.4f), filled: false, antialiased: true, width: 0.4f);
            }
        }
    }
}
