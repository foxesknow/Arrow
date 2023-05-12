using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    /// <summary>
    /// An identifier for a request sent from a client to a server
    /// </summary>
    public sealed class RequestID : IEquatable<RequestID>
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="applicationID"></param>
        /// <param name="correlationID"></param>
        public RequestID(Guid applicationID, Guid correlationID)
        {
            this.ApplicationID = applicationID;
            this.CorrelationID = correlationID;
        }

        /// <summary>
        /// A unique identifier for the application sending a request to the server
        /// </summary>
        public Guid ApplicationID{get;}

        /// <summary>
        /// An application specific correlation id that is sent back
        /// </summary>
        public Guid CorrelationID{get;}

        /// <inheritdoc/>
        public bool Equals(RequestID? other)
        {
            return other is not null && 
                   this.ApplicationID == other.ApplicationID &&
                   this.CorrelationID == other.CorrelationID;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as RequestID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ApplicationID.GetHashCode(), this.CorrelationID.GetHashCode());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ApplicationID = {ApplicationID}, CorrelationID = {CorrelationID}";
        }
    }
}
