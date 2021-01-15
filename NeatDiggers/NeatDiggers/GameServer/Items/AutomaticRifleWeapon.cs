namespace NeatDiggers.GameServer.Items
{
    public class AutomaticRifleWeapon : Item
    {
        public AutomaticRifleWeapon()
        {
            Name = ItemName.AutomaticRifle;
            Title = "Автомат (двуручный)";
            Description = "3 урона. Дальность +2. Выстрел за 1 сброс";
            Type = ItemType.Weapon;
            WeaponHanded = WeaponHanded.Two;
            WeaponType = WeaponType.Ranged;
            WeaponDamage = 3;
            WeaponDistance = 2;
            WeaponConsumption = 1;
        }
    }
}