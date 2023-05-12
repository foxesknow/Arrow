using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    /// <summary>
    /// Identifies a process that published InsideOut data
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

            this.ServerName = serverName.ToLower();
            this.InstanceName = instanceName.ToLower();
        }

        /// <summary>
        /// The name of the server (always in lowercase)
        /// </summary>
        public string ServerName{get;}
        
        /// <summary>
        /// The name of the instance (always in lower case(
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
                   this.ServerName == other.ServerName &&
                   this.InstanceName == other.InstanceName;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PublisherID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ServerName.GetHashCode(), this.InstanceName.GetHashCode());
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
