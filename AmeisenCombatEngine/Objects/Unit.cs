using AmeisenCombatEngineCore.FSM.Enums;

namespace AmeisenCombatEngineCore.Objects
{
    public class Unit
    {
        public double Health { get; set; }
        public double MaxHealth { get; set; }
        public double HealthPercentage => (double)Health / (double)MaxHealth * 100.0;

        public double Energy { get; set; }
        public double MaxEnergy { get; set; }
        public double EnergyPercentage => (double)Energy / (double)MaxEnergy * 100.0;

        public CombatState CombatState { get; set; }

        public Unit(
            double health, 
            double maxHealth, 
            double energy, 
            double maxEnergy, 
            CombatState combatState)
        {
            Health = health;
            MaxHealth = maxHealth;
            Energy = energy;
            MaxEnergy = maxEnergy;
            CombatState = combatState;
        }
    }
}
