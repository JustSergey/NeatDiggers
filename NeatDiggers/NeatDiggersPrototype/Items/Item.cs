using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    enum ItemName
    {
        Empty,
        Vest
    }

    class ItemInfo
    {
        public bool IsOpen;
        public ItemName Name;
    }

    abstract class Item
    {
        bool isOpen;
        ItemName name;

        public Item(ItemName name)
        {
            isOpen = false;
            this.name = name;
        }

        public static Item CreateItem(ItemName name) =>
            name switch
            {
                ItemName.Empty => new EmptyItem(),
                ItemName.Vest => new VestItem(),
                _ => null
            };

        public ItemInfo GetInfo() =>
            new ItemInfo
            {
                IsOpen = isOpen,
                Name = name
            };
    }
}
