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
        AutomaticRifle,
        FirstAidKit,
        Crossbow,
        Bandage,
        BigFirstAidKit,
        ArmorPlate,
        Grenade,
        HeavySword,
        Vest,
        DoubleTurnItem,
        OneDamageItem,
        TwoDamageItem,
        PowerShieldItem,
        ArmorBuffItem,
        Katana,
        Claws,
        HealFiveItem,
        InvulnerabilituItem,
        DoubleDamageItem,
        Hook,
        Jacket,
        Laser,
        Bow,
        Sword,
        SledgeHammer,
        Knife,
        ExplosiveSpellItem,
        SwitchPositionsItem,
        HandGun,
        Scope,
        DoubleScope,
        MachineGun,
        Boots,
        HealThreeItem,
        SniperRifle,
        Axe,
        SpikeArmor,
        SyringeGun,
        Shield,
        length
    }

    public enum ItemType
    {
        //Event,
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
        public WeaponHanded WeaponHanded { get; set; }
        public WeaponType WeaponType { get; set; }
        public int WeaponDamage { get; set; }
        public int WeaponConsumption { get; set; }
        public int WeaponDistance { get; set; }
        public  int ArmorStrength { get; set; }
        public int ArmorDurability { get; set; }

        public virtual void Get(Room room, GameAction gameAction) { }
        public virtual bool Use(Room room, GameAction gameAction) { return false; }
        public virtual void Drop(Room room, GameAction gameAction) { }

        public static Item CreateItem(ItemName name) =>
            name switch
            {
                ItemName.Empty => new EmptyItem(),
                ItemName.Rain => new RainItem(),
                ItemName.AutomaticRifle => new AutomaticRifleWeapon(),
                ItemName.StrangeTeleport => new StrangeTeleportEvent(),
                ItemName.CatchUp => new CatchUpEvent(),
                ItemName.SoulsExchange => new SoulsExchangeEvent(),
                ItemName.SuperJump => new SuperJumpEvent(),
                ItemName.ItemSteal => new ItemStealEvent(),
                _ => null
            };
    }

    public class EmptyItem : Item
    {
        public EmptyItem()
        {
            Name = ItemName.Empty;
            Type = ItemType.Active;
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
