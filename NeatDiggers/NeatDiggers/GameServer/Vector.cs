using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public class Vector
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

        public override int GetHashCode() => X ^ (Y << 16) ^ (Y >> 16);

        public override bool Equals(object obj)
        {
            if (obj is Vector v)
            {
                if (v.X == X && v.Y == Y)
                    return true;
            }
            return false;
        }
    }
}
