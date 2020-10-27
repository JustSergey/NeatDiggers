using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    class PlayerPrepareInfo
    {
        public string Prefix;
        public string Name;
        public CharacterInfo Character;
        public bool IsReady;
    }

    class PlayerInfo
    {
        public string Prefix;
        public string Name;
        public CharacterInfo Character;
        public InventoryInfo Inventory;
    }

    class Player
    {
        bool isReady;
        string prefix;
        string name;
        Character character;
        Inventory inventory;

        public Player(string prefix, string name)
        {
            this.prefix = prefix;
            this.name = name;
            character = new EmptyCharacter();
            inventory = new Inventory();
            isReady = false;
        }

        public void SetCharacter(Character character) => this.character = character;

        public void SetReady() => isReady = !isReady;

        public PlayerPrepareInfo GetPrepareInfo() =>
            new PlayerPrepareInfo
            {
                Prefix = prefix,
                Name = name,
                Character = character.GetInfo(),
                IsReady = isReady
            };

        public PlayerInfo GetInfo() =>
            new PlayerInfo
            {
                Prefix = prefix,
                Name = name,
                Character = character.GetInfo(),
                Inventory = inventory.GetInfo()
            };
    }
}
