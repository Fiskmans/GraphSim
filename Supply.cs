using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using System;

public partial class Supply : LogisticsEndpoint
{
    [Export]
    public int Stock;

    public override void _Ready()
    {
        Capacity = Stock * Constants.DataScale;
        Amount = Stock * Constants.DataScale;
        Mode = LogisticsMode.Produces;

        OnChange += (v, d) =>
        {
            Capacity = v;
            if (v == 0)
                QueueFree();
        };

        base._Ready();
    }

    public override void _EnterTree()
    {
        Position = new Vector2(GD.Randf(), GD.Randf()) * (this.GetFirstParentOfType<Control>()?.Size ?? new Vector2(0,0));

        base._EnterTree();
    }
}
