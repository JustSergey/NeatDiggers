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
            int targetHealth = room.GetPlayer(gameAction.TargetPlayer.Id).Health;
            room.GetPlayer(gameAction.TargetPlayer.Id).Health = room.GetPlayer(gameAction.CurrentPlayer.Id).Health;
            room.GetPlayer(gameAction.CurrentPlayer.Id).Health = targetHealth;
            return true;
        }
    }
}