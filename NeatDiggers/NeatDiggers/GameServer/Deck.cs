using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer.Decks
{
    public enum DeckType
    {
        Standart,
        Custom
    }

    public class Deck
    {
        private List<(ItemName, int)> items;

        public List<Item> Shuffle()
        {
            Random random = new Random();
            List<Item> result = new List<Item>();
            foreach (var item in items)
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

        public static Deck Parse(string stringDeck)
        {
            string[] list = stringDeck.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (list.Length < 2)
                return null;
            List<string> itemNames = Enum.GetNames(typeof(ItemName)).ToList();
            List<(ItemName, int)> resultItems = new List<(ItemName, int)>();
            for (int i = 0; i < list.Length; i++)
            {
                string[] item = list[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (item.Length != 2)
                    return null;
                int index = itemNames.IndexOf(item[0]);
                if (index < 0)
                    return null;
                if (!int.TryParse(item[1], out int count))
                    return null;
                resultItems.Add(((ItemName)index, count));
            }
            return new Deck { items = resultItems };
        }
    }
}
