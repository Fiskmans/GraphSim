using Godot;
using GraphSim;
using GraphSim.Attributes;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = Godot.Color;

[Tool]
public partial class Site : LogisticsHub
{
    public enum GridNode
    {
        Unstable,
        Stable,
        Busy
    }

    [Export]
    public int MapWidth = 100;

    [Export()]
    public int MapHeight = 100;

    Construction _Construction;

    Dictionary<GraphSim.Resource, long> SurfaceResources = new();

    public GridNode[,] Map { get; private set; }
    public Construction Construction
    {
        get => _Construction;
        set
        {
            _Construction = value;
            if (_Construction != null)
                _Construction.Site = this;
            QueueRedraw();
        }
    }

    int ModificationProgress = 0;
    List<IMapModifier> Modifications = new();

    public void AddModification(IMapModifier modifier) => Modifications.Add(modifier);
    public void RemoveModification(IMapModifier modifier)
    {
        if (Modifications.Remove(modifier))
            ModificationProgress--;
    }

    public Vector2 SnapToGrid(Vector2 pos)
    {
        return pos.Snapped(Constants.NodeSpacing);
    }

    public Vector2I GridCoordsAt(Vector2 pos)
    {
        return (Vector2I)((pos + new Vector2(0.5f, 0.5f)) / Constants.NodeSpacing);
    }

    void Generate()
    {
        if (Engine.IsEditorHint())
            return;

        SurfaceResources.Add(GraphSim.Resource.Rock,        1000000 * (long)Constants.DataScale);
        SurfaceResources.Add(GraphSim.Resource.Hematite,    100000 * (long)Constants.DataScale);
        SurfaceResources.Add(GraphSim.Resource.Malachite,   70000 * (long)Constants.DataScale);

        Map = new GridNode[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Map[x, y] = GridNode.Stable;
            }
        }

        QueueRedraw();
    }

    public override void _EnterTree()
    {
        Generate();
    }

    public void Dump(GraphSim.Resource resource, int amount)
    {
        if (!SurfaceResources.ContainsKey(resource))
            SurfaceResources.Add(resource, 0);

        SurfaceResources[resource] += amount;
    }

    public struct Extraction
    {
        public GraphSim.Resource Resource;
        public int Amount;
    }

    public Extraction Extract(int amount)
    {
        long total = SurfaceResources.Values.Sum();
        float selection = GD.Randf();
        double fraction = 0;
        foreach (var kvPair in SurfaceResources)
        {
            fraction += (double)kvPair.Value / total;

            int result = (int)Math.Min(kvPair.Value, amount);

            SurfaceResources[kvPair.Key] = kvPair.Value - result;

            if (fraction > selection)
                return new Extraction { Resource = kvPair.Key, Amount = result };
        }

        return new Extraction { Resource = GraphSim.Resource.Rock, Amount = 0 };
    }

    public override void _Process(double delta)
    {
        if (ModificationProgress < Modifications.Count)
        {
            try
            {
                IEnumerable<Rect2I> blocks = Modifications[ModificationProgress].GetBlockedRegions();
                if (blocks == null)
                    return;

                GD.Print($"{Modifications[ModificationProgress]} Returned: {blocks.Count()} blocks");
                ModificationProgress++;

                foreach (Rect2I rect in blocks)
                {
                    Vector2I topLeft = rect.Position;
                    Vector2I bottomRight = topLeft + rect.Size;

                    topLeft = topLeft.Max(new Vector2I(0, 0));
                    bottomRight = bottomRight.Min(new Vector2I(MapWidth, MapHeight));

                    for (int x = topLeft.X; x < bottomRight.X; x++)
                        for (int y = topLeft.Y; y < bottomRight.Y; y++)
                            Map[x, y] = GridNode.Busy;

                    QueueRedraw();
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"{(Modifications[ModificationProgress] as Node)?.GetPath()} Failed to get blocked regions: {e.Message}");
                Modifications.RemoveAt(ModificationProgress);
                throw;
            }
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (Construction != null)
        {
            if (@event is InputEventMouseMotion mouse)
            {
                Construction.At = mouse.Position;
                QueueRedraw();
            }

            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                {
                    ConstructionSite site = Construction.Build();
                    AddChild(site);
                    if (!Input.IsKeyPressed(Key.Shift))
                        Construction = null;
                }
            }
        }
    }

    public override void _Draw()
    {
        const float radius = 0.4f;
        Color color = new Color(1,1,1, 0.3f);

        if (Engine.IsEditorHint())
        {
            int width = (int)(Size.X / Constants.NodeSpacing);
            int height = (int)(Size.Y / Constants.NodeSpacing);

            MapWidth = width;
            MapHeight = height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawCircle(new Vector2((x + 0.5f) * Constants.NodeSpacing, (y + 0.5f) * Constants.NodeSpacing), radius * 2, color, filled: true, antialiased: true);
                }
            }

            DrawRect(new Rect2(0, 0, 80, 30), new Color(0, 0, 0));

            DrawString(ThemeDB.FallbackFont, new Vector2(3, 20), $"{width}x{height}");
        }
        else
        {
            List<Vector2> points = new List<Vector2>();

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    switch (Map[x, y])
                    {
                        case GridNode.Stable:
                            Vector2 v = new Vector2((x + 0.5f) * Constants.NodeSpacing, (y + 0.5f) * Constants.NodeSpacing);
                            points.Add(v + Vector2.Left);
                            points.Add(v + Vector2.Right);
                            break;
                        case GridNode.Busy:
                            break;
                    }
                }
            }

            DrawMultiline(points.ToArray(), color, width: radius, antialiased: true);
        }

        if (Construction != null)
        {
            Construction.Draw(this);
        }
    }
}
