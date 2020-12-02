using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer;

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
                    Func<Room> action = gameAction.Type switch
                    {
                        GameActionType.Move => () => Move(room, player, gameAction.TargetPosition),
                        GameActionType.Dig => () => Dig(room, player),
                        GameActionType.Attack => () => Attack(room, player, gameAction.TargetPlayer),
                        GameActionType.UseItem => () => UseItem(room, player, gameAction.Item, gameAction.TargetPlayer, gameAction.TargetPosition),
                        GameActionType.DropItem => () => DropItem(room, player, gameAction.Item),
                        GameActionType.UseAbility => () => UseAbility(room, player, gameAction.TargetPlayer, gameAction.TargetPosition),
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

        public Room Move(Room room, Player currentPlayer, Vector targetPosition)
        {
            return room;
        }

        public Room Dig(Room room, Player currentPlayer)
        {
            return room;
        }

        public Room Attack(Room room, Player currentPlayer, Player targetPlayer)
        {
            return room;
        }

        public Room UseItem(Room room, Player currentPlayer, Item item, Player targetPlayer, Vector targetPosition)
        {
            return room;
        }

        public Room DropItem(Room room, Player currentPlayer, Item item)
        {
            return room;
        }

        public Room UseAbility(Room room, Player currentPlayer, Player targetPlayer, Vector targetPosition)
        {
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
