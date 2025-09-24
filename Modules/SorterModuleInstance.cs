using Godot;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class SorterModuleInstance : ModuleInstance
    {
        public SorterModule Module;
        Site Site;

        Dictionary<GraphSim.Resource, LogisticsEndpoint> Outputs = new();
        List<LogisticsEndpoint> Inputs = new();

        public SorterModuleInstance(SorterModule module) : base(module)
        {
            Module = module;

            foreach (var kvpair in Module.Consumes)
            {
                Inputs.Add(new LogisticsEndpoint { 
                    Resource = kvpair.Key,
                    Capacity = kvpair.Value * Constants.DataScale * Constants.BufferScale,
                    Mode = LogisticsMode.Consumes
                });
            }

            foreach (Resource accepts in Module.Accepts)
            {
                Outputs.Add(accepts, new LogisticsEndpoint
                {
                    Resource = accepts,
                    Capacity = Constants.DataScale * Constants.BufferScale,
                    Mode = LogisticsMode.Produces
                });
            }

            UI = new VBoxContainer();
            UI.AddChild(new Label { Text = "Sorter" });
            foreach (LogisticsEndpoint p in Inputs)
            {
                UI.AddChild(new ResourceBar { Node = p });
                AddChild(p);
            }
            UI.AddChild(new HSeparator());
            foreach (LogisticsEndpoint p in Outputs.Values)
            {
                UI.AddChild(new ResourceBar { Node = p });
                AddChild(p);
            }
        }

        public override void _Ready()
        {
            base._Ready();
            Site = this.GetFirstParentOfType<Site>();
        }

        public override int DoWork(int budget)
        {
            int target = budget;

            foreach (LogisticsEndpoint output in Outputs.Values)
                target = Math.Min(output.Space / Constants.BufferScale / Constants.SimulationSpeed, target);

            foreach (LogisticsEndpoint input in Inputs)
                target = Math.Min(input.Amount / PerCycle(input), target);

            Site.Extraction extracted = Site.Extract(target);

            foreach (LogisticsEndpoint input in Inputs)
                input.Withdraw(extracted.Amount * PerCycle(input));

            LogisticsEndpoint result;
            if (Outputs.TryGetValue(extracted.Resource, out result))
                result.Deposit(extracted.Amount);
            else
                Site.Dump(extracted.Resource, extracted.Amount);

            return extracted.Amount;
        }
    }
}
