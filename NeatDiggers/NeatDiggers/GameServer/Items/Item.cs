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
        Event,
        Passive,
        Active,
        Weapon,
        Armor
    }

    public class Item
    {
        public ItemName Name { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public ItemType Type { get; protected set; }
        public WeaponHanded WeaponHanded { get; protected set; }
        public WeaponType WeaponType { get; protected set; }

        public virtual void Get(Room room, GameAction gameAction) { }
        public virtual void Use(Room room, GameAction gameAction) { }
        public virtual void Drop(Room room, GameAction gameAction) { }

        public static Item CreateItem(ItemName name) =>
            name switch
            {
                ItemName.Empty => new EmptyItem(),
                ItemName.Rain => new RainItem(),
                _ => null
            };
    }

    public class EmptyItem : Item
    {
        public EmptyItem()
        {
            Name = ItemName.Empty;
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }
    }
}
