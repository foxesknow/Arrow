using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.FastLog
{
    public interface IFastLogger
    {
        public IFastLogLevel Debug{get;}
        public IFastLogLevel Info{get;}
        public IFastLogLevel Warn{get;}
        public IFastLogLevel Error{get;}
        public IFastLogLevel Fatal{get;}
    }
}
