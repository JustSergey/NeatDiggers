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
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            if (gameAction.CurrentPlayer.Inventory.Drop >= consumption)
            {
                gameAction.CurrentPlayer.Inventory.Drop -= consumption;
                gameAction.CurrentPlayer.Armor += 10000;
                string id = gameAction.CurrentPlayer.Id;
                Action<Room> cancelingAction = (Room room) => room.GetPlayer(id).Armor -= 10000;
                room.AddCancelingAction(room.PlayerTurn, room.Round + time, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
