using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Arrow.Execution;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Base class for filters that write something to a file
    /// </summary>
    public abstract class FileFilterBase : Filter
    {
        public sealed override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Filename is null) throw new ArgumentNullException(nameof(Filename));

            var filename = MakeExpander().Expand(this.Filename);

            if(File.Exists(filename))
            {
                if(this.SkipExisting)
                {
                    Log.Info($"Skipping {filename}");
                    return items;
                }

                if(this.Overwrite)
                {
                    Log.Info($"deleting {filename}");
                    File.Delete(filename);
                }
                else
                {
                    throw new WorkbenchException($"file already exists: {filename}");
                }
            }

            return WriteToFile(this.Filename, items);
        }

        /// <summary>
        /// Deletes the file if a rollback occurs
        /// </summary>
        /// <param name="filename"></param>
        protected void RegisterRollbackFile(string filename)
        {
            this.Context.RegisterRollback(() =>
            {
                Log.Error($"deleting {filename}");
                File.Delete(filename);

                return Array.Empty<Exception>();
            });
        }

        protected abstract IAsyncEnumerable<object> WriteToFile(string filename, IAsyncEnumerable<object> items);

        /// <summary>
        /// The file to write to
        /// </summary>
        public string? Filename{get; set;}

        /// <summary>
        /// True to overwrite the file.
        /// If false then if the file already exists an exception is thrown
        /// </summary>
        public bool Overwrite{get; set;}

        /// <summary>
        /// True to skip writing to file if the file already exists
        /// </summary>
        public bool SkipExisting{get; set;}
    }
}
