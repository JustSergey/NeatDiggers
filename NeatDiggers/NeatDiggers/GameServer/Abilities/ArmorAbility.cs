using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class ArmorAbility : Ability
    {
        int armor;

        public ArmorAbility(int armor)
        {
            this.armor = armor;
            Name = AbilityName.Armor;
            Description = $"+{armor} защита";
            Type = AbilityType.Passive;
            Target = Target.None;
        }

        public override void Get(Player player)
        {
            player.Armor += armor;
        }
    }
}
