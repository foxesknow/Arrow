using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Tcp.Client;

internal interface INetworkClient : IDisposable
{
    /// <summary>
    /// Connects to a server
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public Task Connect(string host, int port);

    /// <summary>
    /// Returns a stream that can be read and written to
    /// </summary>
    /// <returns></returns>
    public Stream GetStream();
}
