using System;
using System.Collections.Generic;
using System.Linq;

namespace WaPesLeague.Data.Entities.Formation.Extensions
{
    public static class ListExtensions
    {
        public static int GetFormationIdByName(this IEnumerable<Formation> formations, string name)
        {
            return formations.Single(f => string.Equals(f.Name, name, StringComparison.InvariantCultureIgnoreCase)).FormationId;
        }
    }
}
