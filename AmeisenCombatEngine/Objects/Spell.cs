using AmeisenCombatEngineCore.Enums;
using System.Collections.Generic;
using System.Threading;

namespace AmeisenCombatEngineCore.Objects
{
    public class Spell
    {
        public string SpellName { get; set; }
        public double EnergyCost { get; set; }
        public double MaxRange { get; set; }
        public int CooldownMs { get; set; }
        public SpellType MainSpellType { get; set; }
        public SpellExecution SpellExecution { get; set; }
        public Dictionary<SpellType, double> SpellImpacts { get; set; }
        public bool IsOnCooldown { get; set; }

        public Spell(
            string spellName,
            double energyCost,
            double maxRange,
            int cooldownMs,
            SpellType mainSpellType,
            SpellExecution spellExecution,
            Dictionary<SpellType, double> spellImpacts)
        {
            SpellName = spellName;
            EnergyCost = energyCost;
            CooldownMs = cooldownMs;
            MainSpellType = mainSpellType;
            SpellExecution = spellExecution;
            SpellImpacts = spellImpacts;
            MaxRange = maxRange;
        }

        public void StartCooldown() => new Thread(new ThreadStart(WaitOnCooldown)).Start();

        private void WaitOnCooldown()
        {
            IsOnCooldown = true;
            Thread.Sleep(CooldownMs);
            IsOnCooldown = false;
        }

        public override string ToString() => $"{SpellName} {MainSpellType} {SpellExecution}";
    }
}
