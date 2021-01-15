using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.GameServer.Abilities
{
    public class DamageAbility : Ability
    {
        int damage;
        WeaponType type;

        public DamageAbility(int damage, WeaponType type)
        {
            this.damage = damage;
            this.type = type;
            Name = AbilityName.Damage;
            Description = $"+{damage} {(type == WeaponType.Melee ? "ближнего" : "дальнего")} урона";
            Type = AbilityType.Passive;
            Target = Target.None;
        }

        public override void Get(Player player)
        {
            if (type == WeaponType.Melee)
                player.MeleeDamage += damage;
            else
                player.RangedDamage += damage;
        }
    }
}
