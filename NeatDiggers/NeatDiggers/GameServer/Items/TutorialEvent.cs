namespace NeatDiggers.GameServer.Items
{
    public class TutorialEvent : Item
    {
        public TutorialEvent()
        {
            Name = ItemName.Tutorial;
            Title = "Обучение";
            Description = "+1 уровень";
            Type = ItemType.Event;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override void Use(Room room, GameAction gameAction)
        {
            room.GetPlayer(gameAction.CurrentPlayer.Id).Level += 1;
        }
    }
}