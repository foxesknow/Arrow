using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Collections;
using Arrow.IO;

namespace Tango.Workbench.Sources
{
    /// <summary>
    /// Returns all directory names that match a given directory spec
    /// 
    /// NOTE: All directories are resolved before the sequence is returned.
    /// This means that subsequent filters can update any source directories 
    /// without seeing their changes appear in the sequence.
    /// </summary>
    [Source("ChildDirectories")]
    public sealed class ChildDirectoriesSource : Source
    {
        public override IAsyncEnumerable<object> Run()
        {
            if(this.DirectorySpec is null) throw new WorkbenchException($"no directory spec specified");

            var directoryInfo = DirectoryExpander.Expand(this.DirectorySpec, DirectoryExpanderMode.OnlyExisting).ToList();
            return directoryInfo.ToAsyncEnumerable();
        }

        /// <summary>
        /// The directory spec to return.
        /// The Arrow DirectoryExpander class is used to generate the names of the directories
        /// </summary>
        public string? DirectorySpec{get; set;}
    }
}
