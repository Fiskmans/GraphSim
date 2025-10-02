using Godot;
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

public partial class ReplaceWith : Node
{
    [Export]
    public string Assembly = System.Reflection.Assembly.GetAssembly(typeof(ReplaceWith)).FullName;

    [Export]
    public string Class;

    [Export]
    public int Count = 0;

    public override void _Process(double delta)
    {
        QueueFree();

        for (int i = 0; i < Count; i++)
        {
            ObjectHandle handle = Activator.CreateInstance(Assembly, Class);

            if (handle == null)
            {
                GD.PrintErr($"Failed to instansiate {Class} from {Assembly}");
                return;
            }

            object obj = handle.Unwrap();
            if (!(obj is Node))
            {
                GD.PrintErr($"{Class} does not derive from Node");
                return;
            }

            AddSibling(obj as Node);
        }
    }
}
