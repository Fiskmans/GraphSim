using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Enums
{
    public enum Direction
    {
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
        North,
        NorthEast
    }
    public static class DirectionExtensions
    {
        public static Vector2I Offset(this Direction direction)
        {
            int x = 0;
            int y = 0;

            switch (direction)
            {
                case Direction.NorthEast:
                case Direction.East:
                case Direction.SouthEast:
                    x = 1;
                    break;
                case Direction.SouthWest:
                case Direction.West:
                case Direction.NorthWest:
                    x = -1;
                    break;
            }

            switch (direction)
            {
                case Direction.NorthEast:
                case Direction.North:
                case Direction.NorthWest:
                    y = -1;
                    break;
                case Direction.SouthWest:
                case Direction.South:
                case Direction.SouthEast:
                    y = 1;
                    break;
            }

            return new Vector2I(x, y);
        }

        public static int StepsTo(this Direction self, Direction other)
        {
            return int.Min(int.Abs((int)self - (int)other), int.Abs((int)other - (int)self));
        }

        public static Direction Reversed(this Direction direction)
        {
            return (Direction)(((int)direction + 4) % 8);
        }

        public static Direction Next(this Direction direction)
        {
            return (Direction)(((int)direction + 1) % 8);
        }

        public static Direction Prev(this Direction direction)
        {
            return (Direction)(((int)direction + 7) % 8);
        }
    }
}
