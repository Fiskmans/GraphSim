using Godot;
using GraphSim;
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

    [Export]
    Node Container;

    public override void _Ready()
    {
        base._Ready();

        Data = this.GetFirstParentOfType<DataLoader>().Data;

        PopupMenu factory = new PopupMenu();

        foreach(PopupMenuExtensions.SubMenuMapping<Building> mapping in factory.AddSubmenuRange(Data.Buildings, (b) => b.Name))
        {
            Building building = mapping.Obj;

            mapping.Menu.AddRange(building.Conversions, (c) => c.Name, (c) =>
            {
                Container cont = Container as Container;

                WorldSlot slot;

                if (!TryGetSlot(out slot))
                    return;

                var creator = () => new ConvertingBuilding($"{building.Name}:{c.Name}", c, cont, building.Catalysts);

                if (building.Cost != null)
                {
                    slot.Content = new ConstructionSite(building.Name, cont, building.Cost, creator);
                }
                else
                {
                    slot.Content = creator();
                }
            });
        }

        AddSubmenuNodeItem("Factory", factory);

        PopupMenu extractor = new PopupMenu();

        AddExtractorType(extractor, "solar");
        AddExtractorType(extractor, "drill");

        AddSubmenuNodeItem("Extractor", extractor);

        PopupMenu sifting = new PopupMenu();
        AddSiftable(sifting);
        AddSubmenuNodeItem("Sifting", sifting);
    }

    void AddExtractorType(PopupMenu menu, string extractorTag)
    {
        PopupMenu extractor = new PopupMenu();

        foreach (GraphSim.Resource resource in Enum.GetValues<GraphSim.Resource>())
        {
            ExtractableAttribute extractable = resource.GetAttribute<ExtractableAttribute>();

            if (extractable == null)
                continue;

            if (!extractable.ExtractorTags.Contains(extractorTag))
                continue;

            extractor.AddItem(resource.ToString(), (int)resource);
        }

        extractor.IdPressed += (long id) =>
        {
            WorldSlot slot;

            if (!TryGetSlot(out slot))
                return;

            slot.Content = new Extractor((GraphSim.Resource)id, Container as Container);
        };

        menu.AddSubmenuNodeItem(extractorTag, extractor);
    }

    void AddSiftable(PopupMenu menu)
    {
        foreach (GraphSim.Resource resource in Enum.GetValues<GraphSim.Resource>())
        {
            SeparatableAttribute siftable = resource.GetAttribute<SeparatableAttribute>();

            if (siftable == null)
                continue;

            menu.AddItem(resource.ToString(), (int)resource);
        }

        menu.IdPressed += (long id) =>
        {
            WorldSlot slot;

            if (!TryGetSlot(out slot))
                return;

            slot.Content = new Sifter(Container as Container, (GraphSim.Resource)id);
        };
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
