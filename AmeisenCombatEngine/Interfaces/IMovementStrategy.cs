using AmeisenCombatEngineCore.Objects;
using AmeisenCombatEngineCore.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenCombatEngineCore.Interfaces
{
    public interface IMovementStrategy
    {
        Vector3 CalculatePosition(Unit me, Unit target);
    }
}
