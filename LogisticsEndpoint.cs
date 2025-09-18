using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public enum LogisticsMode
    {
        Produces,
        Stores,
        Consumes
    }

    public partial class LogisticsEndpoint : Godot.Node
    {
        LogisticsHub Hub;
        public GraphSim.Resource Resource;
        public LogisticsMode Mode;

        public delegate void ChangeHandler(float newValue, float delta);

        public event ChangeHandler OnChange;

        public float _Amount = 0;
        public float Amount {
            get => _Amount;
            set
            {
                float delta = value - _Amount;
                _Amount = value;
                OnChange?.Invoke(_Amount, delta);
            }
        }
        public float Capacity;
        public float Fraction { get => Amount / Capacity; }
        public float Space { get => Capacity - Amount; }

        public bool Full { get => Space < 0.00001f; }

        public override void _Ready()
        {
            Hub = this.GetFirstParentOfType<LogisticsHub>();
            if (Hub != null)
            {
                Hub.Register(this);
            }
            else
            {
                GD.Print($"Logistics node is unable to locate a hub to connect to {GetPath()}");
            }
        }

        public override void _ExitTree()
        {
            Hub?.Unregister(this);
        }

        public float Deposit(float amount)
        {
            float space = Space;
            if (space < amount)
            {
                Amount = Capacity;
                return space;
            }

            Amount += amount;
            return amount;
        }

        public float Withdraw(float amount)
        {
            float has = Amount;
            if (has < amount)
            {
                Amount = 0;
                return has;
            }

            Amount -= amount;
            return amount;
        }
    }
}
