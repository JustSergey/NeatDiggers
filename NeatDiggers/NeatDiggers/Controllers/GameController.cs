using Microsoft.AspNetCore.Mvc;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Decks;
using NeatDiggers.GameServer.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Matches()
        {
            List<Room> rooms = Server.GetRooms();
            return View(rooms);
        }

        public IActionResult CreateLobby() 
        {
            string code = Server.CreateRoom(new StandartGameMap(), new StandartDeck());
            return RedirectToAction("Watch", "Game", new { code });
        }

        public IActionResult Watch(string code)
        {
            Room room = Server.GetRoom(code);
            return View(room);
        }

        public IActionResult PlayersLobby(string code) 
        {
            Room room = Server.GetRoom(code);
            ViewData["CharacterNames"] = Enum.GetNames(typeof(GameServer.Characters.CharacterName));
            return View(room);
        }
    }
}
