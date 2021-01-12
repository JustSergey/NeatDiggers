namespace NeatDiggers.GameServer.Items
{
    public class AutomaticRifleWeapon : Item
    {
        public AutomaticRifleWeapon()
        {
            Name = ItemName.AutomaticRifle;
            Title = "Автомат";
            Description = "3 урона. Дальность +1";
            Type = ItemType.Weapon;
            WeaponHanded = WeaponHanded.Two;
            WeaponType = WeaponType.Ranged;
            WeaponDamage = 3;
            WeaponDistance = 1;
            WeaponConsumption = 1;
        }
    }
}