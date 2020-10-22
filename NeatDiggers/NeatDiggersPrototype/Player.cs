using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    class PlayerInfo
    {
        public string Prefix;
        public string Name;
        public CharacterInfo Character;
        public InventoryInfo Inventory;
    }

    class Player
    {
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
        }

        public void SetCharacter(Character character) => this.character = character;

        public PlayerInfo GetInfo()
        {
            return new PlayerInfo
            {
                Prefix = prefix,
                Name = name,
                Character = character.GetInfo(),
                Inventory = inventory.GetInfo()
            };
        }
    }
}
