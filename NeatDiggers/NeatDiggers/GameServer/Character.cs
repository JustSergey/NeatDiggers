using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer
{
    public enum CharacterName
    {
        Empty,
        Pandora
    }

    public class Character
    {
        public CharacterName Name { get; protected set; }
        public string Title { get; protected set; }
        public int MaxHealth { get; protected set; }
        public WeaponType WeaponType { get; set; }

        public static Character CreateCharacter(CharacterName name) =>
            name switch
            {
                CharacterName.Empty => new EmptyCharacter(),
                _ => null
            };
    }

    public class EmptyCharacter : Character
    {
        public EmptyCharacter() => Name = CharacterName.Empty;
    }
}
