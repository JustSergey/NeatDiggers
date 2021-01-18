using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class RainItem : Item
    {
        public RainItem()
        {
            Name = ItemName.Rain;
            Title = "Дождь";
            Description = "-2 скорость у всех игроков на 2 круга";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            room.Players.ForEach(p => 
            {
                p.Speed -= 2;
                p.Effects.Add("Дождь (-2 к скорости)");
            });
            Action<Room> cancelingAction = (Room room) =>
            {
                room.Players.ForEach(p =>
                {
                    p.Speed += 2;
                    p.Effects.Remove("Дождь (-2 к скорости)");
                });
            };
            room.AddCancelingAction(room.Round + 2, cancelingAction);
            return true;
        }
    }
}
