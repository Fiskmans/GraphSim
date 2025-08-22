using Godot;
using System;
using System.Collections.Generic;

public partial class ContainerUi : VBoxContainer
{
    [Export]
    public string ContainerName = "N/A";

    [Export]
    public Node ContainerNode;

    RichTextLabel Names = new RichTextLabel { BbcodeEnabled = true, FitContent = true, AutowrapMode = TextServer.AutowrapMode.Off };
    RichTextLabel Values = new RichTextLabel { BbcodeEnabled = true, FitContent = true, AutowrapMode = TextServer.AutowrapMode.Off };
    RichTextLabel Delta = new RichTextLabel { BbcodeEnabled = true, FitContent = true, AutowrapMode = TextServer.AutowrapMode.Off };

    private Dictionary<string, float> last = new();

    private string FormatDelta(float delta)
    {
        if (Math.Abs(delta) < 0.001)
        {
            return "";
        }

        if (delta > 0)
        {
            return $"[color=green]+{delta.ToString("0.0000")}[/color]";
        }
        else
        {
            return $"[color=red]{delta.ToString("0.0000")}[/color]";
        }
    }

    public override void _Ready()
    {
        base._Ready();

        AddChild(new Label { Text = ContainerName });

        HBoxContainer contentArea = new HBoxContainer();

        contentArea.AddChild(Names);
        contentArea.AddChild(Values);
        contentArea.AddChild(Delta);

        AddChild(contentArea);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Container container = ContainerNode as Container;

        Names.Text = "";
        Values.Text = "";
        Delta.Text = "";

        foreach (var kvPair in container.Contents)
        {
            if (!last.ContainsKey(kvPair.Key))
                last.Add(kvPair.Key, kvPair.Value);

            Names.AppendText($"{kvPair.Key}\n");
            Values.AppendText($"{kvPair.Value.ToString("0.0")}\n");
            Delta.AppendText($"{FormatDelta((kvPair.Value - last[kvPair.Key]) / (float)delta)}\n");

            last[kvPair.Key] = kvPair.Value;
        }
    }
}
