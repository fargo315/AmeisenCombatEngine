using AmeisenCombatEngineCore.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenCombatEngineCore.Interfaces
{
    public interface ISpellStrategy
    {
        string DoRoutine(Unit me, Unit target);
    }
}
