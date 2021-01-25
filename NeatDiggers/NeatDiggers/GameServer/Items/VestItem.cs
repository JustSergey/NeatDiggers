using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class VestItem : Item
    {
        public VestItem()
        {
            Name = ItemName.Vest;
            Title = "Жилет";
            Description = "Защита 2 на 2 раза";
            Type = ItemType.Armor;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            ArmorStrength = 2;
            ArmorDurability = 2;
            Rarity = Rarity.Uncommon;
        }
    }
}
