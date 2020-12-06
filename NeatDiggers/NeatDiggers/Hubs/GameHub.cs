using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Characters;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.Hubs
{
    public class GameHub : Hub
    {
        public async Task<Room> ConnectToRoom(string code, string name)
        {
            Room room = Server.GetRoom(code);
            if (room != null && !room.IsStarted)
            {
                if (room.AddPlayer(Context.ConnectionId, name))
                {
                    await Clients.Group(code).SendAsync("UserConnected", name);
                    await Groups.AddToGroupAsync(Context.ConnectionId, code);
                    return room;
                }
            }
            return null;
        }

        public async Task ChangeCharacter(string code, CharacterName characterName)
        {
            Room room = Server.GetRoom(code);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && !player.IsReady)
                {
                    player.ChangeCharacter(characterName);
                    await Clients.Group(code).SendAsync("ChangeState", room);
                }
            }
        }

        public async Task ChangeReady(string code)
        {
            Room room = Server.GetRoom(code);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null)
                {
                    player.ChangeReady();
                    await Clients.Group(code).SendAsync("ChangeState", room);
                }
            }
        }

        public async Task StartGame(string code)
        {
            Room room = Server.GetRoom(code);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && room.Start())
                    await Clients.Group(code).SendAsync("ChangeState", room);
            }
        }

        public int RollTheDice()
        {
            Random random = new Random();
            int dice = random.Next(1, 7);
            Context.Items["Dice"] = dice;
            return dice;
        }

        public async Task DoAction(string code, GameAction gameAction)
        {
            Room room = Server.GetRoom(code);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player.IsTurn)
                {
                    gameAction.CurrentPlayer = player;
                    Func<Room> action = gameAction.Type switch
                    {
                        GameActionType.Move => () => Move(room, gameAction),
                        GameActionType.Dig => () => Dig(room, player),
                        GameActionType.Attack => () => Attack(room, gameAction),
                        GameActionType.UseItem => () => UseItem(room, gameAction),
                        GameActionType.DropItem => () => DropItem(room, gameAction),
                        GameActionType.UseAbility => () => UseAbility(room, gameAction),
                        _ => null
                    };
                    if (action != null)
                    {
                        room = action();
                        await Clients.Group(code).SendAsync("ChangeStateWithAction", room, gameAction);
                    }
                }
            }
        }

        private Room Move(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.Position = gameAction.TargetPosition;
            return room;
        }

        private Room Dig(Room room, Player currentPlayer)
        {
            int diceRollResult = (int) Context.Items["Dice"];
            if (diceRollResult % 2 == 0)
            {
                int i = diceRollResult / 2;
                while (diceRollResult > 0)
                {
                    Item dugItem = room.Dig();
                    room.GetPlayer(currentPlayer.Id).Inventory.Items.Add(dugItem);
                    diceRollResult -= 1;
                }

                return room;
            }
            else
                return room;
        }

        private Room Attack(Room room, GameAction gameAction)
        {
            gameAction.TargetPlayer.Health -= gameAction.CurrentPlayer.Damage;
            return room;
        }

        private Room UseItem(Room room, GameAction gameAction)
        {
            gameAction.Item.Use(room, gameAction);
            return room;
        }

        private Room DropItem(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.Inventory.Items.Remove(gameAction.Item);
            gameAction.CurrentPlayer.Inventory.Drop++;
            return room;
        }

        private Room UseAbility(Room room, GameAction gameAction)
        {
            gameAction.Ability.Use(room, gameAction);
            return room;
        }

        public async Task EndTurn(string code)
        {
            Room room = Server.GetRoom(code);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player.IsTurn)
                {
                    room.NextTurn();
                    await Clients.Group(code).SendAsync("ChangeState", room);
                }
            }
        }
    }
}
