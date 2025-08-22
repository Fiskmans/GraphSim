using Godot;
using GraphSim;
using System;
using System.Collections.Generic;

public partial class Factory : VBoxContainer
{
    Building Type;
    Conversion Conversion;

    public float Load = 0;

    [Export]
    Node ContainerNode;

    ProgressBar LoadBar = new ProgressBar();

    Container Container;
    Data Data;

    OptionButton TypeSelector = new ();

    OptionButton ConverterSelector = new();

    public override void _Ready()
    {
        base._Ready();

        Data = this.GetFirstParentOfType<DataLoader>()?.Data;

        GD.Print(Data);

        Container = ContainerNode as Container;

        foreach (Building building in Data.Buildings)
        {
            TypeSelector.AddItem(building.Name);
        }

        TypeSelector.FitToLongestItem = true;
        ConverterSelector.FitToLongestItem = true;

        TypeSelector.ItemSelected += TypeSelectionChanged;
        ConverterSelector.ItemSelected += ConversionSelected;

        AddChild(LoadBar);
        AddChild(TypeSelector);
        AddChild(ConverterSelector);

        ConverterSelector.Visible = false;
    }

    private void ConversionSelected(long index)
    {
        Conversion = Type.Conversions[(int)index];
    }

    private void TypeSelectionChanged(long index)
    {
        ConverterSelector.Clear();
        Conversion = null;
        ConverterSelector.Visible = true;

        Type = Data.Buildings[(int)index];

        foreach (Conversion conversion in Type.Conversions)
        {
            ConverterSelector.AddItem(conversion.Name);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Conversion != null)
        {
            Load = 1;

            foreach (var kvPair in Conversion.Amounts)
            {
                if (kvPair.Value > 0)
                {
                    Load = Math.Min(Load, Container.SpaceFor(kvPair.Key) / kvPair.Value);
                }
                else
                {
                    Load = Math.Min(Load, Container.AmountOf(kvPair.Key) / -kvPair.Value);
                }
            }

            float toProcess = Load * (float)delta;

            foreach (var kvPair in Conversion.Amounts)
            {
                Container.Contents[kvPair.Key] += toProcess * kvPair.Value;
            }

            LoadBar.Value = Load;
        }
    }
}
