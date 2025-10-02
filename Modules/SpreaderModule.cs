using GraphSim;
using GraphSim.Data;
using GraphSim.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class SpreaderModule : Module
{
    [JsonPropertyName("Inputs")]
    public Dictionary<Resource, int> Inputs { get; set; }

    [JsonPropertyName("Dumps")]
    public Dictionary<Resource, int> Dumps { get; set; }

    public override ModuleInstance Instantiate()
    {
        return new SpreaderModuleInstance(this);
    }
}
