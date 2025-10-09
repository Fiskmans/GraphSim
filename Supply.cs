using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;

public partial class Supply : SiteItem
{
    [Export]
    public int Stock;

    [Export]
    public GraphSim.Resource Resource;

    public Supply() : base(new Vector2I(0,0))
    {

    }

    public override void _Ready()
    {
        LogisticsEndpoint output = new LogisticsEndpoint
        {
            Capacity = Stock * Constants.DataScale,
            Resource = Resource,
            Mode = LogisticsMode.Produces
        };
        output.Deposit(int.MaxValue);

        output.OnChange += (v, d) =>
        {
            if (v == 0)
                QueueFree();
        };

        AddChild(output);

        base._Ready();
    }

    public override void _EnterTree()
    {
        Position = new Vector2(GD.Randf(), GD.Randf()) * (this.GetFirstParentOfType<Control>()?.Size ?? new Vector2(0,0));

        base._EnterTree();
    }

    public override IEnumerable<Rect2I> GetShape()
    {
        return [
                new Rect2I(0,0,1,1)
            ];
    }

    public override Port GetPort(PortType type)
    {
        return new Port { Position = new Vector2I(0, 0), Type = PortType.Output, Direction = GraphSim.Enums.Direction.East };
    }
}
