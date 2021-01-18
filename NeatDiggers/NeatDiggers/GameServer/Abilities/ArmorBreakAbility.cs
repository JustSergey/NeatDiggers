using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class ArmorBreakAbility : Ability
    {
        int armor;
        int time;
        int consumption;

        public ArmorBreakAbility(int armor, int time, int consumption)
        {
            this.armor = armor;
            this.time = time;
            this.consumption = consumption;
            Name = AbilityName.ArmorBreak;
            Description = $"Ломает {armor} брони цели на {time} круг(а) за {consumption} сброса";
            Type = AbilityType.Active;
            Target = Target.Player;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            if (gameAction.CurrentPlayer.Inventory.Drop >= consumption)
            {
                gameAction.CurrentPlayer.Inventory.Drop -= consumption;
                Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);
                targetPlayer.Armor -= armor;
                targetPlayer.Effects.Add(new Effect(p => p.Armor += armor)
                {
                    Title = $"Сломана броня (-{armor})",
                    Duration = time
                });
                return true;
            }
            return false;
        }
    }
}
