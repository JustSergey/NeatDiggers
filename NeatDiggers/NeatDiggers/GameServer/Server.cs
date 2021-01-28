using NeatDiggers.GameServer.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public static class Server
    {
        const int codeLength = 5;
        const int tokenLength = 10;
        static Dictionary<string, User> users = new Dictionary<string, User>();
        static Dictionary<string, Room> rooms = new Dictionary<string, Room>();
        static Dictionary<string, List<string>> tokens = new Dictionary<string, List<string>>();
        static Random random = new Random();
        static HashSet<string> codes = new HashSet<string>();

        private static string GenerateCode(int length, bool upper)
        {
            string code;
            do
            {
                code = "";
                for (int i = 0; i < length; i++)
                    code += (char)random.Next('a', 'z' + 1);
                if (upper)
                    code = code.ToUpper();
            } while (codes.Contains(code));
            codes.Add(code);
            return code;
        }

        public static User ConnectToRoom(string code)
        {
            Room room = GetRoom(code);
            if (room != null && !room.IsStarted && !room.IsFull)
            {
                string token = GenerateCode(tokenLength, false);
                if (tokens.ContainsKey(code))
                    tokens[code].Add(token);
                else
                    tokens[code] = new List<string> { token };
                User user = new User { Token = token, Code = code };
                user.Id = user.GetId();
                return user;
            }
            return null;
        }

        public static string CreateRoom(GameMap gameMap, Deck deck, int scoreToWin)
        {
            string code = GenerateCode(codeLength, true);
            rooms.Add(code, new Room(code, gameMap, deck, scoreToWin));
            return code;
        }

        public static Room GetRoom(string code)
        {
            if (rooms.ContainsKey(code))
                return rooms[code];
            return null;
        }

        public static List<Room> GetRooms()
        {
            return rooms.Values.ToList();
        }

        public static bool RemoveRoom(string code)
        {
            return rooms.Remove(code);
        }

        public static bool RemoveEmptyRoom(Room room)
        {
            if (room.Players.All(p => p.Devices <= 0))
            {
                if (tokens.ContainsKey(room.Code))
                    tokens[room.Code].ForEach(t => codes.Remove(t));
                codes.Remove(room.Code);
                return rooms.Remove(room.Code);
            }
            return false;
        }

        public static void AddUser(string connectionId, User user) => users.Add(connectionId, user);

        public static User GetUser(string connectionId)
        {
            if (users.ContainsKey(connectionId))
                return users[connectionId];
            return null;
        }

        public static void RemoveUser(string connectionId) => users.Remove(connectionId);
    }
}