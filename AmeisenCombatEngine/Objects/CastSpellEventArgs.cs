using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenCombatEngineCore.Objects
{
    public class CastSpellEventArgs : EventArgs
    {
        public Spell Spell { get; set; }
    }
}
