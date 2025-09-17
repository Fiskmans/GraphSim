using GraphSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class Catalyst : ResourceBar
{
    GraphSim.Resource Type;
    float Target;

    public Catalyst(GraphSim.Resource type, float max)
    {
        Type = type;

        Max = max;
        Target = max;
        Value = 0;

        Label = Type.ToString();
    }

    public void Refill(Container container)
    {
        float wants = Target - Value;
        float available = container.AmountOf(Type);

        float take = Math.Min(wants, available);

        Value += take;
        container.Remove(Type, take);
    }

    public void Consume(float amount)
    {
        Value -= amount;
    }
}