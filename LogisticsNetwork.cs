using Godot;
using GraphSim.Extensions;
using GraphSim.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class LogisticsNetwork : Node2D
    {
        public List<LogisticsEndpoint>[] Layers = [new(), new(), new(), new()];


        public void Register(LogisticsEndpoint endpoint)
        {
            AddChild(new EndpointTraceNode(endpoint));

            Layers[(int)endpoint.Mode].Add(endpoint);
        }

        public bool Unregister(LogisticsEndpoint endpoint)
        {
            Layers[(int)endpoint.Mode].Remove(endpoint);
            QueueRedraw();

            return Layers.All(l => l.Count == 0);
        }

        public override void _Process(double delta)
        {
            Transfer(from: Layers[(int)LogisticsMode.Produces], to: Layers[(int)LogisticsMode.Consumes]);
            Transfer(from: Layers[(int)LogisticsMode.Stores], to: Layers[(int)LogisticsMode.Consumes]);
            Transfer(from: Layers[(int)LogisticsMode.Produces], to: Layers[(int)LogisticsMode.Stores]);
            Transfer(from: Layers[(int)LogisticsMode.Produces], to: Layers[(int)LogisticsMode.Sinks]);

            QueueRedraw();
        }

        public void Transfer(List<LogisticsEndpoint> from, List<LogisticsEndpoint> to)
        {
            int available = from.Select(n => n.Amount).Sum();
            int space = to.Select(n => n.Space).Sum();
            int amount = Math.Min(available, space);

            if (amount == 0)
                return;

            Distribute(from, amount, (n, a) => n.Withdraw(a, isTransfer: true));
            Distribute(to, amount, (n, a) => n.Deposit(a, isTransfer: true));
        }

        private static ThreadLocal<List<LogisticsEndpoint>> DistroList = new();

        private void Distribute(List<LogisticsEndpoint> between, int amount, Func<LogisticsEndpoint, int, int> applyFunc)
        {
            if (!DistroList.IsValueCreated)
                DistroList.Value = new();

            List<LogisticsEndpoint> list = DistroList.Value;

            list.Clear();
            list.AddRange(between);

            int left = amount;
            while (left > 0)
            {
                int per = left / list.Count();

                if (per == 0)
                    per = 1;

                if (list.Count == 0)
                {
                    GD.PrintErr($"Could not fully distribute {amount} between [{string.Join(',', between.Select((n) => n.GetPath()))}]");
                    return;
                }

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    int applied = applyFunc(list[i], per);

                    left -= applied;

                    if (applied < per)
                        list.RemoveAt(i);
                }
            }
        }
    };
}
