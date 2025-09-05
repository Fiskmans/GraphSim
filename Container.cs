using Godot;
using System.Collections.Generic;
using System.Linq;

public interface IExtraCapacity
{
    public float ExtraCapacityFor(GraphSim.Resource Type);
}

public partial class Container : Node2D
{
    [Export]
    public float Capacity = 100;

    public List<IExtraCapacity> ExtraCapacities = new();
    public Dictionary<GraphSim.Resource, float> Contents = new();

    public float AmountOf(GraphSim.Resource type)
    {
        if (!Contents.ContainsKey(type))
            Contents.Add(type, 0);

        if (float.IsNaN(Contents[type]) || float.IsInfinity(Contents[type]))
        {
            Contents[type] = 0;
        }

        return Contents[type];
    }

    public float SpaceFor(GraphSim.Resource type)
    {
        float cap = Capacity + ExtraCapacities.Select(e => e.ExtraCapacityFor(type)).Sum();

        return cap - AmountOf(type);
    }
}
