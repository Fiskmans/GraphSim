using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim
{
    public class Building
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Conversions")]
        public List<Conversion> Conversions { get; set; }

        [JsonPropertyName("Catalysts")]
        public Dictionary<GraphSim.Resource, float> Catalysts { get; set; }

        [JsonPropertyName("Cost")]
        public Dictionary<GraphSim.Resource, float> Cost { get; set; }
    }
}
