using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer
{
    public class Inventory
    {
        public const int MaxItems = 6;
        public Item LeftWeapon { get; set; }
        public Item RightWeapon { get; set; }
        public Item Armor { get; set; }
        public List<Item> Items { get; set; }
        public int Drop { get; set; }

        public Inventory()
        {
            LeftWeapon = new EmptyItem();
            RightWeapon = new EmptyItem();
            Armor = new EmptyItem();
            Items = new List<Item>();
            Drop = 0;
        }

        public void Clear(Player player)
        {
            LeftWeapon = new EmptyItem();
            RightWeapon = new EmptyItem();
            Armor = new EmptyItem();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == ItemType.Passive)
                    Items[i].Drop(player);
            }
            Items = new List<Item>();
            Drop = 0;
        }

        public bool DropItem(Item item, Player player)
        {
            if (item.Name == ItemName.Empty)
                return false;
            int value = 0;
            Item invItem = Items.Find(i => i.Name == item.Name);
            if (invItem != null)
            {
                if (invItem.Type == ItemType.Passive)
                    invItem.Drop(player);
                value = (int)invItem.Rarity;
                Items.Remove(invItem);
            }
            else if (LeftWeapon.Name == item.Name)
            {
                value = (int)LeftWeapon.Rarity;
                LeftWeapon = new EmptyItem();
            }
            else if (RightWeapon.Name == item.Name)
            {
                value = (int)RightWeapon.Rarity;
                RightWeapon = new EmptyItem();
            }
            else if (Armor.Name == item.Name)
            {
                value = (int)Armor.Rarity;
                Armor = new EmptyItem();
            }
            else
                return false;
            Drop += value;
            return true;
        }
    }
}
