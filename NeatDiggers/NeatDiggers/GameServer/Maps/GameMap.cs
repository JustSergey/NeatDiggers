using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Maps
{
    public enum Cell
    {
        None,
        Empty,
        Wall,
        Digging,
    }

    public class GameMap
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public List<Vector> SpawnPoints { get; protected set; }
        public Vector FlagSpawnPoint { get; protected set; }
        public Cell[,] Map { get; protected set; }
    }
}
