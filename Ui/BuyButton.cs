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
                int amount = Data.Constants.DataScale;

                if (Input.IsKeyPressed(Key.Shift))
                    amount *= 10;

                if (Input.IsKeyPressed(Key.Ctrl))
                    amount *= 100;

                LogisticsEndpoint bought = new LogisticsEndpoint
                {
                    Resource = selected,
                    Capacity = amount,
                    Mode = LogisticsMode.Produces
                };

                bought.Deposit(amount);

                bought.OnChange += (v, d) =>
                {
                    if (v == 0)
                        bought.QueueFree();

                    bought.Capacity = v;
                };

                Destination.AddChild(bought);
            });
        }
    }

}
