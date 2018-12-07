using AmeisenCombatEngineCore.FSM.Enums;
using AmeisenCombatEngineCore.Structs;
using System;

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

        public Vector3 Position { get; set; }

        public Unit(
            double health, 
            double maxHealth, 
            double energy, 
            double maxEnergy, 
            CombatState combatState,
            Vector3 position)
        {
            Health = health;
            MaxHealth = maxHealth;
            Energy = energy;
            MaxEnergy = maxEnergy;
            CombatState = combatState;
            Position = position;
        }

        public double GetDistanceToUnit(Unit otherUnit) 
            => Math.Sqrt((Position.X - otherUnit.Position.X) * (Position.X - otherUnit.Position.X) +
                         (Position.Y - otherUnit.Position.Y) * (Position.Y - otherUnit.Position.Y) +
                         (Position.Z - otherUnit.Position.Z) * (Position.Z - otherUnit.Position.Z));
    }
}
