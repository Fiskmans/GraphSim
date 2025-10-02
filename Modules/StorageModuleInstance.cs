using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using GraphSim.Data;
using GraphSim.Extensions;

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
                    Mode = LogisticsMode.Stores,
                    Position = new Vector2(35,35)
                };

                AddChild(storage);
                UI.AddChild(new ResourceBar { Node = storage });

                storage.OnChange += (var, d) => { QueueRedraw(); };
            }
        }

        public override void _Draw()
        {
            Control owner = this.GetFirstParentOfType<Control>();

            if (owner == null)
                return;

            Vector2 size = owner.Size;
            float bottom = size.Y - 4;
            float top = 4;
            float x = size.X - 3;


            foreach (LogisticsEndpoint storage in this.GetChildrenOfType<LogisticsEndpoint>())
            {
                DrawRect(new Rect2(x - 2, top + (bottom - top) * (storage.SpaceFraction), 2, (bottom - top) * storage.Fraction), new Color(1, 1, 0), filled: true);

                x -= 2;
            }

            DrawRect(new Rect2(x, top, size.X - 3 - x + 1, bottom - top + 1), new Color(1, 1, 1, 0.5f), filled: false);
        }
    }
}
