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
            Target = Target.Player;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            List<Item> items = room.GetPlayer(gameAction.TargetPlayerId).Inventory.Items;
            if (items.Count > 0)
            {
                int rand = new Random().Next(items.Count);
                Item item = items[rand];
                items.RemoveAt(rand);
                if (item.Type == ItemType.Passive)
                    item.Get(gameAction.CurrentPlayer);
                gameAction.CurrentPlayer.Inventory.Items.Add(item);
                return true;
            }
            return false;
        }
    }
}