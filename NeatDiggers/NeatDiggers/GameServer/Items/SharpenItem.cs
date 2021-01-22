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
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Get(Player player)
        {
            player.MeleeDamage += 1;
        }

        public override void Drop(Player player)
        {
            player.MeleeDamage -= 1;
        }
    }
}
