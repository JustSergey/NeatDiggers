namespace NeatDiggers.GameServer.Items
{
    public class CatchUpEvent : Item
    {
        public CatchUpEvent()
        {
            Name = ItemName.CatchUp;
            Title = "Догонялки";
            Description = "Телепортирует персонажа к цели";
            Type = ItemType.Event;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            Vector targetPosition = gameAction.TargetPosition;
            bool playerIsNear = false;
            foreach (var player in room.Players)
            {
                if (player.Position.CheckAvailability(targetPosition, 1))
                    playerIsNear = true;
            }
            if (targetPosition.IsInMap(room.GameMap) && playerIsNear)
                room.GetPlayer(gameAction.CurrentPlayer.Id).Position = targetPosition;

        }
    }
}