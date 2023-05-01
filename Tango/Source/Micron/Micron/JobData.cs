using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micron
{
    public abstract class JobData
    {
        public string Name{get; init;} = "";

        public string Trigger{get; init;} = "";
    }
}
