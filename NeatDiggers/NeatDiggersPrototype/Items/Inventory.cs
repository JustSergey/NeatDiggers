using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeatDiggersPrototype
{
    class InventoryInfo
    {
        public List<ItemInfo> ItemsInfo;
    }

    class Inventory
    {
        List<Item> items;

        public Inventory()
        {
            items = new List<Item>();
        }

        public InventoryInfo GetInfo()
        {
            return new InventoryInfo
            {
                ItemsInfo = items.Select(i => i.GetInfo()).ToList()
            };
        }
    }
}
