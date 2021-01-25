using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class DoubleDamageItem : Item
    {
        public DoubleDamageItem()
        {
            Name = ItemName.DoubleDamage;
            Title = "Двойной урон";
            Description = "Увеличивает урон в 2 раза на 1 круг за 3 сброса";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
            Rarity = Rarity.Rare;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Player player = gameAction.CurrentPlayer;
            if (player.Inventory.Drop >= 3)
            {
                player.Inventory.Drop -= 3;
                player.MultiplyDamage *= 2.0;
                player.Effects.Add(new Effect(p => p.MultiplyDamage /= 2.0)
                {
                    Title = "Урон x2",
                    Duration = 1
                });
                return true;
            }
            return false;
        }
    }
}
