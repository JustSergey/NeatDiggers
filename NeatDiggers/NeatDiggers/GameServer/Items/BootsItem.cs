using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class BootsItem : Item
    {
        public BootsItem()
        {
            Name = ItemName.Boots;
            Title = "Сапоги";
            Description = "+1 к скорости";
            Type = ItemType.Passive;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            Rarity = Rarity.Uncommon;
        }

        public override void Get(Player player)
        {
            player.Speed += 1;
        }

        public override void Drop(Player player)
        {
            player.Speed -= 1;
        }
    }
}
