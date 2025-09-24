using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using GraphSim.Data;

namespace GraphSim
{
    public partial class StorageModuleInstance : ModuleInstance
    {
        StorageModule Module;

        public StorageModuleInstance(StorageModule module) : base(module)
        {
            Module = module;
        }

        public override void _Ready()
        {
            base._Ready();

            foreach (var kvPair in Module.Stores)
            {
                LogisticsEndpoint storage = new LogisticsEndpoint
                {
                    Resource = kvPair.Key,
                    Capacity = kvPair.Value * Constants.DataScale,
                    Mode = LogisticsMode.Stores
                };

                AddChild(storage);
                UI.AddChild(new ResourceBar { Node = storage });
            }
        }
    }
}
