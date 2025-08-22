using Godot;
using System;
using System.Collections.Generic;

public partial class ResourceIntake : Node2D
{
    [Export]
    Godot.Collections.Array<Node> Sources = new();
}
