using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Abilities;

namespace NeatDiggers.GameServer.Characters
{
    public class PandoraCharacter : Character
    {
        public PandoraCharacter()
        {
            Name = CharacterName.Pandora;
            Title = "Пандора";
            MaxHealth = 7;
            WeaponType = WeaponType.Ranged;
            Abilities = new List<Ability>
            {
                new ShurikenAbility(2, 1, 1),
                new ArmorAbility(1),
                new DistanceAbility(2, WeaponType.Ranged),
                new DamageAbility(3, WeaponType.Ranged)
            };
        }
    }
}
