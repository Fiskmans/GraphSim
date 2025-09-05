using Godot;
using System;
using System.Collections.Generic;

public partial class ContainerUi : VBoxContainer, IExtraCapacity
{
    [Export]
    public string ContainerName = "N/A";

    [Export]
    public Node ContainerNode;
    Container ContainerObject;

    class Values
    {
        public float Last;
        public Label Amount;
        public RichTextLabel Delta;
        public float Capacity;
    }

    GridContainer ContentArea = new GridContainer { Columns = 5, SizeFlagsHorizontal = SizeFlags.ExpandFill };
    private Dictionary<GraphSim.Resource, Values> Entries = new();

    private string FormatDelta(float delta)
    {
        if (Math.Abs(delta) < 0.00001)
        {
            return "";
        }

        if (delta > 0)
        {
            return $"[color=green]+{delta.ToString("0.00000")}[/color]";
        }
        else
        {
            return $"[color=red]{delta.ToString("0.00000")}[/color]";
        }
    }

    public override void _Ready()
    {
        base._Ready();

        ContainerObject = ContainerNode as Container;
        ContainerObject.ExtraCapacities.Add(this);

        foreach (GraphSim.Resource resource in Enum.GetValues<GraphSim.Resource>())
        {
            ContainerObject.SpaceFor(resource);
        }

        AddChild(new Label { Text = ContainerName });
        ScrollContainer contentScroller = new ScrollContainer
        {
            VerticalScrollMode = ScrollContainer.ScrollMode.ShowAlways,
            HorizontalScrollMode = ScrollContainer.ScrollMode.ShowNever,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            SizeFlagsHorizontal = SizeFlags.Expand,
            CustomMinimumSize = new Vector2(400,100),
            GrowHorizontal = GrowDirection.Begin,
            SizeFlagsStretchRatio = 1,
            DrawFocusBorder = true
        };

        GrowHorizontal = GrowDirection.End;
        GrowVertical = GrowDirection.End;

        contentScroller.AddChild(ContentArea);
        AddChild(contentScroller);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var kvPair in ContainerObject.Contents)
        {
            Values vals;

            if (!Entries.TryGetValue(kvPair.Key, out vals))
            {
                vals = new();
                vals.Last = kvPair.Value;
                vals.Amount = new Label { 
                    VerticalAlignment = VerticalAlignment.Top,
                    SizeFlagsStretchRatio = 1 
                };
                vals.Delta = new RichTextLabel { 
                    BbcodeEnabled = true, 
                    FitContent = true, 
                    AutowrapMode = TextServer.AutowrapMode.Off, 
                    VerticalAlignment = VerticalAlignment.Center, 
                    SizeFlagsStretchRatio = 0 
                };

                vals.Capacity = 0;

                Entries.Add(kvPair.Key, vals);

                Button buy = new Button { 
                    Text = "Buy", 
                    Size = new Vector2(50, 0),
                    SizeFlagsStretchRatio = 0
                };

                buy.Pressed += () => ContainerObject.Contents[kvPair.Key] += 1;

                Button expand = new Button { 
                    Text = "Expand", 
                    Size = new Vector2(50, 2),
                    SizeFlagsStretchRatio = 0
                };
                expand.Pressed += () => vals.Capacity += 100.0f;

                ContentArea.AddChild(new Label { Text = kvPair.Key.ToString(), VerticalAlignment = VerticalAlignment.Center });
                ContentArea.AddChild(vals.Amount);
                ContentArea.AddChild(vals.Delta);
                ContentArea.AddChild(buy);
                ContentArea.AddChild(expand);
            }

            vals.Amount.Text = $"{kvPair.Value.ToString("0.0")}\n";
            vals.Delta.Text = FormatDelta((kvPair.Value - vals.Last) / (float)delta);
            vals.Last = kvPair.Value;
        }
    }

    public float ExtraCapacityFor(GraphSim.Resource Type)
    {
        Values vals;
        if (!Entries.TryGetValue(Type, out vals))
            return 0;

        return vals.Capacity;
    }
}
