using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Abilities;

namespace NeatDiggers.GameServer.Characters
{
    public class KirillCharacter : Character
    {
        public KirillCharacter()
        {
            Name = CharacterName.Kirill;
            Title = "Кирилл";
            MaxHealth = 6;
            WeaponType = WeaponType.Melee;
            Abilities = new List<Ability>
            {
                new HealAbility(1, 1),
                new ArmorBreakAbility(2, 2, 3),
                new SpeedAbility(2),
                new InvulAbility(2, 4)
            };
        }
    }
}
