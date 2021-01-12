namespace NeatDiggers.GameServer.Items
{
    public class CatchUpEvent : Item
    {
        public CatchUpEvent()
        {
            Name = ItemName.CatchUp;
            Title = "Догонялки";
            Description = "Телепортирует персонажа к цели";
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);
            if (targetPlayer != null)
            {
                Vector targetPosition = targetPlayer.Position;
                if (targetPosition.IsInMap(room.GetGameMap()))
                    gameAction.CurrentPlayer.Position = targetPosition;
                return true;
            }
            return false;
        }
    }
}