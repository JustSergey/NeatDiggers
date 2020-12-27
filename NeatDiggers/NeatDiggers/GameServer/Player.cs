using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Characters;

namespace NeatDiggers.GameServer
{
    public class Player
    {
        public string Id { get; }
        public string Name { get; }
        public bool IsReady { get; private set; }
        public bool IsTurn { get; set; }
        public bool WithFlag { get; set; }
        public Vector SpawnPoint { get; }
        public Vector Position { get; set; }
        public Character Character { get; set; }
        public Inventory Inventory { get; set; }
        public int Level { get; private set; }
        public int Speed { get; set; }
        public int Health { get; set; }
        public int MeleeDamage { get; set; }
        public int RangedDamage { get; set; }
        public int AttackRadius { get; set; }
        public int DigPower { get; set; }
        public int Hands { get; set; }

        public Player(string id, string name, Vector position)
        {
            Id = id;
            Name = name;
            IsReady = false;
            IsTurn = false;
            WithFlag = false;
            SpawnPoint = position;
            Position = position;
            Character = new EmptyCharacter();
            Inventory = new Inventory();
            Level = 0;
            LevelUp();
            Speed = 0;
            AttackRadius = 0;
            DigPower = 0;
            MeleeDamage = 0;
            RangedDamage = 0;
            Hands = 2;
        }

        public void LevelUp()
        {
            if (Character.Abilities.Count > Level && 
                Character.Abilities[Level].Type == Abilities.AbilityType.Passive)
            {
                Character.Abilities[Level].Get(this);
                Character.Abilities[Level].IsActive = true;
            }
            Level++;
        }

        public void ChangeCharacter(CharacterName characterName)
        {
            Character newCharacter = Character.CreateCharacter(characterName);
            if (newCharacter != null)
                Character = newCharacter;
        }

        public void ChangeReady()
        {
            if (IsReady)
                IsReady = false;
            else if (Character.Name != CharacterName.Empty)
                IsReady = true;
        }
    }
}
