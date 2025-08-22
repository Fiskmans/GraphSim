using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim
{
    
    public class Conversion
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        public Dictionary<string, float> Amounts { get; set; }
    }
}
