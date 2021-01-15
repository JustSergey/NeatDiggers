namespace NeatDiggers.GameServer.Items
{
    public class KatanaWeapon : Item
    {
        public KatanaWeapon()
        {
            Name = ItemName.Katana;
            Title = "Катана (двуручная)";
            Description = "3 урона. Дальность +1";
            Type = ItemType.Weapon;
            WeaponHanded = WeaponHanded.Two;
            WeaponType = WeaponType.Melee;
            WeaponDamage = 3;
            WeaponDistance = 1;
            WeaponConsumption = 0;
        }
    }
}