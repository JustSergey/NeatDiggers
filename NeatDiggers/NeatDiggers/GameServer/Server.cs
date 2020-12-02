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
        static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public static string CreateRoom(GameMap gameMap)
        {
            Random random = new Random();
            string code;
            do
            {
                code = "";
                for (int i = 0; i < codeLength; i++)
                    code.Append((char)random.Next('A', 'Z' + 1));
            } while (rooms.ContainsKey(code));

            rooms.Add(code, new Room(code, gameMap));
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
    }
}