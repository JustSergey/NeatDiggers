using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Maps;

namespace NeatDiggers.GameServer
{
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y) => (X, Y) = (x, y);

        public bool CheckAvailability(Vector targetPoint, int radius)
        {
            int x = targetPoint.X - X;
            int y = targetPoint.Y - Y;
            return radius >= (int)Math.Round(Math.Sqrt(x * x + y * y));
        }

        public bool IsInMap(GameMap map)
        {
            return X < map.Width && X >= 0 && Y < map.Height && Y >= 0;
        }
    }
}
