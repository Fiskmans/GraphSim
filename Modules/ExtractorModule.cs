using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class ExtractorModule : Module
{
    [JsonPropertyName("Input")]
    public Dictionary<GraphSim.Resource, int> Input { get; set; }

    [JsonPropertyName("Resource")]
    public GraphSim.Resource Resource { get; set; }

    [JsonPropertyName("Speed")]
    public int Speed { get; set; } = 1;

    public override GraphSim.ModuleInstance Instantiate()
    {
        return new ExtractorModuleInstance(this);
    }
}
