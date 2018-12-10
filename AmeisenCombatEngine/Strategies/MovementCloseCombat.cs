using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Structs;

namespace AmeisenCombatEngineCore.Strategies
{
    public class MovementCloseCombat : IMovementStrategy
    {
        public Vector3 CalculatePosition(Unit me, Unit target)
        {
            if(me.GetDistanceToUnit(target) > 2.0)
            {
                return target.Position;
            }

            return new Vector3(0.0, 0.0, 0.0);
        }
    }
}
