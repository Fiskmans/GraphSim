using Godot;
using GraphSim;
using GraphSim.Attributes;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Site : LogisticsHub
{
    Dictionary<GraphSim.Resource, long> SurfaceResources = new();

    List<LogisticsEndpoint> DumpingNodes = new();

    public Site()
    {
        Generate();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        foreach (LogisticsEndpoint dump in DumpingNodes)
            Dump(dump.Resource, dump.Withdraw(int.MaxValue));
    }

    public void Generate()
    {
        SurfaceResources.Add(GraphSim.Resource.Rock,        1000000 * (long)Constants.DataScale);
        SurfaceResources.Add(GraphSim.Resource.Hematite,    100000 * (long)Constants.DataScale);
        SurfaceResources.Add(GraphSim.Resource.Malachite,   70000 * (long)Constants.DataScale);
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
}
