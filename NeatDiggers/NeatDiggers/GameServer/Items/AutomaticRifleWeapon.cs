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
            WeaponConsumption = 1;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            //TODO
        }
    }
}