using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer
{
    public class Inventory
    {
        public Item LeftWeapon { get; set; }
        public Item RightWeapon { get; set; }
        public List<Item> Items { get; set; }
        public int Drop { get; set; }

        public Inventory()
        {
            LeftWeapon = new EmptyItem();
            RightWeapon = new EmptyItem();
            Items = new List<Item>();
            Drop = 0;
        }
    }
}
