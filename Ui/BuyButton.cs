using Godot;
using GraphSim.Extensions;
using System;

namespace GraphSim
{
    public partial class BuyButton : PopupMenu
    {
        [Export]
        public Node Destination;

        public BuyButton()
        {
            this.AddRange(Enum.GetValues<Resource>(), r => r.ToString(), (selected) => 
            { 
                int amount = 10;

                if (Input.IsKeyPressed(Key.Shift))
                    amount *= 10;

                if (Input.IsKeyPressed(Key.Ctrl))
                    amount *= 100;

                Destination.AddChild(new Supply { Resource = selected, Stock = amount });
            });
        }
    }

}
