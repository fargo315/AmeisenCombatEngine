using AmeisenCombatEngineCore.FSM.Enums;

namespace AmeisenCombatEngineCore.Objects
{
    public class Me : Unit
    {
        public Me(
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
