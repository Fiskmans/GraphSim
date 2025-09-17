using Godot;
using GraphSim;
using System;

public partial class WorldSlot : PanelContainer
{
    SlotItem _Content;
    public SlotItem Content
    {
        get => _Content;
        set {
            if (_Content != null)
                this.RemoveChild(_Content);
            this._Content = value;
            this.AddChild(_Content);
        }
    }


    public WorldSlot()
    {
        CustomMinimumSize = new Vector2(50, 50);
    }
}
