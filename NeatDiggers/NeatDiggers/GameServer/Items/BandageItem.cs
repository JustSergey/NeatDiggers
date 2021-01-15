using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Items
{
    public class BandageItem : Item
    {
        public BandageItem()
        {
            Name = ItemName.Bandage;
            Title = "Бинт";
            Description = "Восстанавливает 1 хп";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.Health += 1;
            if (gameAction.CurrentPlayer.Health > gameAction.CurrentPlayer.Character.MaxHealth)
                gameAction.CurrentPlayer.Health = gameAction.CurrentPlayer.Character.MaxHealth;
            return true;
        }
    }
}
