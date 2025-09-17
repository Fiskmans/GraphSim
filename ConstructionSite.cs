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
        Dictionary<GraphSim.Resource, ResourceBar> progress = new();
        Container Source;

        public ConstructionSite(string name, Container source, Dictionary<GraphSim.Resource, float> costs, Func<SlotItem> onBuild)
        {
            Tooltip = new VBoxContainer();

            Source = source;

            Tooltip.AddChild(new Label { Text = name });

            foreach (var kvPair in costs)
            {
                ResourceBar bar = new ResourceBar
                {
                    Label = kvPair.Key.ToString(),
                    Max = kvPair.Value
                };

                progress.Add(kvPair.Key, bar);
                Tooltip.AddChild(bar);
            }
            OnBuild = onBuild;
        }

        public override void _Process(double delta)
        {
            bool done = true;
            foreach (var kvPair in progress)
            {
                if (kvPair.Value.Value < kvPair.Value.Max)
                {
                    float available = Source.AmountOf(kvPair.Key);
                    float wanted = kvPair.Value.Missing;

                    if (available > wanted)
                    {
                        Source.Remove(kvPair.Key, wanted);
                        kvPair.Value.Value = kvPair.Value.Max;
                        QueueRedraw();
                    }
                    else
                    {
                        done = false;
                        if (available > 0)
                        {
                            kvPair.Value.Value += available;
                            Source.Remove(kvPair.Key, available);
                            QueueRedraw();
                        }
                    }
                }
            }

            if (done)
            {
                this.GetFirstParentOfType<WorldSlot>().Content = OnBuild();
                QueueFree();
            }
        }

        public override void _Draw()
        {
            int index = 0;

            float width = ((Size.X * 0.5f) - 3.0f) / progress.Count;

            foreach (var kvPair in progress)
            {
                kvPair.Value.DrawAsInternalArc(this, Size * 0.5f, (index + 0.5f) * width, width - 1.0f);
                index++;
                DrawCircle(Size * 0.5f, index * width, new Color(1, 1, 1, 0.4f), filled: false, antialiased: true, width: 0.5f);
            }
        }
    }
}
