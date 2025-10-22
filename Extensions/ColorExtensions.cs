using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class ColorExtensions
    {
        public static Color OnTrace(this Color self)
        {
            return self.Lerp(new Color(1, 1, 1), 0.3f);
        }
        
        public static Color Alpha(this Color self, float alpha)
        {
            Color c = self;
            c.A = alpha;
            return c;
        }


    }
}
