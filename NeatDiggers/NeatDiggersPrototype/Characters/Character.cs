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
    }

    abstract class Character
    {
        delegate Character CreateCharacter();
        static Dictionary<CharacterName, CreateCharacter> characters = new Dictionary<CharacterName, CreateCharacter>
        {
            { CharacterName.Empty, () => new EmptyCharacter() },
            { CharacterName.Pandora, () => new PandoraCharacter() }
        };

        CharacterName name;
        int health;

        public Character(CharacterName name, int health)
        {
            this.name = name;
            this.health = health;
        }

        public static Character NewCharacter(CharacterName name)
        {
            if (characters.TryGetValue(name, out CreateCharacter creator))
                return creator.Invoke();
            return null;
        }

        public CharacterInfo GetInfo()
        {
            return new CharacterInfo
            {
                Name = name,
                Health = health
            };
        }
    }
}
