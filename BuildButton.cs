using Godot;
using GraphSim;
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

        for (int i = 0; i < Data.Buildings.Count; i++)
        {
            factory.AddItem(Data.Buildings[i].Name, i);
        }

        factory.IdPressed += (long id) =>
        {
            TargetNode.AddChild(new Factory(Data.Buildings[(int)id], Container as Container));
        };

        AddSubmenuNodeItem("Factory", factory);

        PopupMenu extractor = new PopupMenu();

        AddExtractorType(extractor, "solar");
        AddExtractorType(extractor, "drill");

        AddSubmenuNodeItem("Extractor", extractor);
    }

    void AddExtractorType(PopupMenu menu, string extractorTag)
    {
        PopupMenu extractor = new PopupMenu();
        
        Type enumType = typeof(GraphSim.Resource);

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
            TargetNode.AddChild(new Extractor((GraphSim.Resource)id, Container as Container));
        };

        menu.AddSubmenuNodeItem(extractorTag, extractor);
    }
}
