using GraphSim;
using GraphSim.Data;
using GraphSim.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class DropperModule : Module
{
    [JsonPropertyName("CostPer")]
    Dictionary<Resource, int> CostPer { get; set; }

    [JsonPropertyName("Drops")]
    Dictionary<Resource, int> Drops { get; set; }

    public override ModuleInstance Instantiate()
    {
        return new DropperModuleInstance(this);
    }
}
