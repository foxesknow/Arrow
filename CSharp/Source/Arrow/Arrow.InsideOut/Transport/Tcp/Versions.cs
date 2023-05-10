using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Tcp
{
    static class Versions
    {
        public const int Version = 1;
        public static readonly byte[] VersionAsBuffer = BitConverter.GetBytes(Version);
    }
}
