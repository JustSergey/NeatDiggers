using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public class Room
    {
        public bool IsStarted { get; private set; }
        public string Code { get; }
        public Dictionary<string, Player> Players { get; }
        public GameMap GameMap { get; }

        public Room(string code, GameMap gameMap)
        {
            IsStarted = false;
            Code = code;
            GameMap = gameMap;
            Players = new Dictionary<string, Player>();
        }

        public bool AddPlayer(string id, string name)
        {
            if (Players.ContainsKey(id))
                return false;
            if (Players.Count >= GameMap.SpawnPoints.Count)
                return false;
            Vector position = GameMap.SpawnPoints[Players.Count];
            Player player = new Player(id, name, position);
            Players.Add(id, player);
            return true;
        }

        public Player GetPlayer(string id)
        {
            if (Players.ContainsKey(id))
                return Players[id];
            return null;
        }

        public bool Start()
        {
            if (Players.All(p => p.Value.IsReady && p.Value.Character.Name != CharacterName.Empty))
                IsStarted = true;
            return IsStarted;
        }
    }
}
