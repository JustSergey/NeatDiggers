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

        public static Room CreateRoom(GameMap gameMap)
        {
            Random random = new Random();
            string code;
            do
            {
                code = "";
                for (int i = 0; i < codeLength; i++)
                    code += (char)random.Next('A', 'Z' + 1);
            } while (rooms.ContainsKey(code));

            Room room = new Room(code, gameMap);
            rooms.Add(code, room);
            return room;
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