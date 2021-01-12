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
            WeaponType = WeaponType.None;
            Abilities = new List<Ability>
            {
                new Shuriken(2, 1, 1),
                new SpeedAbility(2)
            };
        }
    }
}
