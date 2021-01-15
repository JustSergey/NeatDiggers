using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Abilities;

namespace NeatDiggers.GameServer.Characters
{
    public class JupiterCharacter : Character
    {
        public JupiterCharacter()
        {
            Name = CharacterName.Jupiter;
            Title = "Юпитер";
            MaxHealth = 10;
            WeaponType = WeaponType.Melee;
            Abilities = new List<Ability>
            {
                new DamageAbility(1, WeaponType.Melee),
                new HealAbility(2, 2),
                new ArmorAbility(2),
                new InvulAbility(2, 4)
            };
        }
    }
}
