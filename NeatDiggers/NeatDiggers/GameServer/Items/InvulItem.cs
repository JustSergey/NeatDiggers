using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class InvulItem : Item
    {
        public InvulItem()
        {
            Name = ItemName.Invul;
            Title = "Неуязвимость";
            Description = "Неуязвимость на 1 круг за 2 сброса";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            if (gameAction.CurrentPlayer.Inventory.Drop >= 2)
            {
                gameAction.CurrentPlayer.Inventory.Drop -= 2;
                gameAction.CurrentPlayer.Armor += 10000;
                string id = gameAction.CurrentPlayer.Id;
                Action<Room> cancelingAction = (Room room) => room.GetPlayer(id).Armor -= 10000;
                room.AddCancelingAction(room.PlayerTurn, room.Round + 1, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
