using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    public class Building
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Catalysts")]
        public Dictionary<Resource, float> Catalysts { get; set; }

        [JsonPropertyName("Cost")]
        public Dictionary<Resource, float> Cost { get; set; }

        [JsonPropertyName("ModuleSlots")]
        public int ModuleSlots { get; set; } = 1;

        [JsonPropertyName("Modules")]
        public List<Module> Modules { get; set; }
    }
}
