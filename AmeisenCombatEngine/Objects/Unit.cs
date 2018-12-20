using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Structs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AmeisenCombatEngineCore.Objects
{
    public class Unit
    {
        public double Health { get; set; }
        public double MaxHealth { get; set; }
        public double HealthPercentage => Health / MaxHealth * 100.0;

        public double Energy { get; set; }
        public double MaxEnergy { get; set; }
        public double EnergyPercentage => Energy / MaxEnergy * 100.0;
        
        public CombatState CombatState { get; private set; }
        public Dictionary<CombatState, Thread> CombatStateThreads { get; private set; }

        public Vector3 Position { get; set; }

        public List<string> Auras { get; set; }

        public Unit(
            double health,
            double maxHealth,
            double energy,
            double maxEnergy,
            CombatState combatState,
            Vector3 position,
            List<string> auras)
        {
            Health = health;
            MaxHealth = maxHealth;
            Energy = energy;
            MaxEnergy = maxEnergy;
            CombatState = combatState;
            Position = position;
            Auras = auras;
            CombatStateThreads = new Dictionary<CombatState, Thread>();
        }

        public void SetCombatState(CombatState state, int duration)
        {
            if (CombatStateThreads.ContainsKey(state) 
                && CombatStateThreads[state] != null)
            {
                if (CombatStateThreads[state].IsAlive)
                {
                    CombatStateThreads[state].Abort();
                }
            }

            CombatStateThreads[state] = new Thread(new ThreadStart(() => SwitchCombatStateInTime(CombatState.Standing, duration)));
            CombatState = state;
            CombatStateThreads[state].Start();
        }

        public double GetDistanceToPosition(Vector3 position)
            => Math.Sqrt((Position.X - position.X) * (Position.X - position.X) +
                         (Position.Y - position.Y) * (Position.Y - position.Y) +
                         (Position.Z - position.Z) * (Position.Z - position.Z));

        public double GetDistanceToUnit(Unit otherUnit)
            => GetDistanceToPosition(otherUnit.Position);

        private void SwitchCombatStateInTime(CombatState state, int msToWait) { Thread.Sleep(msToWait); CombatState = state; }
    }
}
