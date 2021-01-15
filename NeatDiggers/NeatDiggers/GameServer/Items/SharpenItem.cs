using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class SharpenItem : Item
    {
        public SharpenItem()
        {
            Name = ItemName.Sharpen;
            Title = "Заточить лезвие";
            Description = "Увеличивает урон ближнего оружия на 1";
            Type = ItemType.Passive;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Get(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.MeleeDamage += 1;
        }

        public override void Drop(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.MeleeDamage -= 1;
        }
    }
}
