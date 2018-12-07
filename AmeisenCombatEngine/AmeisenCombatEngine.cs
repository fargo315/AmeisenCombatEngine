using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using System.Collections.Generic;

namespace AmeisenCombatEngineCore
{
    public class CombatEngine
    {
        public List<Spell> Spells { get; set; }
        public Unit Me { get; set; }
        public Unit Target { get; set; }
        public ISpellStrategy SpellStrategy { get; set; }

        public CombatEngine(Unit me, Unit target, List<Spell> spells, ISpellStrategy spellStrategy)
        {
            Me = me;
            Target = target;
            Spells = spells;
            SpellStrategy = spellStrategy;
        }

        public string DoIteration()
        {
            return SpellStrategy.DoRoutine(Me, Target);
        }
    }
}
