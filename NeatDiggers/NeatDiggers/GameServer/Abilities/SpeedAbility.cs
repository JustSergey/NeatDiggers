using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class SpeedAbility : Ability
    {
        int speed;

        public SpeedAbility(int speed)
        {
            this.speed = speed;
            Name = AbilityName.Speed;
            Description = $"+{speed} скорость";
            Type = AbilityType.Passive;
            Target = Target.None;
        }

        public override void Get(Player player)
        {
            player.Speed += speed;
        }
    }
}
