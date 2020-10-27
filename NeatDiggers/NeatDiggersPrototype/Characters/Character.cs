using System;
using System.Collections.Generic;
using System.Text;

namespace NeatDiggersPrototype
{
    enum CharacterName
    {
        Empty,
        Pandora
    }

    class CharacterInfo
    {
        public CharacterName Name;
        public int Health;
        public int Level;
    }

    abstract class Character
    {
        CharacterName name;
        int health;
        int level;

        public Character(CharacterName name, int health)
        {
            this.name = name;
            this.health = health;
            level = 0;
        }

        public static Character CreateCharacter(CharacterName name) =>
            name switch
            {
                CharacterName.Empty => new EmptyCharacter(),
                CharacterName.Pandora => new PandoraCharacter(),
                _ => null
            };

        public CharacterInfo GetInfo() =>
            new CharacterInfo
            {
                Name = name,
                Health = health,
                Level = level
            };
    }
}
