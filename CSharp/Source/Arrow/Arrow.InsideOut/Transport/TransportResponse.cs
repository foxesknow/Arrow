using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    public sealed class TransportResponse
    {
        public TransportResponse(NodeResponse nodeResponse, RequestID requestID, ResponseBase response)
        {
            if(requestID is null) throw new ArgumentNullException(nameof(requestID));
            if(response is null) throw new ArgumentNullException(nameof(response));

            this.NodeResponse = nodeResponse;
            this.RequestID = requestID;
            this.Response = response;
        }

        /// <summary>
        /// The request details sent by the client
        /// </summary>
        public RequestID RequestID{get;}

        /// <summary>
        /// The type of the response
        /// </summary>
        public NodeResponse NodeResponse{get;}

        /// <summary>
        /// The data being sent back
        /// </summary>
        public ResponseBase Response{get;}

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.NodeResponse.ToString();
        }
    }
}
