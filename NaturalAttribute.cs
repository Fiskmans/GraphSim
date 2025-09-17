using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public class NaturalAttribute : Attribute
    {
        float Concentration;

        public NaturalAttribute(float concentration)
        {
            Concentration = concentration;
        }
    }
}
