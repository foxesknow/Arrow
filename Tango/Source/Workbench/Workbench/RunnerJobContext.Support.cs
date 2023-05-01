using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workbench
{
    internal partial class RunnerJobContext
    {
        /// <summary>
        /// Does a case insensitive comparision of a database identifier
        /// </summary>
        private sealed class ScopedDatabaseComparer : IEqualityComparer<(long ScopeID, string Name)>
        {
            public static readonly IEqualityComparer<(long ScopeID, string Name)> Instance = new ScopedDatabaseComparer();

            public bool Equals((long ScopeID, string Name) x, (long ScopeID, string Name) y)
            {
                return x.ScopeID == y.ScopeID && StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);
            }

            public int GetHashCode([DisallowNull] (long ScopeID, string Name) obj)
            {
                return HashCode.Combine(obj.ScopeID.GetHashCode(), StringComparer.OrdinalIgnoreCase.GetHashCode());
                
            }
        }
    }
}
