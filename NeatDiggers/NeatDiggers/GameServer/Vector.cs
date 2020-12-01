using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y) => (X, Y) = (x, y);
    }
}
