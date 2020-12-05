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
        }

        public override void Use(Room room, GameAction gameAction)
        {
            room.GetPlayer(gameAction.CurrentPlayer.Id).Speed += speed;
        }
    }
}
