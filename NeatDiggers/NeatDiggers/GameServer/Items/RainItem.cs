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
            Type = ItemType.Event;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            room.Players.ForEach(p => p.Speed -= 2);
            Action<Room> cancelingAction = (Room room) => room.Players.ForEach(p => p.Speed += 2);
            room.AddCancelingAction(room.PlayerTurn, room.Round + 2, cancelingAction);
        }
    }
}
