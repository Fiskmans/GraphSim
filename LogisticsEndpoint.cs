using Godot;
using GraphSim.Attributes;
using GraphSim.Data;
using GraphSim.Extensions;
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
        Sinks,
        Stores,
        Consumes
    }

    public partial class LogisticsEndpoint : Godot.Node
    {
        LogisticsHub Hub;
        private DecayingAttribute Decay;

        public GraphSim.Resource _Resource;
        [Export]
        public GraphSim.Resource Resource
        {
            get => _Resource;
            set
            {
                _Resource = value;
                Decay = value.GetAttribute<DecayingAttribute>();
            }
        }


        [Export]
        public LogisticsMode Mode;

        public delegate void ChangeHandler(int newValue, int delta);

        public event ChangeHandler OnChange;

        [Export]
        public int EditorAmount = 0;
        public int _Amount = 0;
        public int Amount {
            get => _Amount;
            private set
            {
                if (value == _Amount)
                    return;

                int delta = value - _Amount;
                _Amount = value;
                OnChange?.Invoke(_Amount, delta);
            }
        }

        [Export]
        public int EditorCapacity;
        public int Capacity;
        public float Fraction { get => (float)Amount / (float)Capacity; }
        public float SpaceFraction { get => 1.0f - Fraction; }
        public int Space { get => Capacity - Amount; }

        public bool Full { get => Space == 0; }

        public override void _Ready()
        {
            Capacity += EditorAmount * Constants.DataScale;
            Amount += EditorAmount * Constants.DataScale;

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

        public override void _Process(double delta)
        {
            if (Decay != null)
                Amount = Decay.ApplyDecay(Amount);
        }

        public override void _ExitTree()
        {
            Hub?.Unregister(this);
        }

        public int Deposit(int amount)
        {
            int space = Space;
            if (space < amount)
            {
                Amount = Capacity;
                return space;
            }

            Amount += amount;
            return amount;
        }

        public int Withdraw(int amount)
        {
            int has = Amount;
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
