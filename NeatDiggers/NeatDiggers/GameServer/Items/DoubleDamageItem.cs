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
            Description = "Увеличивает урон в 2 раза на 1 круг за 3 сброса";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Player player = gameAction.CurrentPlayer;
            if (player.Inventory.Drop >= 3)
            {
                player.Inventory.Drop -= 3;
                player.MultiplyDamage *= 2.0;
                player.Effects.Add("Урон х2");
                Action<Room> cancelingAction = (Room room) =>
                {
                    Player p = room.GetPlayer(player.Id);
                    if (p != null)
                    {
                        p.MultiplyDamage /= 2.0;
                        p.Effects.Remove("Урон х2");
                    }
                };
                room.AddCancelingAction(room.Round + 1, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
