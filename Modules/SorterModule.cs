using GraphSim;
using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public partial class SorterModule : Module
{
    [JsonPropertyName("Consumes")]
    public Dictionary<Resource, int> Consumes { get; set; }

    [JsonPropertyName("Accepts")]
    public List<Resource> Accepts { get; set; }

    public override ModuleInstance Instantiate()
    {
        return new SorterModuleInstance(this);
    }
}
