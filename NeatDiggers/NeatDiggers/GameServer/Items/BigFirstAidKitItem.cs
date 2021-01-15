using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class BigFirstAidKitItem : Item
    {
        public BigFirstAidKitItem()
        {
            Name = ItemName.BigFirstAidKit;
            Title = "Большая аптечка";
            Description = "Восстанавливает 3 хп";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.Health += 3;
            if (gameAction.CurrentPlayer.Health > gameAction.CurrentPlayer.Character.MaxHealth)
                gameAction.CurrentPlayer.Health = gameAction.CurrentPlayer.Character.MaxHealth;
            return true;
        }
    }
}
