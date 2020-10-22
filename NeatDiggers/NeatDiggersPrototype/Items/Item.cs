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
        delegate Item CreateItem();
        static Dictionary<ItemName, CreateItem> characters = new Dictionary<ItemName, CreateItem>
        {
            { ItemName.Empty, () => new EmptyItem() },
            { ItemName.Vest, () => new VestItem() }
        };

        bool isOpen;
        ItemName name;

        public Item(ItemName name)
        {
            isOpen = false;
            this.name = name;
        }

        public static Item NewItem(ItemName name)
        {
            if (characters.TryGetValue(name, out CreateItem creator))
                return creator.Invoke();
            return null;
        }

        public ItemInfo GetInfo()
        {
            return new ItemInfo
            {
                IsOpen = isOpen,
                Name = name
            };
        }
    }
}
