namespace NeatDiggers.GameServer.Items
{
    public class RevolverWeapon : Item
    {
        public RevolverWeapon()
        {
            Name = ItemName.Revolver;
            Title = "Револьвер (одноручный)";
            Description = "2 уронa. Дальность +1. Выстрел за 1 сброс";
            Type = ItemType.Weapon;
            Target = Target.None;
            WeaponHanded = WeaponHanded.One;
            WeaponType = WeaponType.Ranged;
            WeaponDamage = 2;
            WeaponDistance = 1;
            WeaponConsumption = 1;
            Rarity = Rarity.Rare;
        }
    }
}