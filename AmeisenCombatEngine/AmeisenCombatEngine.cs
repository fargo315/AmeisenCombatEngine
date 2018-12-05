using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenCombatEngineCore
{
    public class CombatEngine
    {
        public List<Spell> Spells { get; set; }
        public Unit Me { get; set; }
        public Unit Target { get; set; }

        private readonly string learningDataFilepath = Environment.CurrentDirectory + "\\Data\\combatData.csv";

        public CombatEngine(Unit me, Unit target, List<Spell> spells)
        {
            Me = me;
            Target = target;
            Spells = spells;
        }

        public string DoIteration()
        {
            List<Spell> DamageSpells = Spells.Where(spell => spell.MainSpellType == SpellType.Damage).ToList();
            List<Spell> HealSpells = Spells.Where(spell => spell.MainSpellType == SpellType.Heal).ToList();

            if(Me.HealthPercentage < Target.HealthPercentage - 5 
                || Me.HealthPercentage < 25)
            {
                return HealSpells.OrderByDescending(
                    spell => spell.SpellImpacts.Where(
                        spellimpact => spellimpact.Key == SpellType.Heal).First().Value).First().SpellName;
            }
            else
            {
                return DamageSpells.OrderByDescending(
                    spell => spell.SpellImpacts.Where(
                        spellimpact => spellimpact.Key == SpellType.Damage).First().Value).First().SpellName;
            }
        }
    }
}
