using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class LogisticsHub : Control
    {
        Dictionary<GraphSim.Resource, LogisticsNetwork> Networks = new();

        public void Register(LogisticsEndpoint endpoint)
        {
            if (!IsNodeReady())
            {
                Callable.From(() => this.Register(endpoint)).CallDeferred();
                return;
            }

            LogisticsNetwork net;
            if (!Networks.TryGetValue(endpoint.Resource, out net))
            {
                net = new LogisticsNetwork();
                Networks.Add(endpoint.Resource, net);
                AddChild(net);
            }

            net.Register(endpoint);
        }

        public void Unregister(LogisticsEndpoint endpoint)
        {
            if (!IsNodeReady())
            {
                Callable.From(() => this.Unregister(endpoint)).CallDeferred();
                return;
            }

            if (Networks[endpoint.Resource].Unregister(endpoint))
                Networks.Remove(endpoint.Resource);
        }
    }
}
