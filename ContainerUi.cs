using Godot;
using GraphSim;
using System;
using System.Collections.Generic;
using System.Data;

public partial class ContainerUi : VBoxContainer
{
    [Export]
    public string ContainerName = "N/A";

    [Export]
    public Node ContainerNode;
    Container ContainerObject;

    class Values
    {
        public float Last;
        public ResourceBar Bar;
        public LogisticsEndpoint BuyStash;
        public RichTextLabel Delta;
    }

    GridContainer ContentArea = new GridContainer { Columns = 3, SizeFlagsHorizontal = SizeFlags.ExpandFill };
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
            CustomMinimumSize = new Vector2(500,100),
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

        foreach (var kvPair in ContainerObject.ReadContents())
        {
            Values vals;

            if (!Entries.TryGetValue(kvPair.Key, out vals))
            {
                vals = new();
                vals.Last = kvPair.Value.Amount;
                vals.Bar = new ResourceBar { Node = kvPair.Value };
                vals.BuyStash = new LogisticsEndpoint { Resource = kvPair.Key, Capacity = 1000, Mode = LogisticsMode.Produces };
                vals.Delta = new RichTextLabel { 
                    BbcodeEnabled = true, 
                    FitContent = true, 
                    AutowrapMode = TextServer.AutowrapMode.Off, 
                    VerticalAlignment = VerticalAlignment.Center, 
                    SizeFlagsStretchRatio = 0 
                };

                AddChild(vals.BuyStash);

                Entries.Add(kvPair.Key, vals);

                Button buy = new Button { 
                    Text = "Buy", 
                    Size = new Vector2(50, 0),
                    SizeFlagsStretchRatio = 0
                };

                buy.Pressed += () => vals.BuyStash.Deposit(Input.IsPhysicalKeyPressed(Key.Shift) ? 100 : 1);

                ContentArea.AddChild(vals.Bar);
                ContentArea.AddChild(buy);

                if (kvPair.Key.GetAttribute<DumpableAttribute>() != null)
                {
                    CheckBox dump = new CheckBox();

                    dump.Toggled += (bool value) =>
                    {
                        if (value)
                        {
                            ContainerObject.StartDumping(kvPair.Key);
                        }
                        else
                        {
                            ContainerObject.StopDumping(kvPair.Key);
                        }
                    };

                    ContentArea.AddChild(dump);
                }
                else
                {
                    ContentArea.AddChild(new Label());
                }
            }

            vals.Delta.Text = FormatDelta((kvPair.Value.Amount - vals.Last) / (float)delta);
            vals.Last = kvPair.Value.Amount;
        }
    }
}
