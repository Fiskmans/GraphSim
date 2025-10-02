using Godot;
using GraphSim;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Drawing;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public partial class ExtractorModuleInstance : ModuleInstance
{
    ExtractorModule Module;
    LogisticsEndpoint Output;

    double _Animation = 0;
    double Animation {
        get => _Animation;
        set
        {
            _Animation = value.Fract();
            QueueRedraw();
        }
    }

    public ExtractorModuleInstance(ExtractorModule module) : base(module)
    {
        Module = module;

        SetupInput(Module.Input);

        Output = new LogisticsEndpoint
        {
            Resource = Module.Resource,
            Capacity = Module.Speed * Constants.DataScale * Constants.BufferScale,
            Mode = LogisticsMode.Produces,
            Position = new Vector2(35, 35)
        };
        AddChild(Output);

        UI.AddChild(new ResourceBar { Node = Output });
    }

    public override int DoWork(int budget)
    {
        int cap = int.Min(GetProductionCapacity(), (int)(Output.SpaceFraction * Constants.SimulationScale));

        int cycles = int.Min(cap, budget);

        ConsumeInputs(cycles);

        Animation += (float)cycles / Constants.SimulationScale / Constants.SimulationSpeed * 2.0f;
        Output.Deposit(Module.Speed * cycles);

        return cycles;
    }

    public override void _Draw()
    {
        Control owner = this.GetFirstParentOfType<Control>();

        if (owner == null)
            return;

        Vector2 size = owner.Size - this.Position;

        int horizontalPadding = 10;
        int verticalPadding = 10;

        Vector2 topLeft = new Vector2(horizontalPadding, verticalPadding);
        Vector2 center = new Vector2(size.X * 0.5f, size.Y - verticalPadding);
        Vector2 topRight = new Vector2(size.X - horizontalPadding, verticalPadding);

        DrawMultiline(
            [
            topLeft, center, 
            center, topRight,
            topLeft, topRight,
            ], 
            new Godot.Color(1.0f,1.0f,1.0f), 
            width: 0.6f,
            antialiased: true);

        DrawMultiline(
            [
            center.Lerp(topLeft, (float)Animation * 0.3f + 0.0f), center.Lerp(topRight, (float)Animation * 0.3f + 0.1f),
            center.Lerp(topLeft, (float)Animation * 0.3f + 0.3f), center.Lerp(topRight, (float)Animation * 0.3f + 0.4f),
            center.Lerp(topLeft, (float)Animation * 0.3f + 0.6f), center.Lerp(topRight, (float)Animation * 0.3f + 0.7f),
            center.Lerp(topLeft, (float)Animation * 0.1f + 0.9f), topRight.Lerp(topLeft, (float)Animation),
            ], 
            new Godot.Color(0.8f,0.8f,0.8f), 
            width: 1.0f,
            antialiased: true);
    }
}
