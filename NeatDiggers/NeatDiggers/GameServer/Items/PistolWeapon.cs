namespace NeatDiggers.GameServer.Items
{
    public class PistolWeapon : Item
    {
        public PistolWeapon()
        {
            Name = ItemName.Pistol;
            Title = "Пистолет (одноручный)";
            Description = "1 уронa. Дальность +1. Выстрел за 1 сброс";
            Type = ItemType.Weapon;
            Target = Target.None;
            WeaponHanded = WeaponHanded.One;
            WeaponType = WeaponType.Ranged;
            WeaponDamage = 1;
            WeaponDistance = 1;
            WeaponConsumption = 1;
        }
    }
}