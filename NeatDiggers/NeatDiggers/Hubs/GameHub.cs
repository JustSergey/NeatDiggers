using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Characters;
using NeatDiggers.GameServer.Items;
using Microsoft.AspNetCore.Authorization;

namespace NeatDiggers.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task ConnectToRoomAsSpectator(string code)
        {
            Room room = Server.AddUser(null, Context.ConnectionId, code, true);
            if (room != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
                await Clients.Group(code).ChangeState(room);
            }
        }

        public async Task<string> ConnectToRoom(string code, string name)
        {
            Room room = Server.AddUser(name, Context.ConnectionId, code, false);
            if (room != null && !room.IsStarted)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, code);
                await Clients.Group(code).ChangeState(room);
                return Context.ConnectionId;
            }

            return null;
        }

        public async Task ChangeCharacter(CharacterName characterName)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && !player.IsReady)
                {
                    player.ChangeCharacter(characterName);
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
        }

        public async Task ChangeReady()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null)
                {
                    player.ChangeReady();
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
        }

        public async Task StartGame()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && !room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player != null && room.Start())
                    await Clients.Group(room.Code).ChangeState(room);
            }
        }

        public int RollTheDice()
        {
            Random random = new Random();
            int dice = random.Next(1, 7);
            Context.Items["Dice"] = dice;
            return dice;
        }

        public async Task DoAction(GameAction gameAction)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
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
                        await Clients.Group(room.Code).ChangeStateWithAction(room, gameAction);
                    }
                }
            }
        }

        private Room Move(Room room, GameAction gameAction)
        {
            int diceRollResult = (int) Context.Items["Dice"];
            Vector playerPosition = gameAction.CurrentPlayer.Position;
            Vector targetPosition = gameAction.TargetPosition;
            if (playerPosition.CheckAvailability(targetPosition, diceRollResult))
                gameAction.CurrentPlayer.Position = targetPosition;
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
            int diceRollResult = (int) Context.Items["Dice"];
            Vector playerPosition = gameAction.CurrentPlayer.Position;
            int playerAttackRadius = gameAction.CurrentPlayer.AttackRadius;
            Vector targetPosition = gameAction.TargetPosition;
            if (playerPosition.CheckAvailability(targetPosition, playerAttackRadius))
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

        public async Task EndTurn()
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null && room.IsStarted)
            {
                Player player = room.GetPlayer(Context.ConnectionId);
                if (player.IsTurn)
                {
                    room.NextTurn();
                    await Clients.Group(room.Code).ChangeState(room);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Room room = Server.GetRoomByUserId(Context.ConnectionId);
            if (room != null)
            {
                room.Disconnect(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Code);
                await Clients.Group(room.Code).ChangeState(room);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}