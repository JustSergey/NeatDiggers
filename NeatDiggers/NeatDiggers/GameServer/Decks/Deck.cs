using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer.Decks
{
    public abstract class Deck
    {
        protected List<(ItemName, int)> deck;

        public List<Item> Shuffle()
        {
            Random random = new Random();
            List<Item> result = new List<Item>();
            foreach (var item in deck)
            {
                for (int i = 0; i < item.Item2; i++)
                    result.Add(Item.CreateItem(item.Item1));
            }
            for (int i = 0; i < result.Count; i++)
            {
                int index = random.Next(result.Count);
                Item temp = result[i];
                result[i] = result[index];
                result[index] = temp;
            }
            return result;
        }
    }
}
