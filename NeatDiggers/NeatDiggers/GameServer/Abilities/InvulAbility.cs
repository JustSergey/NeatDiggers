using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class InvulAbility : Ability
    {
        int time;
        int consumption;

        public InvulAbility(int time, int consumption)
        {
            this.time = time;
            this.consumption = consumption;
            Name = AbilityName.Invul;
            Description = $"Неуязвимость на {time} круг(а) за {consumption} сброса";
            Type = AbilityType.Active;
            Target = Target.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Player player = gameAction.CurrentPlayer;
            if (player.Inventory.Drop >= consumption)
            {
                player.Inventory.Drop -= consumption;
                player.Armor += 10000;
                player.Effects.Add(new Effect(p => p.Armor -= 10000)
                {
                    Title = "Неуязвимость",
                    Duration = time
                });
                return true;
            }
            return false;
        }
    }
}
