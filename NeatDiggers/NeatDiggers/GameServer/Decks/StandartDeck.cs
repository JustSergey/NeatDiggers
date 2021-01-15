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
                (ItemName.StrangeTeleport, 1),
                (ItemName.CatchUp, 1),
                (ItemName.SoulsExchange, 1),
                (ItemName.SuperJump, 1),
                (ItemName.ItemSteal, 1),
                (ItemName.Tutorial, 1),
                (ItemName.AutomaticRifle, 1),
                (ItemName.SniperRifle, 1),
                (ItemName.Pistol, 2),
                (ItemName.Revolver, 1),
                (ItemName.Sword, 1),
                (ItemName.Katana, 1),
                (ItemName.Knife, 2),
                (ItemName.Blade, 1),
                (ItemName.Sharpen, 2),
                (ItemName.Scope, 2),
                (ItemName.Jacket, 4),
                (ItemName.Vest, 2),
                (ItemName.Armor, 1),
                (ItemName.Boots, 2),
                (ItemName.SuperBoots, 1),
                (ItemName.Bandage, 4),
                (ItemName.FirstAidKit, 2),
                (ItemName.BigFirstAidKit, 1),
                (ItemName.Invul, 1),
                (ItemName.DoubleDamage, 1)
            };
        }
    }
}
