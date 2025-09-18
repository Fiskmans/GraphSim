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

        FileAccess file = FileAccess.Open(Filepath, FileAccess.ModeFlags.Read);

        Data = JsonSerializer.Deserialize<Data>(file.GetAsText());

        file.Close();
    }
}
