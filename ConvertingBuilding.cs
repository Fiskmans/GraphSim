using Godot;
using GraphSim;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ConvertingBuilding : SlotItem
{
    Conversion Conversion;
    string Label;

    public float Load = 0;
    public float HardLimit = 0;

    readonly Color StatusGood = new Color(0.8f, 1, 0.8f);
    readonly Color StatusLimited = new Color(1.0f, 1, 0.4f);
    readonly Color StatusStopped = new Color(1.0f, 0, 0);

    Color _StatusColor;
    Color StatusColor
    {
        get => _StatusColor;
        set
        {
            _StatusColor = value;
            QueueRedraw();
        }
    }
    Font Font;

    ProgressBar LoadBar = new ProgressBar();

    RichTextLabel ResourceStatus = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };
    RichTextLabel Status = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };

    Container Container;

    Dictionary<GraphSim.Resource, Catalyst> Catalysts = new();

    public ConvertingBuilding(string name, Conversion conversion, Container container, Dictionary<GraphSim.Resource, float> catalysts)
    {
        Label = name;
        Conversion = conversion;
        Container = container;

        if (catalysts != null)
        {
            foreach (var kvPair in catalysts)
            {
                Catalysts.Add(kvPair.Key, new Catalyst(kvPair.Key, kvPair.Value));
            }
        }

        Tooltip = new VBoxContainer();

        Tooltip.AddChild(new Label { Text = Label });
        Tooltip.AddChild(LoadBar);

        if (Catalysts.Count != 0)
            Tooltip.AddChild(new HSeparator());

        foreach (var kvPair in Catalysts)
        {
            Tooltip.AddChild(kvPair.Value);
        }

        Tooltip.AddChild(new HSeparator());
        Tooltip.AddChild(Status);
    }

    public override void _Ready()
    {
        if (Font == null)
            Font = ThemeDB.FallbackFont;
    }

    private void AddStatus(string reason, float amount)
    {
        if (amount < 0.0001f)
        {
            Status.Text = $"Stopped by {reason}\n";
        }
        else if (amount < 1.0f && amount < Load)
        {
            Status.Text = $"Limited by {reason}\n";
        }

        Load = Math.Max(0, Math.Min(Load, amount));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        ResourceStatus.Text = "";
        Status.Text = "";

        if (Conversion != null)
        {
            RecalculateLoad();

            float toProcess = Load * (float)delta;

            foreach (var kvPair in Conversion.Amounts)
            {
                if (kvPair.Value < 0)
                {
                    if (Catalysts.ContainsKey(kvPair.Key))
                    {
                        Catalysts[kvPair.Key].Consume(toProcess * -kvPair.Value);
                    }
                    else
                    {
                        Container.Remove(kvPair.Key, toProcess * -kvPair.Value);
                    }
                }
                else
                {
                    Container.Add(kvPair.Key, toProcess * kvPair.Value);
                }
            }
        }

        foreach (var kvPair in Catalysts)
        {
            kvPair.Value.Refill(Container);
        }
    }

    private void RecalculateLoad()
    {
        Load = 1;

        foreach (var kvPair in Conversion.Amounts)
        {
            if (kvPair.Value > 0)
            {
                AddStatus($"Space for {kvPair.Key}", Container.SpaceFor(kvPair.Key) / kvPair.Value);
            }
            else
            {
                if (Catalysts.ContainsKey(kvPair.Key))
                    AddStatus($"Amount of {kvPair.Key}", Catalysts[kvPair.Key].Value / -kvPair.Value);
                else
                    AddStatus($"Amount of {kvPair.Key}", Container.AmountOf(kvPair.Key) / -kvPair.Value);
            }
        }

        foreach (var kvPair in Catalysts)
        {
            AddStatus($"Available {kvPair.Key}", kvPair.Value.Fraction);
        }

        LoadBar.Value = Load * 100;

        if (Load > 0.1f)
        {
            float weight = Mathf.InverseLerp(0.1f, 1.0f, Load);
            StatusColor = StatusLimited.Lerp(StatusGood, weight);
        }
        else
        {
            StatusColor = StatusStopped.Lerp(StatusLimited, Load * 10.0f);
        }
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(Size.X - 5, 5), 4, StatusColor);
        DrawCharOutline(Font, new Vector2(2, Size.Y - 3), Label.Substr(0,1));
    }
}
