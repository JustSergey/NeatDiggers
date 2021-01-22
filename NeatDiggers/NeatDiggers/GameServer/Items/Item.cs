using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public enum ItemName
    {
        Empty,
        Rain,
        StrangeTeleport,
        SoulsExchange,
        SuperJump,
        CatchUp,
        ItemSteal,
        Tutorial,
        FirstAidKit,
        Katana,
        Invul,
        DoubleDamage,
        Bandage,
        BigFirstAidKit,
        Grenade,
        HeavySword,
        PowerShieldItem,
        ArmorBuffItem,
        Hook,
        Laser,
        Bow,
        MachineGun,
        Shield,
        Boots,
        AutomaticRifle,
        SniperRifle,
        Blade,
        Pistol,
        Revolver,
        Sharpen,
        Scope,
        DoubleScope,
        Armor,
        Vest,
        Jacket,
        Sword,
        Knife,
        SuperBoots,
        length
    }

    public enum ItemType
    {
        Passive,
        Active,
        Weapon,
        Armor
    }

    public class Item
    {
        public ItemName Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public Target Target { get; set; }
        public WeaponHanded WeaponHanded { get; set; }
        public WeaponType WeaponType { get; set; }
        public int WeaponDamage { get; set; }
        public int WeaponConsumption { get; set; }
        public int WeaponDistance { get; set; }
        public  int ArmorStrength { get; set; }
        public int ArmorDurability { get; set; }

        public virtual void Get(Player player) { }
        public virtual bool Use(Room room, GameAction gameAction) { return false; }
        public virtual void Drop(Player player) { }

        public static Item CreateItem(ItemName name) =>
            name switch
            {
                ItemName.Empty => new EmptyItem(),
                ItemName.Rain => new RainItem(),
                ItemName.StrangeTeleport => new StrangeTeleportEvent(),
                ItemName.CatchUp => new CatchUpEvent(),
                ItemName.SoulsExchange => new SoulsExchangeEvent(),
                ItemName.SuperJump => new SuperJumpEvent(),
                ItemName.ItemSteal => new ItemStealEvent(),
                ItemName.Tutorial => new TutorialEvent(),
                ItemName.Sword => new SwordWeapon(),
                ItemName.Katana => new KatanaWeapon(),
                ItemName.Knife => new KnifeWeapon(),
                ItemName.Blade => new BladeWeapon(),
                ItemName.SniperRifle => new SniperRifleWeapon(),
                ItemName.AutomaticRifle => new AutomaticRifleWeapon(),
                ItemName.Pistol => new PistolWeapon(),
                ItemName.Revolver => new RevolverWeapon(),
                ItemName.Sharpen => new SharpenItem(),
                ItemName.Scope => new ScopeItem(),
                ItemName.Jacket => new JacketItem(),
                ItemName.Vest => new VestItem(),
                ItemName.Armor => new ArmorItem(),
                ItemName.Boots => new BootsItem(),
                ItemName.SuperBoots => new SuperBootsItem(),
                ItemName.Bandage => new BandageItem(),
                ItemName.FirstAidKit => new FirstAidKitItem(),
                ItemName.BigFirstAidKit => new BigFirstAidKitItem(),
                ItemName.Invul => new InvulItem(),
                ItemName.DoubleDamage => new DoubleDamageItem(),
                _ => null
            };
    }

    public class EmptyItem : Item
    {
        public EmptyItem()
        {
            Name = ItemName.Empty;
            Type = ItemType.Active;
            Target = Target.None;
            WeaponDamage = 0;
            WeaponDistance = 0;
            WeaponConsumption = 0;
            ArmorStrength = 0;
            ArmorDurability = 0;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }
    }
}
