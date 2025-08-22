using Godot;
using System.Collections.Generic;

public partial class Container : Node2D
{
    [Export]
    public float Capacity = 100;

    public Dictionary<string, float> Contents = new();

    public float AmountOf(string type)
    {
        if (!Contents.ContainsKey(type))
            Contents.Add(type, 0);

        return Contents[type];
    }

    public float SpaceFor(string type)
    {
        return Capacity - AmountOf(type);
    }
}
