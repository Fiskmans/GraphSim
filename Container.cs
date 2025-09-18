using Godot;
using GraphSim;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public partial class Container : Node2D
{
    [Export]
    public float Capacity = 100;

    Dictionary<GraphSim.Resource, LogisticsEndpoint> Contents = new();
    Dictionary<GraphSim.Resource, int> DumpingRefs = new();
    Dictionary<GraphSim.Resource, DecayingAttribute> Decay = new();

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var kvPair in Decay)
        {
            Contents[kvPair.Key].Amount = Contents[kvPair.Key].Amount * kvPair.Value.DecayAfter((float)delta);
        }
    }


    public IEnumerable<KeyValuePair<GraphSim.Resource, LogisticsEndpoint>> ReadContents()
    {
        return Contents;
    }

    public LogisticsEndpoint Query(GraphSim.Resource type)
    {
        if (!Contents.ContainsKey(type))
        {
            LogisticsEndpoint endpoint = new LogisticsEndpoint { Resource = type, Capacity = Capacity, Amount = 0, Mode = LogisticsMode.Stores };

            AddChild(endpoint);
            Contents.Add(type, endpoint);
            DecayingAttribute decay = type.GetAttribute<DecayingAttribute>();

            if (decay != null)
                Decay.Add(type, decay);
        }

        return Contents[type];
    }

    public float SpaceFor(GraphSim.Resource type)
    {
        if (DumpingRefs.ContainsKey(type))
        {
            return float.PositiveInfinity;
        }

        return Query(type).Space;
    }

    public void StartDumping(GraphSim.Resource type)
    {
        if (!DumpingRefs.ContainsKey(type))
            DumpingRefs.Add(type, 0);

        DumpingRefs[type]++;
    }

    public void StopDumping(GraphSim.Resource type)
    {
        if(--DumpingRefs[type] == 0)
            DumpingRefs.Remove(type);
    }
}
