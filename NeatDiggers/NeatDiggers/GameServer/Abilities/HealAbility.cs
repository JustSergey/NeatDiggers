using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class HealAbility : Ability
    {
        int heal;
        int consumption;

        public HealAbility(int heal, int consumption)
        {
            this.heal = heal;
            this.consumption = consumption;
            Name = AbilityName.Heal;
            Description = $"Лечит {heal} хп цели за {consumption} сброса";
            Type = AbilityType.Active;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);
            if (gameAction.CurrentPlayer.Inventory.Drop >= consumption)
            {
                gameAction.CurrentPlayer.Inventory.Drop -= consumption;
                if (targetPlayer.Health + heal <= targetPlayer.Character.MaxHealth)
                    targetPlayer.Health += heal;
                return true;
            }
            return false;
        }
    }
}
