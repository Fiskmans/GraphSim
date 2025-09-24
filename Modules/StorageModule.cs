using GraphSim;
using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class StorageModule : Module
{
    [JsonPropertyName("Stores")]
    public Dictionary<GraphSim.Resource, int> Stores { get; set; }

    public override ModuleInstance Instantiate()
    {
        return new StorageModuleInstance(this);
    }
}
