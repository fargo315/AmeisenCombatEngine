using AmeisenCombatEngineCore.Enums;
using AmeisenCombatEngineCore.Interfaces;
using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Structs;
using System;
using System.Collections.Generic;

namespace AmeisenCombatEngineCore
{
    public class CombatEngine
    {
        public List<Spell> Spells { get; set; }

        public Unit Me { get; set; }
        public Unit Target { get; set; }

        public ISpellStrategy SpellStrategy { get; set; }
        public IMovementStrategy MovementStrategy { get; set; }

        public event EventHandler OnCastSpell;
        public event EventHandler OnMoveCharacter;

        public CombatEngine(Unit me, Unit target, List<Spell> spells, ISpellStrategy spellStrategy, IMovementStrategy movementStrategy)
        {
            Me = me;
            Target = target;
            Spells = spells;
            SpellStrategy = spellStrategy;
            MovementStrategy = movementStrategy;
        }

        public void DoIteration()
        {
            if (Me.CombatState == CombatState.Stunned)
            {
                // TODO: check for un-stun abilities
                return;
            }

            CastSpellEventArgs argsCast = new CastSpellEventArgs
            {
                Spell = SpellStrategy.DoRoutine(Me, Target)
            };
            if (argsCast.Spell != null)
            {
                OnCastSpell.Invoke(this, argsCast);
            }

            if (Me.CombatState == CombatState.Rooted)
            {
                // TODO: check for un-root abilities
                return;
            }

            MoveCharacterEventArgs argsMove = new MoveCharacterEventArgs
            {
                PositionToGoTo = MovementStrategy.CalculatePosition(Me, Target)
            };
            if (!argsMove.PositionToGoTo.Equals(new Vector3(0.0, 0.0, 0.0)))
            {
                OnMoveCharacter.Invoke(this, argsMove);
            }
        }
    }
}
