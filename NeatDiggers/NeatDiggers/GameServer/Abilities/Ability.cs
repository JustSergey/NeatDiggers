using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public enum AbilityName
    {
        Speed,
        Shuriken,
        Heal,
        Damage,
        Distance,
        Armor,
        Invul,
        ArmorBreak
    }

    public enum AbilityType
    {
        Passive,
        Active
    }

    public class Ability
    {
        public AbilityName Name { get; set; }
        public string Description { get; set; }
        public AbilityType Type { get; set; }
        public Target Target { get; set; }
        public bool IsActive { get; set; }

        public virtual bool Use(Room room, GameAction gameAction) { return false; }
        public virtual void Get(Player player) { }
    }
}
