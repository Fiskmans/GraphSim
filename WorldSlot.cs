using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using System;

public partial class WorldSlot : PanelContainer
{
    [Export]
    public string Autobuild;

    SlotItem _Content;
    public SlotItem Content
    {
        get => _Content;
        set {
            if (_Content != null)
                this.RemoveChild(_Content);
            this._Content = value;
            this.AddChild(_Content);
        }
    }


    public WorldSlot()
    {
        CustomMinimumSize = new Vector2(50, 50);
    }

    public override void _Ready()
    {
        base._Ready();

        if (Autobuild == null)
            return;

        Building building = null;

        foreach (Building b in this.GetFirstParentOfType<DataLoader>().Data.Buildings)
        {
            if (!string.Equals(b.Name, Autobuild))
                continue;
         
            building = b;
            break;
        }

        if (building == null)
        {
            GD.PrintErr($"No such building: [{Autobuild}]");
            return;
        }

        Content = new BuildingInstance(building);
    }
}
