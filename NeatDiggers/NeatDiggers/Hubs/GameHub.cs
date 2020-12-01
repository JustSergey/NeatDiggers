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
            Context.Items["Name"] = name;
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

        public void DoAction(ActionType actionType, Player targetPlayer, Vector targetPosition)
        {

        }
    }
}
