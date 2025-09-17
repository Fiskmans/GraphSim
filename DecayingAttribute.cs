using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    internal class DecayingAttribute : Attribute
    {
        public float HalfLife;
        public float DecayConstant;

        public DecayingAttribute(float halfLife) 
        {
            HalfLife = halfLife;
            DecayConstant = MathF.Log(2.0f) / HalfLife;
        }

        public float DecayAfter(float aDelta)
        {
            return MathF.Pow(float.E, -aDelta * DecayConstant);
        }
    }
}
