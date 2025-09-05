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
        [Extractable(30.0f, 1, 1, 0, ["solar"])]
        ElectricCharge,

        [Extractable(0.1f, 0.1f,0.8f, 0.1f, ["drill"])]
        Uraninite,
        EnrichedUranium,
        DepletedFuel,

        [Extractable(1, 0.8f, 0.1f, 0.9f, ["gas_capture"])]
        XenonGas,

        [Extractable(1, 0.2f, 0.2f, 0.25f, ["drill"])]
        Ore,
        LiquidFuel,
        Oxidizer,
        MonoPropellant,

        [Extractable(1, 0.05f, 0.1f, 0.2f, ["drill"])]
        Minerals,
        
        [Extractable(1, 0.3f, 0.1f, 0.02f, ["drill"])]
        Dirt,
        
        [Extractable(1, 0.1f, 0.1f, 0.2f, ["drill"])]
        Rock,
        
        [Extractable(1, 0.1f, 0.2f, 0.3f, ["drill"])]
        MetallicOre,

        [Extractable(1, 0.3f, 0.1f, 0.1f, ["drill"])]
        Substrate,

        [Extractable(1, 0.6f, 0.6f, 0.6f, ["drill"])]
        Gypsum,

        [Extractable(1, 0.7f, 0.7f, 0.7f, ["drill"])]
        Silicates,

        [Extractable(1, 0.1f, 0.1f, 0.5f, ["drill"])]
        Hydrates,

        [Extractable(1, 0.7f, 0.7f, 0.1f, ["drill"])]
        RareMetals,

        [Extractable(1, 0.1f, 0.6f, 0.6f, ["drill"])]
        ExoticMinerals,

        Water,
        Fertilizer,
        Mulch,
        Supplies,
        Organics,
        ColonySupplies,

        Alloys,
        Chemicals,
        Metals,
        Polymers,
        Silicon,
        RefinedExotics,

        MaterialKits,
        Machinery,
        Recyclables,
        SpecializedParts,
        Synthetics,
        Electronics,
        Robotics,
        Prototypes,

        TransportCredits,
    }
}
