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
        public double FightingDistance { get; private set; }
        public double MinHpPercentageToUseSelfDamage { get; private set; }
        public bool ShouldHealOthers { get; private set; }

        /// <summary>
        /// Simple DPS Strategy
        /// 
        /// Does Damage while own HP is not under 25%, if
        /// it is the case it will heal itself
        /// </summary>
        public SpellSimple(List<Spell> spells, double minHp = 25, double minHpPercentageToUseSelfDamage = 60, bool shouldHealOthers = false)
        {
            Spells = spells;
            MinHp = minHp;
            ShouldHealOthers = shouldHealOthers;

            int count = 0;
            foreach (Spell s in Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Damage, int.MaxValue, 0))
            {
                FightingDistance += s.MaxRange;
                count++;
            }
            FightingDistance /= count;
        }

        public Spell DoRoutine(Unit me, Unit target)
        {
            double distance = me.GetDistanceToUnit(target);
            Spell spellToUse = null;

            List<Spell> ManaRestoreSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Manarestore, me.Energy, distance);
            if (me.EnergyPercentage < 10)
            {
                spellToUse = TryUseSpellFromSpellType(FilterSpellsThatWouldKillMe(me, ManaRestoreSpells), SpellType.Manarestore);
                if (spellToUse != null) { return spellToUse; }
            }

            List<Spell> Buffs = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Buff, me.Energy, distance);
            foreach (Spell spell in Buffs)
            {
                if (!me.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            List<Spell> Debuffs = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Debuff, me.Energy, distance);
            foreach (Spell spell in Debuffs)
            {
                if (!target.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            List<Spell> Dots = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Dot, me.Energy, distance);
            foreach (Spell spell in Dots)
            {
                if (!target.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            List<Spell> Hots = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Hot, me.Energy, distance);
            foreach (Spell spell in Hots)
            {
                if (!target.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            if (distance < FightingDistance / 2)
            {
                spellToUse = GetSpellsAndTryUseIt(SpellType.Gapbuilder, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
            }

            if (distance > FightingDistance)
            {
                spellToUse = GetSpellsAndTryUseIt(SpellType.Gapcloser, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
            }

            if (distance < 6.0)
            {
                spellToUse = GetSpellsAndTryUseIt(SpellType.Fear, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
            }

            if (distance < 3.0)
            {
                spellToUse = GetSpellsAndTryUseIt(SpellType.Stun, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
                spellToUse = GetSpellsAndTryUseIt(SpellType.Root, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
            }
            
            if (me.HealthPercentage < MinHp || (ShouldHealOthers && target.HealthPercentage < MinHp))
            {
                spellToUse = GetSpellsAndTryUseIt(SpellType.Heal, me.Energy, distance);
                if (spellToUse != null) { return spellToUse; }
            }

            spellToUse = GetSpellsAndTryUseIt(SpellType.Damage, me.Energy, distance);
            return spellToUse;
        }

        private Spell GetSpellsAndTryUseIt(SpellType spellType, double energy, double distance)
        {
            List<Spell> UseableSpells = Utils.GetAllUseableSpellsBySpellType(this.Spells, spellType, energy, distance);
            return TryUseSpellFromSpellType(UseableSpells, spellType);
        }

        private Spell TryUseSpellFromSpellType(List<Spell> availableSpells, SpellType type)
        {
            if (availableSpells.Count > 0)
            {
                try
                {
                    List<Spell> spells =
                    availableSpells.OrderByDescending(spell => spell.SpellImpacts
                        .Where(spellimpact => spellimpact.Key == type).First().Value).ToList()
                        .Where(spell => !spell.IsOnCooldown).ToList();

                    return spells.Any() ? spells.First() : null;
                }
                catch { return null; }
            }
            else
            {
                return null;
            }
        }

        private List<Spell> FilterSpellsThatWouldKillMe(Unit me, List<Spell> availableSpells)
        {
            List<Spell> filteredSpells = new List<Spell>();
            foreach (Spell spell in availableSpells)
            {
                double totalDamage = 0;
                foreach (KeyValuePair<SpellType, double> keyValuePair in spell.SpellImpacts)
                {
                    if (keyValuePair.Key == SpellType.Selfdamage)
                    {
                        totalDamage += keyValuePair.Value;
                    }
                }

                if ((((me.Health - totalDamage) / me.MaxHealth) * 100) >= MinHpPercentageToUseSelfDamage)
                {
                    filteredSpells.Add(spell);
                }
            }
            return filteredSpells;
        }
    }
}
