using AmeisenCombatEngineCore.FSM.Enums;
using AmeisenCombatEngineCore.FSM.Interfaces;
using AmeisenCombatEngineCore.FSM.States;
using System.Collections.Generic;

namespace AmeisenCombatEngineCore.FSM
{
    public class AmeisenCombatFSM
    {
        public CombatState CurrentCombatState { get; set; }

        public Dictionary<CombatState, IState> CombatStates { get; private set; }

        public AmeisenCombatFSM()
        {
            CombatStates = new Dictionary<CombatState, IState>
            {
                { CombatState.Standing, new Standing() },
                { CombatState.Moving, new Moving() },
                { CombatState.Casting, new Casting() },
                { CombatState.Stunned, new Stunned() },
                { CombatState.Rooted, new Rooted() },
                { CombatState.Silenced, new Silenced() },
                { CombatState.Disarmed, new Disarmed() }
            };
        }

        public void DoThings() { CombatStates[CurrentCombatState].DoThings(); }
    }
}
