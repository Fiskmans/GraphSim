using Godot;
using GraphSim;
using System;
using System.Linq;

public partial class Extractor : PanelContainer
{
    public GraphSim.Resource Type;
    Container Container;

    public float Speed = 1;
    public int Amount = 1;

    Label AmountLabel = new Label { };

    public override void _EnterTree()
    {
        base._EnterTree();

        Extractor other = this.GetSiblings<Extractor>().Where(e => e.Type == Type).FirstOrDefault();
        if (other != null) 
        {
            other.Amount += Amount;
            this.QueueFree();
        }
    }

    public Extractor(GraphSim.Resource type, Container container)
    {
        Type = type;
        Container = container;

        CustomMinimumSize = new Vector2 { X = 50, Y = 50 };

        ExtractableAttribute extractable = type.GetAttribute<ExtractableAttribute>();

        Speed *= extractable.Yield;
        Modulate = extractable.Color;

        TooltipText = type.ToString();
    }

    public override void _Process(double delta)
    {
        float space = Container.SpaceFor(Type);

        float production = Speed * Amount * (float)delta;

        if (space > production)
        {
            Container.Contents[Type] += production;
        }
        else
        {
            Container.Contents[Type] += space;
        }
    }
}
