using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenCombatEngineCore
{
    public abstract class Utils
    {
        public static List<Spell> GetAllUseableSpellsBySpellType(List<Spell> spells, SpellType spellType, double currentEnergy, double distance)
            => spells.Where(spell => spell.MainSpellType == spellType)
                     .Where(spell => spell.EnergyCost <= currentEnergy)
                     .Where(spell => !spell.IsOnCooldown)
                     .Where(spell => distance < spell.MaxRange)
                     .ToList();
    }
}
