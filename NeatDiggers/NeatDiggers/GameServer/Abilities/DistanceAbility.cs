using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class DistanceAbility : Ability
    {
        int distance;
        WeaponType type;

        public DistanceAbility(int distance, WeaponType type)
        {
            this.distance = distance;
            this.type = type;
            Name = AbilityName.Distance;
            Description = $"+{distance} дальности к {(type == WeaponType.Melee ? "ближней" : "дальней")} атаке";
            Type = AbilityType.Passive;
            Target = Target.None;
        }

        public override void Get(Player player)
        {
            if (type == WeaponType.Melee)
                player.MeleeDistance += distance;
            else
                player.RangedDistance += distance;
        }
    }
}
