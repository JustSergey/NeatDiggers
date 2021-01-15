using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class FirstAidKitItem : Item
    {
        public FirstAidKitItem()
        {
            Name = ItemName.FirstAidKit;
            Title = "Аптечка";
            Description = "Восстанавливает 2 хп";
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.Health += 2;
            if (gameAction.CurrentPlayer.Health > gameAction.CurrentPlayer.Character.MaxHealth)
                gameAction.CurrentPlayer.Health = gameAction.CurrentPlayer.Character.MaxHealth;
            return true;
        }
    }
}
