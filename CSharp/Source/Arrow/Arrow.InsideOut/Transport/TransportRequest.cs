using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    public sealed class TransportRequest
    {
        public TransportRequest(NodeFunction nodeFunction, PublisherID publisherID, RequestID requestID)
        {
            if(publisherID is null) throw new ArgumentNullException(nameof(publisherID));
            if(requestID is null) throw new ArgumentNullException(nameof(requestID));

            this.PublisherID = publisherID;
            this.RequestID = requestID;
            this.NodeFunction = nodeFunction;
        }

        /// <summary>
        /// The publisher the request is intended for.
        /// </summary>
        public PublisherID PublisherID{get;}

        /// <summary>
        /// Client side request information
        /// </summary>
        public RequestID RequestID{get;}

        /// <summary>
        /// Which function the client wants to execute
        /// </summary>
        public NodeFunction NodeFunction{get;}

        /// <summary>
        /// And request data for the function
        /// </summary>
        public RequestBase? Request{get; set;}

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.NodeFunction.ToString();
        }
    }
}
