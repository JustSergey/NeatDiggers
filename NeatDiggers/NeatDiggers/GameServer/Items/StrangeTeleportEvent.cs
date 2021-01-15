using System;


namespace NeatDiggers.GameServer.Items
{
    public class StrangeTeleportEvent : Item
    {
        public StrangeTeleportEvent()
        {
            Name = ItemName.StrangeTeleport;
            Title = "Странное Заклинание";
            Description = "Телепортирует всех персонажей в начало";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }
        
        public override bool Use(Room room, GameAction gameAction)
        {
            room.Players.ForEach(p => p.Position = p.SpawnPoint);
            return true;
        }
    }
}