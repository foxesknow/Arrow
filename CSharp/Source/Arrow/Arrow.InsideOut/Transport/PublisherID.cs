using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    /// <summary>
    /// Identifies a process that published InsideOut data.
    /// The case of the server and instance name are retained.
    /// However, they are treated as case insenstive when being compared
    /// </summary>
    public sealed class PublisherID : IEquatable<PublisherID>
    {
        private const char Divider = '@';

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="instanceName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public PublisherID(string serverName, string instanceName)
        {
            if(serverName is null) throw new ArgumentNullException(nameof(serverName));
            if(string.IsNullOrWhiteSpace(serverName)) throw new ArgumentException("invalid server name", nameof(serverName));

            if(instanceName is null) throw new ArgumentNullException(nameof(instanceName));
            if(string.IsNullOrWhiteSpace(instanceName)) throw new ArgumentException("invalid instance name", nameof(instanceName));

            this.ServerName = serverName;
            this.InstanceName = instanceName;
        }

        /// <summary>
        /// The name of the server
        /// </summary>
        public string ServerName{get;}
        
        /// <summary>
        /// The name of the instance
        /// </summary>
        public string InstanceName{get;}

        /// <summary>
        /// Encode an instance as a string
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            return $"{ServerName}{Divider}{InstanceName}";
        }

        /// <inheritdoc/>
        public bool Equals(PublisherID? other)
        {
            return other is not null &&
                   StringComparer.OrdinalIgnoreCase.Equals(this.ServerName, other.ServerName) &&
                   StringComparer.OrdinalIgnoreCase.Equals(this.InstanceName, other.InstanceName);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PublisherID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine
            (
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.ServerName),
                StringComparer.OrdinalIgnoreCase.GetHashCode(this.InstanceName)
            );
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ServerName = {ServerName}, InstanceName = {InstanceName}";
        }

        /// <summary>
        /// Decodes a PublisherID that was encoded via PublisherID.Encode
        /// </summary>
        /// <param name="encodedValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InsideOutException"></exception>
        public static PublisherID Decode(string encodedValue)
        {
            if(encodedValue is null) throw new ArgumentNullException(nameof(encodedValue));

            var pivot = encodedValue.IndexOf(Divider);
            if(pivot == -1) throw new InsideOutException("invalid PublisherID encoding");

            var serverName = encodedValue.Substring(0, pivot);
            var instanceName = encodedValue.Substring(pivot + 1);

            return new(serverName, instanceName);
        }
    }
}
