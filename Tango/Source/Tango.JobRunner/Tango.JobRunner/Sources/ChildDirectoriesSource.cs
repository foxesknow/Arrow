using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.IO;

namespace Tango.Workbench.Sources
{
    /// <summary>
    /// Returns all directory names that match a given directory spec
    /// </summary>
    [Source("ChildDirectories")]
    public sealed class ChildDirectoriesSource : Source
    {
        public override async IAsyncEnumerable<object> Run()
        {
            if(this.DirectorySpec is null) throw new WorkbenchException($"no directory spec specified");

            await ForceAsync();

            foreach(var directory in DirectoryExpander.Expand(this.DirectorySpec, DirectoryExpanderMode.OnlyExisting))
            {
                yield return new DirectoryInfo(directory);
            }
        }

        /// <summary>
        /// The directory spec to return.
        /// The Arrow DirectoryExpander class is used to generate the names of the directories
        /// </summary>
        public string? DirectorySpec{get; set;}
    }
}
