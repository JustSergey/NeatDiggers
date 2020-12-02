using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public enum ItemType
    {
        Event,
        Artifact,
        Weapon,
        Armor
    }

    public enum WeaponHanded
    {
        None,
        One,
        Two
    }

    public enum WeaponType
    {
        None,
        Melee,
        Ranged
    }

    public class Item
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public WeaponHanded WeaponHanded { get; set; }
        public WeaponType WeaponType { get; set; }

        public virtual void Use(Room room, Player targetPlayer, Vector targetPosition) { }
    }

    public class EmptyItem : Item { }

    public class RainItem : Item
    {
        public RainItem()
        {
            Title = "Дождь";
            Description = "-2 скорость у всех игроков на 2 круга";
            Type = ItemType.Event;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, Player targetPlayer, Vector targetPosition)
        {
            room.Players.ForEach(p => p.Speed -= 2);
            Action<Room> cancelingAction = (Room room) => room.Players.ForEach(p => p.Speed += 2);
            room.AddCancelingAction(room.PlayerTurn, room.Round + 2, cancelingAction);
        }
    }
}
