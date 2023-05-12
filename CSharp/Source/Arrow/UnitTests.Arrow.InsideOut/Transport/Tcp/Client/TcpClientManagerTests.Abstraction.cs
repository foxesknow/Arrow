using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport.Tcp.Client;

namespace UnitTests.Arrow.InsideOut.Transport.Tcp.Client
{
    public partial class TcpClientManagerTests
    {
        private class NetworkClient : INetworkClient
        {
            private readonly Stream m_Stream;

            public NetworkClient(Stream stream)
            {
                m_Stream = stream;
            }

            public Task Connect(string host, int port)
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {
            }

            public Stream GetStream()
            {
                return m_Stream;
            }
        }
    }
}
