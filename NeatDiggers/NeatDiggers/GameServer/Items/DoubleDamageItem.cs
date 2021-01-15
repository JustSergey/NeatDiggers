using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class DoubleDamageItem : Item
    {
        public DoubleDamageItem()
        {
            Name = ItemName.DoubleDamage;
            Title = "Двойной урон";
            Description = "Увеличивает весь дальний урон в 2 раза на 1 круг за 3 сброса";
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            if (gameAction.CurrentPlayer.Inventory.Drop >= 3)
            {
                gameAction.CurrentPlayer.Inventory.Drop -= 3;
                gameAction.CurrentPlayer.MultiplyDamage *= 2.0;
                string id = gameAction.CurrentPlayer.Id;
                Action<Room> cancelingAction = (Room room) => room.GetPlayer(id).MultiplyDamage /= 2.0;
                room.AddCancelingAction(room.PlayerTurn, room.Round + 1, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
