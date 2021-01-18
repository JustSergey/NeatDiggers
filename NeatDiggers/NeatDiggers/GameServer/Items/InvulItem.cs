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
            Player player = gameAction.CurrentPlayer;
            if (player.Inventory.Drop >= 2)
            {
                player.Inventory.Drop -= 2;
                player.Armor += 10000;
                player.Effects.Add("Неуязвимость");
                Action<Room> cancelingAction = (Room room) =>
                {
                    Player p = room.GetPlayer(player.Id);
                    if (p != null)
                    {
                        p.Armor -= 10000;
                        p.Effects.Remove("Неуязвимость");
                    }
                };
                room.AddCancelingAction(room.Round + 1, cancelingAction);
                return true;
            }
            return false;
        }
    }
}
