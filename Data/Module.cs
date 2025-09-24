using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    public class Module : TypedJsonObject
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Forced")]
        public bool Forced { get; set; } = false;

        public virtual ModuleInstance Instantiate()
        {
            GD.PrintErr("Instantiating a null module");
            return new ModuleInstance(this);
        }
    }
}
