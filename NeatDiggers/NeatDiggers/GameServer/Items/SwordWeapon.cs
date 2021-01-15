namespace NeatDiggers.GameServer.Items
{
    public class SwordWeapon : Item
    {
        public SwordWeapon()
        {
            Name = ItemName.Sword;
            Title = "Меч (двуручный)";
            Description = "3 урона";
            Type = ItemType.Weapon;
            WeaponHanded = WeaponHanded.Two;
            WeaponType = WeaponType.Melee;
            WeaponDamage = 3;
            WeaponDistance = 0;
            WeaponConsumption = 0;
        }
    }
}