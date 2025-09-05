using Godot;
using GraphSim;
using System;
using System.Collections.Generic;

public partial class Factory : VBoxContainer
{
    Building Type;
    Conversion Conversion;

    public float Load = 0;

    ProgressBar LoadBar = new ProgressBar();

    RichTextLabel ResourceStatus = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };
    RichTextLabel OutputStatus = new RichTextLabel { AutowrapMode = TextServer.AutowrapMode.Off, FitContent = true };

    Container Container;
    Data Data;

    OptionButton ConverterSelector = new();

    public Factory(Building type, Container container)
    {
        this.Type = type;
        this.Container = container;
    }

    public override void _Ready()
    {
        base._Ready();

        Data = this.GetFirstParentOfType<DataLoader>().Data;

        foreach (Conversion conversion in Type.Conversions)
        {
            ConverterSelector.AddItem(conversion.Name);
        }

        ConverterSelector.ItemSelected += ConversionSelected;

        AddChild(new Label { Text = Type.Name });
        AddChild(LoadBar);
        AddChild(ConverterSelector);
        AddChild(new HSeparator());
        AddChild(ResourceStatus);
        AddChild(new Label { Text = "VVV", HorizontalAlignment = HorizontalAlignment.Center });
        AddChild(OutputStatus);

        ConversionSelected(0);
    }

    private void ConversionSelected(long index)
    {
        Conversion = Type.Conversions[(int)index];
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        ResourceStatus.Text = "";
        OutputStatus.Text = "";

        if (Conversion != null)
        {
            Load = 1;

            foreach (var kvPair in Conversion.Amounts)
            {
                if (kvPair.Value > 0)
                {
                    float fract = Container.SpaceFor(kvPair.Key) / kvPair.Value;

                    if (fract < 0.01f)
                    {
                        OutputStatus.AddText($"{kvPair.Key}: Full\n");
                    }
                    else if (fract < 1.0f)
                    {
                        OutputStatus.AddText($"{kvPair.Key}: Limited\n");
                    }
                    else
                    {
                        OutputStatus.AddText($"{kvPair.Key}: Ok\n");
                    }

                    Load = Math.Min(Load, fract);
                }
                else
                {
                    float fract = Container.AmountOf(kvPair.Key) / -kvPair.Value;

                    if (fract < 0.001f)
                    {
                        ResourceStatus.AddText($"{kvPair.Key}: Empty\n");
                    }
                    else if (fract < 1.0f)
                    {
                        ResourceStatus.AddText($"{kvPair.Key}: Limited\n");
                    }
                    else
                    {
                        ResourceStatus.AddText($"{kvPair.Key}: Ok\n");
                    }

                    Load = Math.Min(Load, fract);
                }
            }

            if (Load < 0)
            {
                Load = 0;
            }

            float toProcess = Load * (float)delta;

            foreach (var kvPair in Conversion.Amounts)
            {
                Container.Contents[kvPair.Key] += toProcess * kvPair.Value;
            }

            LoadBar.Value = Load * 100;
        }
    }
}
