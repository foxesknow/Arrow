using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micron
{
    public sealed class RunJobData : JobData
    {
        public string App{get; init;} = "";
        
        public string Arguments{get; init;} = "";
        
        public string WorkingDirectory{get; init;} = "";
    }
}
