using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public enum GameMapType
    {
        Standart,
        Diagonal,
        Large,
        Custom
    }

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
        public Cell[][] Map { get; protected set; }

        public static GameMap Parse(string stringMap)
        {
            string[] grid = stringMap.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int height = grid.Length;
            if (height < 9)
                return null;
            int width = grid[0].Length;
            if (width < 9)
                return null;
            for (int i = 1; i < grid.Length; i++)
            {
                if (grid[i].Length != width)
                    return null;
            }
            var spawnPoints = new List<Vector>();
            var map = new Cell[width][];
            for (int i = 0; i < width; i++)
                map[i] = new Cell[height];
            var flagSpawnPoint = new Vector(-1, -1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[y][x] == 'N')
                        map[x][y] = Cell.None;
                    else if (grid[y][x] == 'E')
                        map[x][y] = Cell.Empty;
                    else if (grid[y][x] == 'D')
                        map[x][y] = Cell.Digging;
                    else if (grid[y][x] == 'F')
                    {
                        map[x][y] = Cell.Empty;
                        flagSpawnPoint = new Vector(x, y);
                    }
                    else if (grid[y][x] == 'W')
                        map[x][y] = Cell.Wall;
                    else if (grid[y][x] == 'S')
                    {
                        map[x][y] = Cell.Empty;
                        spawnPoints.Add(new Vector(x, y));
                    }
                }
            }
            if (flagSpawnPoint.Equals(new Vector(-1, -1)))
                return null;
            if (spawnPoints.Count < 2)
                return null;

            return new GameMap
            {
                Width = width,
                Height = height,
                SpawnPoints = spawnPoints,
                FlagSpawnPoint = flagSpawnPoint,
                Map = map
            };
        }
    }
}
