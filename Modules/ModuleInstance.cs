using Godot;
using GraphSim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class ModuleInstance : Node2D
    {
        public Module BaseModule;
        public Control UI = new VBoxContainer();

        protected List<LogisticsEndpoint> Inputs = new();
        protected Dictionary<Resource, LogisticsEndpoint> BasicOutputs = new();

        public ModuleInstance(Module baseModule)
        {
            BaseModule = baseModule;
        }

        public virtual int DoWork(int budget)
        {
            return 0;
        }

        public void SetupInput(Dictionary<Resource, int> input)
        {
            if (input.Count == 0)
                return;

            UI.AddChild(new Label { Text = "Inputs" });

            foreach (var kvpair in input)
                AddInput(kvpair);
        }

        public void AddInput(KeyValuePair<Resource,int> input)
        {
            LogisticsEndpoint endpoint = new LogisticsEndpoint
            {
                Resource = input.Key,
                Capacity = input.Value * Constants.BufferScale * Constants.DataScale,
                Mode = LogisticsMode.Consumes,
                Position = new Vector2(5, 5 + 10 * Inputs.Count)
            };

            Inputs.Add(endpoint);
            UI.AddChild(new ResourceBar { Node = endpoint });
            AddChild(endpoint);
        }

        protected void SetupBasicOutput(Dictionary<Resource, int> output)
        {
            if (output.Count == 0)
                return;

            UI.AddChild(new Label { Text = "Outputs" });

            foreach (var kvPair in output)
            {
                LogisticsEndpoint endpoint = new LogisticsEndpoint
                {
                    Resource = kvPair.Key,
                    Capacity = kvPair.Value * Constants.BufferScale * Constants.DataScale,
                    Mode = LogisticsMode.Produces,
                    Position = new Vector2(65, 5 + 10 * BasicOutputs.Count)
                };

                BasicOutputs.Add(kvPair.Key, endpoint);
                UI.AddChild(new ResourceBar { Node = endpoint });
                AddChild(endpoint);
            }
        }

        protected int PerCycle(LogisticsEndpoint endpoint)
        {
            return endpoint.Capacity / Constants.DataScale / Constants.BufferScale;
        }

        private int PerWorkunitToPerform(LogisticsEndpoint endpoint)
        {
            return endpoint.Capacity / Constants.SimulationScale;
        }

        public int GetProductionCapacity()
        {
            int cap = int.MaxValue;

            foreach (LogisticsEndpoint input in Inputs)
                cap = int.Min(cap, input.Amount / PerWorkunitToPerform(input));

            foreach (LogisticsEndpoint output in BasicOutputs.Values)
                cap = int.Min(cap, output.Space / PerWorkunitToPerform(output));

            return cap;
        }

        public void ConsumeInputs(int cycles)
        {
            foreach (LogisticsEndpoint input in Inputs)
                input.Withdraw(cycles * PerCycle(input));
        }
    }
}
