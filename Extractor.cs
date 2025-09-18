using Godot;
using GraphSim;
using System;
using System.Linq;

public partial class Extractor : SlotItem
{
    public GraphSim.Resource Type;
    Container Container;

    public float Speed = 1;

    public Extractor(GraphSim.Resource type, Container container)
    {
        Type = type;
        Container = container;

        ExtractableAttribute extractable = type.GetAttribute<ExtractableAttribute>();

        Speed *= extractable.Yield;
        Modulate = extractable.Color;

        Tooltip = new Label { Text = type.ToString() };
    }

    public override void _Process(double delta)
    {
        //Container.Add(Type, (float)delta * Speed);
    }
}
