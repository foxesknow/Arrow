using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Jobs
{
    public sealed partial class ZipDirectoryJob
    {
        public class Details
        {
            /// <summary>
            /// The directory to zip
            /// </summary>
            public string? From{get; set;}

            /// <summary>
            /// The name of the zip file to store it in
            /// </summary>
            public string? To{get; set;}

            /// <summary>
            /// True to delete the from directory on success
            /// </summary>
            public bool DeleteFrom{get; set;}
        }
    }
}
