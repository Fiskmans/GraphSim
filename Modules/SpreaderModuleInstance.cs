using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Modules
{
    public partial class SpreaderModuleInstance : ModuleInstance
    {
        SpreaderModule Module;
        Site Site;
        public SpreaderModuleInstance(SpreaderModule module) : base(module)
        {
            Module = module;

            SetupInput(Module.Inputs);
            foreach (var kvPair in Module.Dumps)
                AddInput(kvPair);
        }

        public override void _Ready()
        {
            Site = this.GetFirstParentOfType<Site>();
        }

        public override int DoWork(int budget)
        {
            int cycles = int.Min(budget, GetProductionCapacity());

            ConsumeInputs(cycles);

            foreach (var kvPair in Module.Dumps)
                Site.Dump(kvPair.Key, kvPair.Value * cycles);

            return cycles;
        }
    }
}
