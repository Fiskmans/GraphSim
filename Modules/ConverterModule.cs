using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class ConverterModule : Module
{
    [JsonPropertyName("Input")]
    public Dictionary<GraphSim.Resource, int> Input { get; set; }

    [JsonPropertyName("Output")]
    public Dictionary<GraphSim.Resource, int> Output { get; set; }

    public override GraphSim.ModuleInstance Instantiate()
    {
        return new GraphSim.ConverterModuleInstance(this);
    }
}
