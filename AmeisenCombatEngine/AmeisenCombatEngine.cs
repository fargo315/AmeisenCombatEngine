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

        public ISpellStrategy SpellStrategy { get; set; }
        public IMovementStrategy MovementStrategy { get; set; }

        public event EventHandler OnCastSpell;
        public event EventHandler OnMoveCharacter;

        public CombatEngine(List<Spell> spells, ISpellStrategy spellStrategy, IMovementStrategy movementStrategy)
        {
            Spells = spells;
            SpellStrategy = spellStrategy;
            MovementStrategy = movementStrategy;
        }

        public void DoIteration(Unit me, Unit target)
        {
            if (me.CombatState == CombatState.Stunned)
            {
                // TODO: check for un-stun abilities
                return;
            }

            CastSpellEventArgs argsCast = new CastSpellEventArgs
            {
                Spell = SpellStrategy.DoRoutine(me, target)
            };
            if (argsCast.Spell != null)
            {
                OnCastSpell.Invoke(this, argsCast);
            }

            if (me.CombatState == CombatState.Rooted)
            {
                // TODO: check for un-root abilities
                return;
            }

            MoveCharacterEventArgs argsMove = new MoveCharacterEventArgs
            {
                PositionToGoTo = MovementStrategy.CalculatePosition(me, target)
            };
            if (!argsMove.PositionToGoTo.Equals(new Vector3(0.0, 0.0, 0.0)))
            {
                OnMoveCharacter?.Invoke(this, argsMove);
            }
        }
    }
}
