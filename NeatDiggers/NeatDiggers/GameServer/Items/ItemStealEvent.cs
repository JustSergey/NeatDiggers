namespace NeatDiggers.GameServer.Items
{
    public class ItemStealEvent : Item
    {
        public ItemStealEvent()
        {
            Name = ItemName.ItemSteal;
            Title = "Мне можно";
            Description = "Позволяет забрать 1 любую вещь у любого игрока";
            Type = ItemType.Event;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            //TODO
        }
    }
}