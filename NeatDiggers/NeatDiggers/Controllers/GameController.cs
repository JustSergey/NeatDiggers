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

        public IActionResult CreateLobby(int map, int maxScore)
        {
            GameMap gameMap;
            switch ((GameMapType)map)
            {
                case GameMapType.Diagonal:
                    gameMap = new DiagonalGameMap();
                    break;
                case GameMapType.Large:
                    gameMap = new LargeGameMap();
                    break;
                default:
                    gameMap = new StandartGameMap();
                    break;
            }

            string code = Server.CreateRoom(gameMap, new StandartDeck(), maxScore);
            return RedirectToAction("Watch", "Game", new { code });
        }

        public IActionResult Watch(string code)
        {
            Room room = Server.GetRoom(code);
            return View(room);
        }

        public IActionResult PlayersLobby(string code, string name) 
        {
            Room room = Server.GetRoom(code);
            ViewData["name"] = name;
            ViewData["CharacterNames"] = Enum.GetNames(typeof(GameServer.Characters.CharacterName));
            return View(room);
        }
    }
}
