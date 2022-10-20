using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
    /// <summary>
    /// Defines the readonly behaviour of a uri manager
    /// </summary>
    public interface IUriManager
    {
        /// <summary>
        /// Attempts to get a uri
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool TryGetUri(string name, [NotNullWhen(true)] out Uri? uri);

        /// <summary>
        /// Gets a uri, throwing an exception if it does not exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Uri GetUri(string name);
    }
}
