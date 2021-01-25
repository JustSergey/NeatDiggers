using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class SuperBootsItem : Item
    {
        public SuperBootsItem()
        {
            Name = ItemName.SuperBoots;
            Title = "Сапоги скороходы";
            Description = "+2 к скорости";
            Type = ItemType.Passive;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            Rarity = Rarity.Rare;
        }

        public override void Get(Player player)
        {
            player.Speed += 2;
        }

        public override void Drop(Player player)
        {
            player.Speed -= 2;
        }
    }
}
