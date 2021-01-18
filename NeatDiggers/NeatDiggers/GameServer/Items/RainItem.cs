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
                p.Effects.Add(new Effect(p => p.Speed += 2)
                {
                    Title = "Дождь (скорость -2)",
                    Duration = 2
                });
            });
            return true;
        }
    }
}
