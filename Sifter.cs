using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class Sifter : SlotItem
    {
        Resource Resource;
        Container Container;
        Area Area;
        Catalyst Soil = new Catalyst(Resource.Soil, 100);

        public Sifter(Container container, Resource resource)
        {
            Container = container;
            Resource = resource;

            Tooltip = new VBoxContainer();

            Tooltip.AddChild(new Label { Text = $"{resource.ToString()} sifter" });
            Tooltip.AddChild(Soil);
        }

        public override void _Ready()
        {
            Area = this.GetFirstParentOfType<Area>();
        }

        public override void _Process(double delta)
        {
            Soil.Refill(Container);

            if (delta > 1.0f)
                delta = 1.0f;

            float make = (float)delta * Area.FractionOf(Resource);

            float load = 1.0f;
            float space = Container.CapacityFor(Resource);
            if (make > space)
                load = space / make;

            Soil.Consume((float)delta * Soil.Fraction * load);
            Container.Add(Resource, make * load);
            Area.Remove(Resource, make * load);
        }

    }
}
