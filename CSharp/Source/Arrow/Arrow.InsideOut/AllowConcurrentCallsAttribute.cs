using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Flags an InsideOut root node as allow multiple concurrent calls.
/// By default concurrent calls are not allowed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AllowConcurrentCallsAttribute : Attribute
{
}
