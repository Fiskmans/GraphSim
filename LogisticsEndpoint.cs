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

    public partial class LogisticsEndpoint : Godot.Node2D
    {
        LogisticsHub Hub;
        SiteItem GridOwner;
        public Port Port;
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

        public LogisticsMode Mode;

        public delegate void ChangeHandler(int newValue, int delta);

        public event ChangeHandler OnChange;
        public event ChangeHandler OnTransfer;

        public int _Amount = 0;
        public int Amount {
            get => _Amount;
            protected set
            {
                if (value == _Amount)
                    return;

                int delta = value - _Amount;
                _Amount = value;
                OnChange?.Invoke(_Amount, delta);
            }
        }

        public int Capacity;
        public float Fraction { get => (float)Amount / (float)Capacity; }
        public float SpaceFraction { get => 1.0f - Fraction; }
        public int Space { get => Capacity - Amount; }

        public bool Full { get => Space == 0; }

        public TraceFinder.TraceCoord Exit => Port.Out(GridOwner?.GridPosition ?? new Vector2I());

        public TraceFinder.TraceCoord Entry => Port.In(GridOwner?.GridPosition ?? new Vector2I());

        public override void _Ready()
        {
            GridOwner = this.GetFirstParentOfType<SiteItem>();

            PortType wantedType = PortType.Output;

            switch (Mode)
            {
                case LogisticsMode.Produces:
                    wantedType = PortType.Output;
                    break;
                case LogisticsMode.Consumes:
                case LogisticsMode.Sinks:
                    wantedType = PortType.Input;
                    break;
                case LogisticsMode.Stores:
                    wantedType = PortType.InOut;
                    break;
            }

            if (GridOwner is BuildingInstance building)
                Port = building.GetPort(wantedType);

            if (Port == null)
                Port = new Port { Type = wantedType };

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

        public int Deposit(int amount, bool isTransfer = false)
        {
            int space = Space;
            if (space < amount)
            {

                Amount = Capacity;
                if (isTransfer)
                    OnTransfer?.Invoke(Capacity, space);

                return space;
            }


            Amount += amount;

            if (isTransfer)
                OnTransfer?.Invoke(Amount, amount);

            return amount;
        }

        public int Withdraw(int amount, bool isTransfer = false)
        {
            int has = Amount;
            if (has < amount)
            {
                Amount = 0;

                if (isTransfer)
                    OnTransfer?.Invoke(0, -has);

                return has;
            }

            Amount -= amount;

            if (isTransfer)
                OnTransfer?.Invoke(Capacity, -amount);

            return amount;
        }
    }
}
