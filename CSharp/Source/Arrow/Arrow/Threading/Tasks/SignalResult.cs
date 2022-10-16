using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public readonly struct SignalResult
    {
        public SignalResult(int successful, int failed, int threw)
        {
            this.Successful = successful;
            this.Failed = failed;
            this.Threw = threw;
        }

        public int Successful{get;}

        public int Failed{get;}

        public int Threw{get;}
    }
}
