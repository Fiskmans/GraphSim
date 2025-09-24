using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public partial class BuildButton : PopupMenu
{
    Data Data;

    [Export]
    Node TargetNode;

    public override void _Ready()
    {
        base._Ready();

        Data = this.GetFirstParentOfType<DataLoader>().Data;

        this.AddRange(Data.Buildings, (b) => b.Name, (b) =>
        {
            WorldSlot slot;

            if (!TryGetSlot(out slot))
                return;

            slot.Content = new ConstructionSite(b.Name, b.Cost, () => new BuildingInstance(b));
        });
    }

    bool TryGetSlot(out WorldSlot slot)
    {
        slot = TargetNode.GetChildren()
                    .Where((c) => c is WorldSlot)
                    .Select((c) => c as WorldSlot)
                    .Where((s) => s.Content == null)
                    .FirstOrDefault();

        return slot != null;
    }
}
