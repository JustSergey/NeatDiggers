using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Characters;
using NeatDiggers.GameServer.Decks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer
{
    public class Room
    {
        public bool IsStarted { get; private set; }
        public string Code { get; }
        public List<Player> Players { get; }
        public List<string> Spectators { get; }
        public int PlayerTurn { get; private set; }
        public Vector FlagPosition { get; set; }
        public bool FlagOnTheGround { get; set; }
        public int Round { get; private set; }
        public Player Winner { get; private set; }
        private GameMap gameMap;
        private int scoreToWin;
        
        Deck deck;
        List<Item> items;
        int nextItem;

        public Room(string code, GameMap gameMap, Deck deck, int scoreToWin)
        {
            IsStarted = false;
            Code = code;
            this.gameMap = gameMap;
            this.scoreToWin = scoreToWin;
            Players = new List<Player>();
            Spectators = new List<string>();
            Round = 0;
            this.deck = deck;
            items = deck.Shuffle();
            nextItem = items.Count - 1;
            FlagOnTheGround = true;
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
                FlagPosition = gameMap.FlagSpawnPoint;
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
            Players[PlayerTurn].EndTurn();
            PlayerTurn++;
            if (PlayerTurn >= Players.Count)
            {
                PlayerTurn = 0;
                Round++;
            }
            Players[PlayerTurn].SetTurn();
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

        public bool TryTakeTheFlag(Player player)
        {
            if (FlagOnTheGround && player.Position.Equals(FlagPosition))
            {
                if (!player.WithFlag && player.Inventory.Items.Count < 6)
                {
                    FlagOnTheGround = false;
                    player.WithFlag = true;
                    player.Speed -= 2;
                    player.Effects.Add(new Effect(p => p.Speed += 2)
                    {
                        Title = "Флаг (скорость -2)",
                        Duration = -1
                    });
                    return true;
                }
            }
            return false;
        }

        public void DropTheFlag(Player player)
        {
            if (player.WithFlag)
            {
                FlagPosition = player.Position;
                player.WithFlag = false;
                FlagOnTheGround = true;
                Effect flagEffect = player.Effects.Find(e => e.Title == "Флаг (скорость -2)");
                if (flagEffect != null)
                {
                    flagEffect.Cancel(player);
                    player.Effects.Remove(flagEffect);
                }
            }
        }

        public bool CheckWinner(Player player)
        {
            if (player.Score >= scoreToWin)
            {
                Winner = player;
                return true;
            }
            return false;
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
