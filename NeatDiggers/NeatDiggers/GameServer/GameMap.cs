using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public enum Cell
    {
        None,
        Empty,
        Wall,
        Digging,
        Flag
    }

    public class GameMap
    {
        public int Width { get; }
        public int Height { get; }
        public List<Vector> SpawnPoints { get; }
        public Cell[,] Map { get; }
    }
}
