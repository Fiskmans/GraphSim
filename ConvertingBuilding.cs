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

    Dictionary<GraphSim.Resource, LogisticsEndpoint> Inputs = new();
    Dictionary<GraphSim.Resource, LogisticsEndpoint> Outputs = new();


    public ConvertingBuilding(string name, Conversion conversion, Dictionary<GraphSim.Resource, float> catalysts)
    {
        Label = name;
        Conversion = conversion;

        if (catalysts != null)
        {
            foreach (var kvPair in catalysts)
            {
                Inputs.Add(kvPair.Key, new LogisticsEndpoint { Resource = kvPair.Key, Capacity = kvPair.Value, Mode = LogisticsMode.Consumes });
            }
        }

        foreach (var kvPair in conversion.Amounts)
        {
            if (kvPair.Value > 0)
            {
                Outputs.Add(kvPair.Key, new LogisticsEndpoint { Resource = kvPair.Key, Capacity = kvPair.Value * 10, Mode = LogisticsMode.Produces });
            }
            else
            {
                Inputs.TryAdd(kvPair.Key, new LogisticsEndpoint { Resource = kvPair.Key, Capacity = -kvPair.Value * 10, Mode = LogisticsMode.Consumes });
            }
        }

        Tooltip = new VBoxContainer();

        Tooltip.AddChild(new Label { Text = Label });
        Tooltip.AddChild(LoadBar);

        Tooltip.AddChild(new HSeparator());
        Tooltip.AddChild(new Label { Text = "Input" });
        foreach (var kvPair in Inputs)
        {
            Tooltip.AddChild(new ResourceBar { Node = kvPair.Value });
        }

        Tooltip.AddChild(new HSeparator());
        Tooltip.AddChild(new Label { Text = "Output" });
        foreach (var kvPair in Outputs)
        {
            Tooltip.AddChild(new ResourceBar { Node = kvPair.Value });
        }

        Tooltip.AddChild(new HSeparator());
        Tooltip.AddChild(Status);

        foreach (var kvPair in Inputs)
            AddChild(kvPair.Value);

        foreach (var kvPair in Outputs)
            AddChild(kvPair.Value);
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

            if (toProcess > 1.0f)
                toProcess = 1.0f;

            foreach (var kvPair in Conversion.Amounts)
            {
                if (kvPair.Value < 0)
                {
                    Inputs[kvPair.Key].Withdraw(toProcess * -kvPair.Value);
                }
                else
                {
                    Outputs[kvPair.Key].Deposit(toProcess * kvPair.Value);
                }
            }
        }
    }

    private void RecalculateLoad()
    {
        Load = 1;

        foreach (var kvPair in Conversion.Amounts)
        {
            if (kvPair.Value > 0)
            {
                AddStatus($"Space for {kvPair.Key}", Outputs[kvPair.Key].Space / kvPair.Value);
            }
            else
            {
                AddStatus($"Amount of {kvPair.Key}", Inputs[kvPair.Key].Fraction);
            }
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
