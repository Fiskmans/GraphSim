using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    public static class Constants
    {
        // How many units represent 1
        public const int SimulationScale = 1000;

        // How many cycles per second
        public const int SimulationSpeed = 60;

        // How many seconds of production buffers should hold
        public const int BufferScale = 2;

        //How much 1 unit is when read from data (1 per second)
        public const int DataScale = SimulationSpeed * SimulationScale;

        public const float NodeSpacing = 8;

        public static string ScaledString(int amount)
        {
            float val = (float)amount / DataScale;

            return val.ToString("0.00");
        }
    }
}
