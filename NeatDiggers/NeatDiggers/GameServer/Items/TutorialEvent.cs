namespace NeatDiggers.GameServer.Items
{
    public class TutorialEvent : Item
    {
        public TutorialEvent()
        {
            Name = ItemName.Tutorial;
            Title = "Обучение";
            Description = "+1 уровень";
            Type = ItemType.Active;
            Target = Target.None;
            WeaponHanded = WeaponHanded.None;
            WeaponType = WeaponType.None;
        }

        public override bool Use(Room room, GameAction gameAction)
        {
            gameAction.CurrentPlayer.LevelUp();
            return true;
        }
    }
}