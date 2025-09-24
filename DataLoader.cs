using Godot;
using GraphSim.Data;
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

        JsonSerializerOptions options = new JsonSerializerOptions();

        options.Converters.Add(new PolymorphicJsonConverter<Module>());
        options.Converters.Add(new EnumJsonConverter<GraphSim.Resource>());

        Data = JsonSerializer.Deserialize<Data>(file.GetAsText(), options);

        file.Close();
    }
}
