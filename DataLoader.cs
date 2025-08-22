using Godot;
using GraphSim;
using System;
using System.Text.Json;

public partial class DataLoader : Node
{
    [Export]
    string Filepath;

    public Data Data;

    public override void _EnterTree()
    {
        base._EnterTree();

        string raw = System.IO.File.ReadAllText(Filepath);

        Data = JsonSerializer.Deserialize<Data>(raw);
        GD.Print(Data.Buildings);
    }
}
