using Godot;
using GraphSim;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public interface IExtraCapacity
{
    public float ExtraCapacityFor(GraphSim.Resource Type);
}

public partial class Container : Node2D
{
    [Export]
    public float Capacity = 100;

    public List<IExtraCapacity> ExtraCapacities = new();
    Dictionary<GraphSim.Resource, float> Contents = new();
    Dictionary<GraphSim.Resource, int> DumpingRefs = new();
    Dictionary<GraphSim.Resource, DecayingAttribute> Decay = new();

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var kvPair in Decay)
        {
            Contents[kvPair.Key] = Contents[kvPair.Key] * kvPair.Value.DecayAfter((float)delta);
        }
    }


    public IEnumerable<KeyValuePair<GraphSim.Resource, float>> ReadContents()
    {
        return Contents;
    }

    public float AmountOf(GraphSim.Resource type)
    {
        if (!Contents.ContainsKey(type))
        {
            Contents.Add(type, 0);
            DecayingAttribute decay = type.GetAttribute<DecayingAttribute>();

            if (decay != null)
                Decay.Add(type, decay);
        }

        if (float.IsNaN(Contents[type]) || float.IsInfinity(Contents[type]))
        {
            Contents[type] = 0;
        }

        return Contents[type];
    }

    public float CapacityFor(GraphSim.Resource type)
    {
        return Capacity + ExtraCapacities.Select(e => e.ExtraCapacityFor(type)).Sum();
    }

    public float SpaceFor(GraphSim.Resource type)
    {
        if (DumpingRefs.ContainsKey(type))
        {
            return float.PositiveInfinity;
        }

        return CapacityFor(type) - AmountOf(type);
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

    public void Add(GraphSim.Resource type, float amount)
    {
        Contents[type] += amount;

        float cap = CapacityFor(type);

        if (Contents[type] > cap)
            Contents[type] = cap;
    }

    public void Remove(GraphSim.Resource type, float amount)
    {
        if (amount > AmountOf(type))
        {
            Contents[type] = 0;
        }
        else
        {
            Contents[type] -= amount;
        }
    }
}
