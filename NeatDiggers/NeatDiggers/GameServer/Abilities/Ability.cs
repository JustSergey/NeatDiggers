using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public enum AbilityName
    {
        Speed
    }

    public enum AbilityType
    {
        Passive,
        Active
    }

    public class Ability
    {
        public AbilityName Name { get; protected set; }
        public string Description { get; protected set; }
        public AbilityType Type { get; protected set; }

        public virtual void Use(Room room, GameAction gameAction) { }
    }
}
