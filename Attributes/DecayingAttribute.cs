using Godot;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Attributes
{
    internal class DecayingAttribute : Attribute
    {
        double RemainPerStep;

        public enum TimeScale
        {
            Ticks,
            Seconds,
            Minutes,
            Hours,
            Days
        }

        public DecayingAttribute(float halfLife, TimeScale scale = TimeScale.Seconds) 
        {
            double actualHalfLife = halfLife;

            switch (scale)
            {
                case TimeScale.Days:
                    actualHalfLife *= 24;
                    goto case TimeScale.Hours;

                case TimeScale.Hours:
                    actualHalfLife *= 60;
                    goto case TimeScale.Minutes;

                case TimeScale.Minutes:
                    actualHalfLife *= 60;
                    goto case TimeScale.Seconds;

                case TimeScale.Seconds:
                    actualHalfLife *= 60;
                    goto case TimeScale.Ticks;

                case TimeScale.Ticks:
                    break;
            }


            double decayConstant = Math.Log(2.0) / actualHalfLife;
            RemainPerStep = Math.Pow(double.E, -decayConstant);
        }

        public int ApplyDecay(int amount)
        {
            double remainder = amount * RemainPerStep;

            if (GD.Randf() < remainder.Fract())
                return (int)Math.Ceiling(remainder);
            else
                return (int)Math.Truncate(remainder);
        }
    }
}
