using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Modules
{
    public partial class DropperModuleInstance : ModuleInstance
    {
        DropperModule Module;
        public DropperModuleInstance(DropperModule module) : base(module)
        {
            Module = module;
        }

        public override int DoWork(int budget)
        {


            return base.DoWork(budget);
        }
    }
}
