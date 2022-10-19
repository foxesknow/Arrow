using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Threading
{
    /// <summary>
    /// Combines 2 cancellation tokens into 1 in a memory efficient way
    /// </summary>
    public static class CombineCancellationToken
    {
        /// <summary>
        /// Combines 2 cancellation tokens into one, avoiding a memory allocation if either token is the default
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="combinedToken"></param>
        /// <returns>A disposable object that will free up any resources</returns>
        public static IDisposable? Make(CancellationToken lhs, CancellationToken rhs, out CancellationToken combinedToken)
        {
            if(lhs == default)
            {
                combinedToken = rhs;
                return null;
            }

            if(rhs == default)
            {
                combinedToken = lhs;
                return null;
            }

            // As they're both non-default we need to combine them
            var source = CancellationTokenSource.CreateLinkedTokenSource(lhs, rhs);
            combinedToken = source.Token;

            return source;
        }
    }
}
