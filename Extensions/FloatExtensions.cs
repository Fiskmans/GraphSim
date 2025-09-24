using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Extensions
{
    public static class FloatExtensions
    {
        public static double Fract(this double self)
        {
            return self - Math.Truncate(self);
        }
        public static float Fract(this float self)
        {
            return self - MathF.Truncate(self);
        }
    }
}
