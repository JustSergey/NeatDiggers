using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Abilities;

namespace NeatDiggers.GameServer.Characters
{
    public class SiriusCharacter : Character
    {
        public SiriusCharacter()
        {
            Name = CharacterName.Sirius;
            Title = "Сириус";
            MaxHealth = 5;
            WeaponType = WeaponType.Ranged;
            Abilities = new List<Ability>
            {
                new DistanceAbility(1, WeaponType.Ranged),
                new SpeedAbility(2),
                new DamageAbility(2, WeaponType.Ranged),
                new ArmorAbility(3)
            };
        }
    }
}
