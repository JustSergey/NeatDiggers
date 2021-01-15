using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class JacketItem : Item
    {
        public JacketItem()
        {
            Name = ItemName.Jacket;
            Title = "Куртка";
            Description = "Защита 1 на 3 раза";
            Type = ItemType.Armor;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            ArmorStrength = 1;
            ArmorDurability = 3;
        }
    }
}
