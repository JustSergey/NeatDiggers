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
                string id = gameAction.TargetPlayerId;
                room.GetPlayer(id).Armor -= armor;
                room.GetPlayer(id).Effects.Add($"Сломана броня (-{armor})");
                Action<Room> cancelingAction = (Room room) =>
                {
                    Player p = room.GetPlayer(id);
                    if (p != null)
                    {
                        p.Armor += armor;
                        p.Effects.Remove($"Сломана броня (-{armor})");
                    }
                    room.GetPlayer(id).Armor += armor;
                };
                room.AddCancelingAction(room.Round + time, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
