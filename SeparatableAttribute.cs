using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public class SeparatableAttribute : Attribute
    {
        string Research;

        public SeparatableAttribute(string research)
        {
            Research = research;
        }
    }
}
