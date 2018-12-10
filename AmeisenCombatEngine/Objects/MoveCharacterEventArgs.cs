using AmeisenCombatEngineCore.Structs;
using System;

namespace AmeisenCombatEngineCore.Objects
{
    public class MoveCharacterEventArgs : EventArgs
    {
        public Vector3 PositionToGoTo { get; set; }
    }
}
