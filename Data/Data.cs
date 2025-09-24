using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    public class Data
    {
        [JsonPropertyName("Buildings")]
        public List<Building> Buildings { get; set; }
    }
}
