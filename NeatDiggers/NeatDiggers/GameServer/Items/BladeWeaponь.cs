namespace NeatDiggers.GameServer.Items
{
    public class BladeWeapon : Item
    {
        public BladeWeapon()
        {
            Name = ItemName.Blade;
            Title = "Клинок (одноручный)";
            Description = "2 урона";
            Type = ItemType.Weapon;
            WeaponHanded = WeaponHanded.One;
            WeaponType = WeaponType.Melee;
            WeaponDamage = 2;
            WeaponDistance = 0;
            WeaponConsumption = 0;
        }
    }
}