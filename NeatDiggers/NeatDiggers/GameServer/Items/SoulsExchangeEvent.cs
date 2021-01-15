namespace NeatDiggers.GameServer.Items
{
    public class SoulsExchangeEvent : Item
    {
        public SoulsExchangeEvent()
        {
            Name = ItemName.SoulsExchange;
            Title = "Обмен душ";
            Description = "Позволяет обменяться здоровьем с целью";
            Type = ItemType.Active;
            Target = Target.Player;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }
        
        public override bool Use(Room room, GameAction gameAction)
        {
            Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);
            int targetHealth = targetPlayer.Health;
            targetPlayer.Health = gameAction.CurrentPlayer.Health;
            if (targetPlayer.Health > targetPlayer.Character.MaxHealth)
                targetPlayer.Health = targetPlayer.Character.MaxHealth;
            gameAction.CurrentPlayer.Health = targetHealth;
            if (gameAction.CurrentPlayer.Health > gameAction.CurrentPlayer.Character.MaxHealth)
                gameAction.CurrentPlayer.Health = gameAction.CurrentPlayer.Character.MaxHealth;
            return true;
        }
    }
}