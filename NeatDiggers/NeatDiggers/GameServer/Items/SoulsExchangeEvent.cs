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
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }
        
        public override bool Use(Room room, GameAction gameAction)
        {
            Player targetPlayer = room.GetPlayer(gameAction.TargetPlayerId);
            int targetHealth = targetPlayer.Health;
            targetPlayer.Health = room.GetPlayer(gameAction.CurrentPlayer.Id).Health;
            gameAction.CurrentPlayer.Health = targetHealth;
            return true;
        }
    }
}