using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.FastLog.Impl
{
    public sealed class FastLogger : IFastLogger
    {
        public FastLogger()
        {
            this.Debug = new FastLogLevel(this, LogLevel.Debug);
            this.Info = new FastLogLevel(this, LogLevel.Info);
            this.Warn = new FastLogLevel(this, LogLevel.Warn);
            this.Error = new FastLogLevel(this, LogLevel.Error);
            this.Fatal = new FastLogLevel(this, LogLevel.Fatal);
        }

        public IFastLogLevel Debug{get;}

        public IFastLogLevel Info{get;}

        public IFastLogLevel Warn{get;}

        public IFastLogLevel Error{get;}

        public IFastLogLevel Fatal{get;}
    }
}
