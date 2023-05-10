using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Tcp.Client;

internal class TcpNetworkClient : INetworkClient
{
    private TcpClient m_Client = new();

    public Task Connect(string host, int port)
    {
        return m_Client.ConnectAsync(host, port);
    }

    public Stream GetStream()
    {
        return m_Client.GetStream();
    }

    public void Dispose()
    {
        m_Client.Dispose();
    }
}
