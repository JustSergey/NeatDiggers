using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeatDiggersPrototype
{
    class RoomInfo
    {
        public string Code;
        public List<PlayerInfo> Players;
    }

    class Room
    {
        const int code_length = 5;

        string code;
        int creatorId;
        Dictionary<int, Player> players;
        Random random;

        public string GetCode() => code;

        public int GetCreatorId() => creatorId;

        public Room(User creator)
        {
            UserInfo userInfo = creator.GetInfo();
            creatorId = userInfo.Id;
            random = new Random();
            code = GenerateCode(random, code_length);
            players = new Dictionary<int, Player>
            {
                { creatorId, new Player("Creator", userInfo.Name) }
            };
        }

        string GenerateCode(Random random, int codeLength)
        {
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < codeLength; i++)
                code.Append((char)random.Next('A', 'Z' + 1));
            return code.ToString();
        }

        public RoomInfo GetInfo()
        {
            return new RoomInfo
            {
                Code = code,
                Players = players.Values.Select(p => p.GetInfo()).ToList()
            };
        }

        public void AddUser(User user)
        {
            UserInfo userInfo = user.GetInfo();
            players.Add(userInfo.Id, new Player("", userInfo.Name));
        }

        public bool ChangeCharacter(int userId, CharacterName characterName)
        {
            Character character = Character.CreateCharacter(characterName);
            if (players.TryGetValue(userId, out Player player))
            {
                player.SetCharacter(character);
                return true;
            }
            return false;
        }
    }
}
