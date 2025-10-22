using Godot;
using GraphSim.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public enum Resource
    {
        ElectricCharge,
        Regolith,

        Rock,
        Malachite,
        Hematite,
        UraniumOre,

        WasteHeat,

        Uranium235,
        Uranium238,
        Plutonium239,
        [Decaying(5.0f, DecayingAttribute.TimeScale.Hours)]
        Plutonium238,

        Slag,
        Iron,
        Copper,
        Carbon,
        Graphite,

        GraphiteCathode,

        ReactorFuelRod,
        DepletedReactorFuelRod,

        HighRadioactiveWaste,

        Machinery
    }

    public static class ResourceExtension
    {
        static readonly Color Metal = new Color(0.9f, 0.9f, 0.9f);

        public static Color Color(this Resource self)
        {
            switch (self)
            {
                case Resource.ElectricCharge:
                    break;
                case Resource.Regolith:
                    break;
                case Resource.Rock:
                    break;
                case Resource.Malachite:
                    break;
                case Resource.Hematite:
                    break;
                case Resource.UraniumOre:
                    break;
                case Resource.WasteHeat:
                    break;
                case Resource.Uranium235:
                    break;
                case Resource.Uranium238:
                    break;
                case Resource.Plutonium239:
                    break;
                case Resource.Plutonium238:
                    break;
                case Resource.Slag:
                    break;
                case Resource.Iron:     return Metal.Lerp(new Color(0.5f, 0.5f, 0.8f), 0.8f);
                case Resource.Copper:   return Metal.Lerp(new Color(0.8f, 0.5f, 0.5f), 0.8f);
                case Resource.Carbon:   return Metal.Lerp(new Color(0.5f, 0.5f, 0.5f), 0.8f);
                case Resource.Graphite:
                    break;
                case Resource.GraphiteCathode:
                    break;
                case Resource.ReactorFuelRod:
                    break;
                case Resource.DepletedReactorFuelRod:
                    break;
                case Resource.HighRadioactiveWaste:
                    break;
                case Resource.Machinery:
                    break;
            }

            return new Color(1, 0, 0);
        }
    }
}
