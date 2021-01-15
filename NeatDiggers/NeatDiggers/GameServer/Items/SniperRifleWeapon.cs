namespace NeatDiggers.GameServer.Items
{
    public class SniperRifleWeapon : Item
    {
        public SniperRifleWeapon()
        {
            Name = ItemName.SniperRifle;
            Title = "Снайперка (двуручная)";
            Description = "3 урона. Дальность +4. Выстрел за 2 сброса";
            Type = ItemType.Weapon;
            Target = Target.None;
            WeaponHanded = WeaponHanded.Two;
            WeaponType = WeaponType.Ranged;
            WeaponDamage = 3;
            WeaponDistance = 4;
            WeaponConsumption = 2;
        }
    }
}