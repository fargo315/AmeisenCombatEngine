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

        /// <summary>
        /// Simple DPS Strategy
        /// 
        /// Does Damage while own HP is not under 25%, if
        /// it is the case it will heal itself
        /// </summary>
        public SpellSimple(List<Spell> spells, double minHp = 25, double minHpPercentageToUseSelfDamage = 60)
        {
            Spells = spells;
            MinHp = minHp;

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

            List<Spell> Buffs = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Buff, me.Energy, distance);
            List<Spell> Debuffs = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Debuff, me.Energy, distance);

            List<Spell> Dots = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Dot, me.Energy, distance);

            List<Spell> DamageSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Damage, me.Energy, distance);
            List<Spell> HealSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Heal, me.Energy, distance);
            List<Spell> ManaRestoreSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Manarestore, me.Energy, distance);

            List<Spell> GapcloserSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Gapcloser, me.Energy, distance);
            List<Spell> GapbuilderSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Gapbuilder, me.Energy, distance);

            List<Spell> FearSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Fear, me.Energy, distance);
            List<Spell> StunSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Stun, me.Energy, distance);
            List<Spell> RootSpells = Utils.GetAllUseableSpellsBySpellType(Spells, SpellType.Root, me.Energy, distance);

            Spell spellToUse = null;

            if (me.EnergyPercentage < 10)
            { spellToUse = TryUseSpellFromSpellType(FilterSpellsThatWouldKillMe(me, ManaRestoreSpells), SpellType.Manarestore); }
            if (spellToUse != null) { return spellToUse; }

            foreach (Spell spell in Buffs)
            {
                if (!me.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    break;
                }
            }
            if (spellToUse != null) { return spellToUse; }

            foreach (Spell spell in Debuffs)
            {
                if (!target.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    break;
                }
            }
            if (spellToUse != null) { return spellToUse; }

            foreach (Spell spell in Dots)
            {
                if (!target.Auras.Contains(spell.SpellName.ToLower()))
                {
                    spellToUse = spell;
                    break;
                }
            }
            if (spellToUse != null) { return spellToUse; }

            if (distance < FightingDistance / 2)
            { spellToUse = TryUseSpellFromSpellType(GapbuilderSpells, SpellType.Gapbuilder); }
            if (spellToUse != null) { return spellToUse; }

            if (distance > FightingDistance)
            { spellToUse = TryUseSpellFromSpellType(GapcloserSpells, SpellType.Gapcloser); }
            if (spellToUse != null) { return spellToUse; }

            if (distance < 5.0)
            { spellToUse = TryUseSpellFromSpellType(FearSpells, SpellType.Fear); }
            if (spellToUse != null) { return spellToUse; }

            if (distance < 3.0)
            { spellToUse = TryUseSpellFromSpellType(StunSpells, SpellType.Stun); }
            if (spellToUse != null) { return spellToUse; }

            if (distance < 3.0)
            { spellToUse = TryUseSpellFromSpellType(RootSpells, SpellType.Root); }
            if (spellToUse != null) { return spellToUse; }

            if (me.HealthPercentage < MinHp)
            { spellToUse = TryUseSpellFromSpellType(HealSpells, SpellType.Heal); }
            if (spellToUse != null) { return spellToUse; }

            spellToUse = TryUseSpellFromSpellType(DamageSpells, SpellType.Damage);
            return spellToUse;
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
