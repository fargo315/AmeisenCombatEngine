using AmeisenCombatEngineCore.FSM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenCombatEngineCore.Objects
{
    public class Target : Unit
    {
        public Target(
            double health,
            double maxHealth,
            double energy,
            double maxEnergy,
            CombatState combatState
        ) : base(
                health,
                maxHealth,
                energy,
                maxEnergy,
                combatState
            )
        {

        }
    }
}
