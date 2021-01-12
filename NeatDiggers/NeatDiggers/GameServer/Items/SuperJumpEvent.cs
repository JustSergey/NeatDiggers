namespace NeatDiggers.GameServer.Items
{
    public class SuperJumpEvent : Item
    {
        public SuperJumpEvent()
        {
            Name = ItemName.SuperJump;
            Title = "Скачок";
            Description = "Телепортирует персонажа на любую клетку (дальность +6)";
            Type = ItemType.Active;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            Vector playerPosition = gameAction.CurrentPlayer.Position;
            if (playerPosition.CheckAvailability(gameAction.TargetPosition, gameAction.CurrentPlayer.Speed + 6))
                room.GetPlayer(gameAction.CurrentPlayer.Id).Position = gameAction.TargetPosition;
        }
    }
}