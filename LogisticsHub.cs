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

        VBoxContainer NetworkList = new();

        public override void _Ready()
        {
            base._Ready();
            
            NetworkList.CustomMinimumSize = new Vector2I(120, 0);

            AddChild(NetworkList);
        }

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
                net.Name = $"Network_{endpoint.Resource.ToString()}";
                Networks.Add(endpoint.Resource, net);

                Color normal = endpoint.Resource.Color().Darkened(0.1f);
                Color highlight = endpoint.Resource.Color().Lightened(0.1f);

                net.Modulate = normal;

                Button label = new Button { Text = endpoint.Resource.ToString() };

                label.MouseEntered += () => net.Modulate = highlight;
                label.MouseExited += () => net.Modulate = normal;

                AddChild(net);
                NetworkList.AddChild(label);
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
