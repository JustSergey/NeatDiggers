using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    class Server
    {
        Dictionary<int, User> users;
        Dictionary<string, Room> rooms;

        public Server()
        {
            users = new Dictionary<int, User>();
            rooms = new Dictionary<string, Room>();
        }

        public UserInfo ConnectToServer(string name)
        {
            User user = new User(name);
            UserInfo userInfo = user.GetInfo();
            users.Add(userInfo.Id, user);
            return userInfo;
        }

        public RoomPrepareInfo CreateRoom(int userId)
        {
            if (users.TryGetValue(userId, out User user))
            {
                Room room = new Room(user);
                rooms.Add(room.GetCode(), room);
                return room.GetPrepareInfo();
            }
            return null;
        }

        public RoomPrepareInfo ConnectToRoom(string code, int userId)
        {
            if (users.TryGetValue(userId, out User user))
            {
                if (rooms.TryGetValue(code, out Room room))
                {
                    room.AddUser(user);
                    return room.GetPrepareInfo();
                }
            }
            return null;
        }

        public void CloseRoom(string code, int creatorId)
        {
            if (rooms.TryGetValue(code, out Room room))
                if (room.GetCreatorId() == creatorId)
                    rooms.Remove(code);
        }

        public bool ChangeCharacter(string roomCode, int userId, CharacterName characterName)
        {
            if (users.ContainsKey(userId))
                if (rooms.TryGetValue(roomCode, out Room room))
                    return room.ChangeCharacter(userId, characterName);
            return false;
        }

        public bool SetReady(string roomCode, int userId)
        {
            if (users.ContainsKey(userId))
                if (rooms.TryGetValue(roomCode, out Room room))
                    return room.SetReady(userId);
            return false;
        }

        public bool StartTheGame(string roomCode, int creatorId)
        {
            if (users.ContainsKey(creatorId))
                if (rooms.TryGetValue(roomCode, out Room room))
                    return room.StartTheGame(creatorId);
            return false;
        }

        public void Update()
        {

        }
    }
}
