using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class ArmorItem : Item
    {
        public ArmorItem()
        {
            Name = ItemName.Armor;
            Title = "Броня";
            Description = "Защита 3 на 2 раза";
            Type = ItemType.Armor;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            ArmorStrength = 3;
            ArmorDurability = 2;
            Rarity = Rarity.Rare;
        }
    }
}
