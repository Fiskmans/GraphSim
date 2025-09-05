using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    internal class ExtractableAttribute : System.Attribute
    {
        public float Yield;
        public string[] ExtractorTags;
        public Color Color;

        public ExtractableAttribute(float yield, float r, float g, float b, string[] extractorTags)
        {
            Yield = yield;
            ExtractorTags = extractorTags;
            Color = new Color(r,g,b);
        }
    }
}
