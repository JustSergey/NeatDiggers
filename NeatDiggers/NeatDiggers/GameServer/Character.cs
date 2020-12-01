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
        public CharacterName Name { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }

        public static Character CreateCharacter(CharacterName name) =>
            name switch
            {
                CharacterName.Empty => new EmptyCharacter(),
                _ => null
            };
    }

    public class EmptyCharacter : Character
    {
        public EmptyCharacter()
        {
            Name = CharacterName.Empty;
            Health = 0;
            Level = 0;
        }
    }
}
