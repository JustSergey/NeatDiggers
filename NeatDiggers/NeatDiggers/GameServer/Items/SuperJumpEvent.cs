namespace NeatDiggers.GameServer.Items
{
    public class SuperJumpEvent : Item
    {
        public SuperJumpEvent()
        {
            Name = ItemName.SuperJump;
            Title = "Скачок";
            Description = "Телепортирует персонажа на любую клетку (дальность = скорость + 4)";
            Type = ItemType.Active;
            Target = Target.Position;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            Vector playerPosition = gameAction.CurrentPlayer.Position;
            if (playerPosition.CheckAvailability(gameAction.TargetPosition, gameAction.CurrentPlayer.Speed + 4))
                gameAction.CurrentPlayer.Position = gameAction.TargetPosition;
            return true;
        }
    }
}