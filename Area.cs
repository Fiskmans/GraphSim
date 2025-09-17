using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Area : Node
{
    Dictionary<GraphSim.Resource, float> Resources = new();

    public Area()
    {
        Generate();
    }

    public void Generate()
    {
        Resources.Add(GraphSim.Resource.Rock, 100000);
        Resources.Add(GraphSim.Resource.Hematite, 100000);
        Resources.Add(GraphSim.Resource.Malachite, 1000);
    }

    public float FractionOf(GraphSim.Resource resource)
    {
        float val;
        if (!Resources.TryGetValue(resource, out val))
            return 0.0f;

        return val / Resources.Values.Sum();
    }

    public void Remove(GraphSim.Resource resource, float amount)
    {
        if (!Resources.ContainsKey(resource))
            return;

        if ((Resources[resource] -= amount) < 0.0f)
        {
            Resources.Remove(resource);
        }
    }
}
