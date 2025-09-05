using Godot;
using Godot.Collections;
using System;

public partial class Extractors : GridContainer
{
    class ExtractorComponents
    {
        public Label Amount;
        public Button Add;
    }

    System.Collections.Generic.Dictionary<Node, ExtractorComponents> Mappings = new();

    public override void _Ready()
    {
        base._Ready();

        AddChild(new Label { Text = "Type" });
        AddChild(new Label { Text = "Speed" });
        AddChild(new Label { Text = "Amount" });
        AddChild(new Label { Text = "Add" });

        foreach(Node child in GetChildren())
        {
            Extractor extractor = child as Extractor;

            if (extractor == null)
                continue;

            ExtractorComponents comps = new();

            comps.Amount = new Label { Text = $"{extractor.Amount}" };
            comps.Add = new Button { Text = "Build" };
            comps.Add.Pressed += () => { extractor.Amount++; comps.Amount.Text = $"{extractor.Amount}"; };


            AddChild(new Label { Text = extractor.Type.ToString() });
            AddChild(new Label { Text = $"{extractor.Speed}" });
            AddChild(comps.Amount);
            AddChild(comps.Add);
        }
    }

}
