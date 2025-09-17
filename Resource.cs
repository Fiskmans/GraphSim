using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public enum Resource
    {
        [Extractable(1.0f, 1, 1, 0, ["solar"])]
        ElectricCharge,

        [Dumpable]
        Rock,

        [Extractable(4.0f, 0.6f, 0.1f, 0.2f, ["drill"])]
        Soil,

        [Separatable("Flotation separation")]
        Malachite,

        [Separatable("Magnetic separation")]
        Hematite,
        
        [Separatable("Centrifuge")]
        UraniumOre,

        [Decaying(4.0f)]
        WasteHeat,

        Uranium235,
        [Dumpable]
        Uranium238,
        Plutonium239,
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
