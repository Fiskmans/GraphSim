using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class LogisticsHub : Godot.Node
    {
        class Network
        {
            const float epsilon = 0.0000000001f;
            public List<LogisticsEndpoint>[] Layers = [new(), new(), new()];

            public void Register(LogisticsEndpoint endpoint)
            {
                Layers[(int)endpoint.Mode].Add(endpoint);
            }

            public bool Unregister(LogisticsEndpoint endpoint)
            {
                Layers[(int)endpoint.Mode].Remove(endpoint);

                return Layers.All(l => l.Count == 0);
            }

            public void DoTransfers()
            {
                Transfer(from: Layers[(int)LogisticsMode.Produces],  to: Layers[(int)LogisticsMode.Consumes]);
                Transfer(from: Layers[(int)LogisticsMode.Stores],    to: Layers[(int)LogisticsMode.Consumes]);
                Transfer(from: Layers[(int)LogisticsMode.Produces],  to: Layers[(int)LogisticsMode.Stores]);
            }

            public void Transfer(List<LogisticsEndpoint> from, List<LogisticsEndpoint> to)
            {
                float available = from.Select(n => n.Amount).Sum();
                float space = to.Select(n => n.Space).Sum();
                float amount = Math.Min(available, space);

                if (amount <= epsilon)
                    return;

                Distribute(from,    amount, (n, a) => n.Withdraw(a));
                Distribute(to,      amount, (n, a) => n.Deposit(a));
            }

            private static ThreadLocal<List<LogisticsEndpoint>> DistroList = new();

            private void Distribute(List<LogisticsEndpoint> between, float amount, Func<LogisticsEndpoint, float, float> applyFunc)
            {

                if (!DistroList.IsValueCreated)
                    DistroList.Value = new();

                List<LogisticsEndpoint> list = DistroList.Value;

                list.Clear();
                list.AddRange(between);

                float excess = amount;
                while (excess > epsilon)
                {
                    float per = amount / list.Count();
                    excess = 0f;

                    if (list.Count == 0)
                    {
                        GD.PrintErr($"Could not fully distribute {amount} between [{string.Join(',', between.Select((n) => n.GetPath()))}]");
                        return;
                    }

                    for(int i = list.Count - 1; i >= 0; i--)
                    {
                        float applied = applyFunc(list[i], per);

                        if (applied < per - epsilon)
                        {
                            list.RemoveAt(i);
                            excess += per - applied;
                        }
                    }
                }
            }
        };

        Dictionary<GraphSim.Resource, Network> Networks = new();

        public override void _Process(double delta)
        {
            foreach (Network network in Networks.Values)
                network.DoTransfers();
        }

        public void Register(LogisticsEndpoint endpoint)
        {
            Network net;
            if (!Networks.TryGetValue(endpoint.Resource, out net))
            {
                net = new Network();
                Networks.Add(endpoint.Resource, net);
            }

            net.Register(endpoint);
        }

        public void Unregister(LogisticsEndpoint endpoint)
        {
            if (Networks[endpoint.Resource].Unregister(endpoint))
                Networks.Remove(endpoint.Resource);
        }
    }
}
