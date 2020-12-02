using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
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

    public class StandartDeck : Deck
    {
        public StandartDeck()
        {
            deck = new List<(ItemName, int)>
            {
                (ItemName.Rain, 2),
                (ItemName.Vest, 1)
            };
        }
    }
}
