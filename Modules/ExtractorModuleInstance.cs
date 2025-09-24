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

        Size = new Vector2(50, 50);
        SetProcessInput(false);

        Output = new LogisticsEndpoint { Resource = module.Resource, Capacity = module.Multiplier * Constants.DataScale * 10, Mode = LogisticsMode.Produces };
        AddChild(Output);

        UI = new VBoxContainer();

        UI.AddChild(new ResourceBar { Node = Output });
    }

    public override int DoWork(int budget)
    {
        int target = (int)(Output.SpaceFraction * Constants.SimulationScale);

        int work = budget;

        if (work > target)
            work = target;

        Animation += (float)work / Constants.SimulationScale / Constants.SimulationSpeed * 2.0f;
        Output.Deposit(Module.Multiplier * target);

        return work;
    }

    public override void _Draw()
    {
        int horizontalPadding = 10;
        int verticalPadding = 10;

        Vector2 topLeft = new Vector2(horizontalPadding, verticalPadding);
        Vector2 center = new Vector2(Size.X * 0.5f, Size.Y - verticalPadding);
        Vector2 topRight = new Vector2(Size.X - horizontalPadding, verticalPadding);

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
