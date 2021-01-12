using System;
using System.Collections.Generic;

namespace NeatDiggers.GameServer.Items
{
    public class ItemStealEvent : Item
    {
        public ItemStealEvent()
        {
            Name = ItemName.ItemSteal;
            Title = "Мне можно";
            Description = "Позволяет забрать 1 случайную вещь у любого игрока";
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            List<Item> items = room.GetPlayer(gameAction.TargetPlayer.Id).Inventory.Items;
            int rand = new Random().Next(items.Count);
            Item item = items[rand];
            items.RemoveAt(rand);
            if (item.Type == ItemType.Passive)
                item.Get(room, gameAction);
            gameAction.CurrentPlayer.Inventory.Items.Add(item);
            return true;
        }
    }
}