using Microsoft.AspNetCore.Mvc;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Decks;
using NeatDiggers.GameServer.Maps;
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
            Room room = Server.GetRoom(code);
            return View(room);
        }

        public IActionResult Watch(string code)
        {
            Room room = Server.GetRoom(code);
            
            return View("./CreateLobby", room);
        }
    }
}
