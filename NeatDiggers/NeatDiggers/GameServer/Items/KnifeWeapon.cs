namespace NeatDiggers.GameServer.Items
{
    public class KnifeWeapon : Item
    {
        public KnifeWeapon()
        {
            Name = ItemName.Knife;
            Title = "Нож (одноручный)";
            Description = "1 урон";
            Type = ItemType.Weapon;
            Target = Target.None;
            WeaponHanded = WeaponHanded.One;
            WeaponType = WeaponType.Melee;
            WeaponDamage = 1;
            WeaponDistance = 0;
            WeaponConsumption = 0;
        }
    }
}