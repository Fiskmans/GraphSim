using Godot;
using GraphSim.Data;
using GraphSim.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class ConverterModuleInstance : ModuleInstance
    {
        ConverterModule Module;

        public ConverterModuleInstance(ConverterModule module) : base(module)
        {
            Module = module;

            SetupInput(Module.Input);
            SetupBasicOutput(Module.Output);
        }

        public override int DoWork(int budget)
        {
            int cycles = int.Min(budget, GetProductionCapacity());


            ConsumeInputs(cycles);

            foreach (LogisticsEndpoint output in BasicOutputs.Values)
                output.Deposit(cycles * PerCycle(output));

            return cycles;
        }
    }
}
