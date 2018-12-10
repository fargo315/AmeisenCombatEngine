using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenCombatEngineCore.Strategies
{
    public class SpellSimple : ISpellStrategy
    {
        private List<Spell> Spells { get; set; }
        private double MinHp { get; set; }

        /// <summary>
        /// Simple DPS Strategy
        /// 
        /// Does Damage while own HP is not under 25%, if
        /// it is the case it will heal itself
        /// </summary>
        public SpellSimple(List<Spell> spells, double minHp = 25)
        {
            Spells = spells;
            MinHp = minHp;
        }

        public Spell DoRoutine(Unit me, Unit target)
        {
            double distance = me.GetDistanceToUnit(target);
            List<Spell> DamageSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Damage, me.Energy, distance);
            List<Spell> HealSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Heal, me.Energy, distance);

            Spell spellToUse = null;

            if (me.HealthPercentage < MinHp)
            {
                if (HealSpells.Count > 0)
                {
                    List<Spell> healingSpells =
                    HealSpells.OrderByDescending(spell => spell.SpellImpacts
                        .Where(spellimpact => spellimpact.Key == SpellType.Heal).First().Value).ToList()
                        .Where(spell => !spell.IsOnCooldown).ToList();

                    spellToUse = healingSpells.Any() ? healingSpells.First() : null;
                }
                else
                {
                    spellToUse = null;
                }
            }

            if (spellToUse != null)
            {
                return spellToUse;
            }

            if (DamageSpells.Count > 0)
            {
                List<Spell> damageSpells =
                DamageSpells.OrderByDescending(spell => spell.SpellImpacts
                    .Where(spellimpact => spellimpact.Key == SpellType.Damage).First().Value).ToList()
                    .Where(spell => !spell.IsOnCooldown).ToList();

                spellToUse = damageSpells.Any() ? damageSpells.First() : null;
            }
            else
            {
                spellToUse = null;
            }
            return spellToUse;
        }
    }
}
