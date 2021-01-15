using NeatDiggers.GameServer.Decks;
using NeatDiggers.GameServer.Maps;
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
        static Dictionary<string, string> users = new Dictionary<string, string>();
        static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public static string CreateRoom(GameMap gameMap, Deck deck)
        {
            Random random = new Random();
            string code;
            do
            {
                code = "";
                for (int i = 0; i < codeLength; i++)
                    code += (char)random.Next('A', 'Z' + 1);
            } while (rooms.ContainsKey(code));

            rooms.Add(code, new Room(code, gameMap, deck));
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
            if (room.Players.Count == 0)
                return rooms.Remove(room.Code);
            return false;
        }

        public static bool AddUser(string id, string code)
        {
            if (!users.ContainsKey(id))
            {
                users.Add(id, code);
                return true;
            }
            return false;
        }

        public static bool RemoveUser(string id) => users.Remove(id);

        public static Room GetRoomByUserId(string id)
        {
            if (users.ContainsKey(id))
                return GetRoom(users[id]);
            return null;
        }
    }
}