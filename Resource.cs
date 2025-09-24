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
        Soil,

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

        GraphiteCathode,

        ReactorFuelRod,
        DepletedReactorFuelRod,

        HighRadioactiveWaste,

        Machinery
    }
}
