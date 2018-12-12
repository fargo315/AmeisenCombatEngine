using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Structs;
using System;

namespace AmeisenCombatEngineCore.Strategies
{
    public class MovementDefensiveRanged : IMovementStrategy
    {
        private Vector3 InitialPosition { get; set; }

        public MovementDefensiveRanged(Vector3 initialPosition)
        {
            InitialPosition = initialPosition;
        }

        public Vector3 CalculatePosition(Unit me, Unit target)
        {
            if (me.GetDistanceToUnit(target) < 8.0)
            {
                Vector3 mePosition = me.Position;
                Vector3 enemyPosition = target.Position;
                int movementValue = 2;

                /*if (me.GetDistanceToPosition(InitialPosition) > 25)
                {
                    movementValue = -2;
                }*/

                if (mePosition.X < enemyPosition.X)
                {
                    mePosition.X -= movementValue;
                }

                if (mePosition.Y < enemyPosition.Y)
                {
                    mePosition.Y -= movementValue;
                }

                if (mePosition.X > enemyPosition.X)
                {
                    mePosition.X += movementValue;
                }

                if (mePosition.Y > enemyPosition.Y)
                {
                    mePosition.Y += movementValue;
                }

                if (mePosition.X == enemyPosition.X)
                {
                    int maxMVal = movementValue + 1;
                    if (movementValue < 0)
                    {
                        maxMVal = (movementValue * -1) + 1;
                    }

                    mePosition.X += new Random().Next(movementValue, maxMVal);
                }

                if (mePosition.Y == enemyPosition.Y)
                {
                    int maxMVal = movementValue + 1;
                    if (movementValue < 0)
                    {
                        maxMVal = (movementValue * -1) + 1;
                    }

                    mePosition.Y += new Random().Next(movementValue, maxMVal);
                }

                return mePosition;
            }

            return new Vector3(0.0, 0.0, 0.0);
        }
    }
}
