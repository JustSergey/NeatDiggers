using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Characters;
using NeatDiggers.GameServer.Decks;
using NeatDiggers.GameServer.Items;
using NeatDiggers.GameServer.Maps;

namespace NeatDiggers.GameServer
{
    public class Room
    {
        public bool IsStarted { get; private set; }
        public string Code { get; }
        public List<Player> Players { get; }
        public List<string> Spectators { get; }
        public int PlayerTurn { get; private set; }
        public int Round { get; private set; }
        private GameMap gameMap;
        
        Dictionary<(int, int), List<Action<Room>>> cancelingActions;
        Deck deck;
        List<Item> items;
        int nextItem;

        public Room(string code, GameMap gameMap, Deck deck)
        {
            IsStarted = false;
            Code = code;
            this.gameMap = gameMap;
            Players = new List<Player>();
            Spectators = new List<string>();
            Round = 0;
            cancelingActions = new Dictionary<(int, int), List<Action<Room>>>();
            this.deck = deck;
            items = deck.Shuffle();
            nextItem = items.Count - 1;
        }

        public bool AddSpectator(string id)
        {
            if (Spectators.Contains(id))
                return false;
            Spectators.Add(id);
            return true;
        }

        public bool AddPlayer(string id, string name)
        {
            if (Players.Any(p => p.Id == id))
                return false;
            if (Players.Count >= gameMap.SpawnPoints.Count)
                return false;
            Player player = new Player(id, name);
            Players.Add(player);
            return true;
        }

        public Player GetPlayer(string id) => Players.Find(p => p.Id == id);

        public bool Start()
        {
            if (Players.All(p => p.IsReady && p.Character.Name != CharacterName.Empty))
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].LevelUp();
                    Players[i].SpawnPoint = gameMap.SpawnPoints[i];
                    Players[i].Respawn();
                }
                Random random = new Random();
                PlayerTurn = random.Next(Players.Count);
                Players[PlayerTurn].IsTurn = true;
                IsStarted = true;
            }
            return IsStarted;
        }

        public GameMap GetGameMap() => gameMap;

        public void NextTurn()
        {
            Players[PlayerTurn].IsTurn = false;
            PlayerTurn++;
            if (PlayerTurn >= Players.Count)
            {
                PlayerTurn = 0;
                Round++;
            }
            Players[PlayerTurn].IsTurn = true;

            if (cancelingActions.TryGetValue((PlayerTurn, Round), out List<Action<Room>> actions))
            {
                foreach (Action<Room> action in actions)
                    action(this);
                cancelingActions.Remove((PlayerTurn, Round));
            }
        }

        public void AddCancelingAction(int playerTurn, int round, Action<Room> action)
        {
            if (cancelingActions.ContainsKey((playerTurn, round)))
                cancelingActions[(playerTurn, round)].Add(action);
            else
                cancelingActions[(playerTurn, round)] = new List<Action<Room>> { action };
        }

        public Item Dig()
        {
            if (nextItem < 0)
            {
                items = deck.Shuffle();
                nextItem = items.Count - 1;
            }
            return items[nextItem--];
        }

        public bool Disconnect(string id)
        {
            if (!Spectators.Remove(id))
            {
                Player player = Players.Find(p => p.Id == id);
                if (player != null)
                {
                    if (player.IsTurn)
                        NextTurn();
                    if (Players.IndexOf(player) < PlayerTurn)
                        PlayerTurn--;
                    Players.Remove(player);
                    return true;
                }
            }
            return false;
        }
    }
}
