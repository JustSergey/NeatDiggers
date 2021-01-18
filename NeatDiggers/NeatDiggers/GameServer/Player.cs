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
        public Vector SpawnPoint { get; set; }
        public Vector Position { get; set; }
        public Character Character { get; set; }
        public Inventory Inventory { get; set; }
        public int Level { get; private set; }
        public int Speed { get; set; }
        public int Health { get; set; }
        public int MeleeDamage { get; set; }
        public int RangedDamage { get; set; }
        public double MultiplyDamage { get; set; }
        public int MeleeDistance { get; set; }
        public int RangedDistance { get; set; }
        public int DigPower { get; set; }
        public int Hands { get; set; }
        public int Armor { get; set; }
        public int Score { get; set; }
        public List<Effect> Effects { get; set; }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
            IsReady = false;
            IsTurn = false;
            WithFlag = false;
            Character = new EmptyCharacter();
            Level = 0;
            Speed = 0;
            MeleeDistance = 0;
            RangedDistance = 0;
            DigPower = 0;
            MeleeDamage = 0;
            RangedDamage = 0;
            MultiplyDamage = 1.0;
            Hands = 2;
            Armor = 0;
            Score = 0;
            Effects = new List<Effect>();
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

        public void Respawn()
        {
            Health = Character.MaxHealth;
            Inventory = new Inventory();
            Position = SpawnPoint;
        }

        public void SetTurn()
        {
            List<Effect> effects = new List<Effect>(Effects.Count);
            foreach (Effect effect in Effects)
            {
                effect.Duration--;
                if (effect.Duration > 0)
                    effects.Add(effect);
                else
                    effect.Cancel(this);
            }
            Effects = effects;
            IsTurn = true;
        }

        public void EndTurn()
        {
            IsTurn = false;
        }
    }
}
