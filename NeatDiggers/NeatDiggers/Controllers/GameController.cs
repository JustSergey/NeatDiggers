using Microsoft.AspNetCore.Mvc;
using NeatDiggers.GameServer;
using NeatDiggers.GameServer.Decks;
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
            DeckType deckType = DeckType.Standart;
            GameMap gameMap = (GameMapType)map switch
            {
                GameMapType.Diagonal => LoadMap("DiagonalGameMap.txt"),
                GameMapType.Large => LoadMap("LargeGameMap.txt"),
                GameMapType.Custom => GameMap.Parse("CustomGameMapContent"),
                _ => null,
            };
            if (gameMap == null)
                gameMap = LoadMap("StandartGameMap.txt");

            Deck deck = deckType switch
            {
                DeckType.Custom => Deck.Parse("CustomDeckContent"),
                _ => null
            };
            if (deck == null)
                deck = LoadDeck("StandartDeck.txt");

            string code = Server.CreateRoom(gameMap, deck, maxScore);
            return RedirectToAction("Watch", "Game", new { code });
        }

        private GameMap LoadMap(string path) => GameMap.Parse(System.IO.File.ReadAllText(path));
        private Deck LoadDeck(string path) => Deck.Parse(System.IO.File.ReadAllText(path));

        public IActionResult Watch(string code)
        {
            Room room = Server.GetRoom(code);
            return View(room);
        }

        public IActionResult PlayersLobby(string code, string name, string token) 
        {
            code = code.ToUpper();
            if (token == null)
            {
                token = Server.ConnectToRoom(code);
                return RedirectToAction("PlayersLobby", "Game", new { code, name, token });
            }
            ViewData["CharacterNames"] = Enum.GetNames(typeof(GameServer.Characters.CharacterName));
            return View(new User { Code = code, Name = name, Token = token });
        }
    }
}
