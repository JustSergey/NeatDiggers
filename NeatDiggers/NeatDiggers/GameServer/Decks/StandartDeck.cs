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
                (ItemName.Rain, 1),
                (ItemName.AutomaticRifle, 1),
                (ItemName.StrangeTeleport, 1),
                (ItemName.CatchUp, 1),
                (ItemName.SoulsExchange, 1),
                (ItemName.SuperJump, 1),
                (ItemName.ItemSteal, 1)
            };
        }
    }
}
