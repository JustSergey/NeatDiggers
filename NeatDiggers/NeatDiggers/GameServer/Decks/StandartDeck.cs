using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Items;

namespace NeatDiggers.GameServer.Decks
{
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
