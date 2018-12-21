using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Structs;

namespace AmeisenCombatEngineCore.Strategies
{
    public class MovementCloseCombat : IMovementStrategy
    {
        double Distance { get; set; }

        public MovementCloseCombat(double distance = 2.0)
        {
            Distance = distance;
        }

        public Vector3 CalculatePosition(Unit me, Unit target)
        {
            if(me.GetDistanceToUnit(target) > Distance)
            {
                return target.Position;
            }

            return new Vector3(0.0, 0.0, 0.0);
        }
    }
}
