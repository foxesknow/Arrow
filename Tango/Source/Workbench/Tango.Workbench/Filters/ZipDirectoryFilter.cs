using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using Arrow;
using Arrow.Calendar;
using Arrow.Execution;
using Arrow.IO;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Takes in a sequence of directories and zips them up
    /// </summary>
    [Filter("ZipDirectory")]
    public sealed class ZipDirectoryFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.ZipSpec is null) throw new WorkbenchException("no zip spec specified");

            return Zip(this.ZipSpec, items);
        }

        private async IAsyncEnumerable<object> Zip(string zipSpec, IAsyncEnumerable<object> items)
        {
            await foreach(var item in items)
            {
                _ = item switch
                {
                    string directory            => ZipDirectory(new(directory), zipSpec),
                    DirectoryInfo directoryInfo => ZipDirectory(directoryInfo, zipSpec), 
                    _                           => throw new WorkbenchException($"item does not specify a directory")
                };

                yield return item;
            }
        }

        private Unit ZipDirectory(DirectoryInfo directoryInfo, string zipSpec)
        {
            foreach(var matchingDirectory in DirectoryExpander.Expand(directoryInfo.FullName, DirectoryExpanderMode.OnlyExisting))
            {
                var zipFilename = GetZipFilename(matchingDirectory, zipSpec);

                MakeZipFileDirectory(zipFilename);

                if(File.Exists(zipFilename))
                {
                    if(this.SkipExisting)
                    {
                        Log.Info($"Skipping {zipFilename}");
                        continue;
                    }

                    if(this.Overwrite)
                    {
                        Log.Info($"deleting {zipFilename}");
                        File.Delete(zipFilename);
                    }
                    else
                    {
                        throw new WorkbenchException($"zip file already exists: {zipFilename}");
                    }
                }

                try
                {
                    Log.Info($"creating {zipFilename}");
                    ZipFile.CreateFromDirectory(matchingDirectory, zipFilename);
                }
                catch
                {
                    Log.Error($"failed to create {zipFilename}");
                    MethodCall.AllowFail(zipFilename, static filename => File.Delete(filename));
                    throw;
                }
            }

            return Unit.Default;
        }

        private string GetZipFilename(string sourceDirectoryInfo, string zipSpec)
        {
            var info = new DirectoryInfo(sourceDirectoryInfo);

            var today = Clock.Now;
            var expander = MakeExpander()
                           .Add("root", info.Name)
                           .Add("self", info.FullName);

            return expander.Expand(zipSpec);
        }

        private void MakeZipFileDirectory(string zipFilename)
        {
            var info = new FileInfo(zipFilename);
            info.Directory?.Create();
        }

        /// <summary>
        /// The file spec used to create the zip file.
        /// The following expansion variables are available:
        /// 
        /// $(root)     The last part of the directory being zipped. For c:\foo\bar its value is "bar"
        /// $(self)     The full name of the directory being zipped
        /// 
        /// As well as the usual date formats
        /// </summary>
        public string? ZipSpec{get; set;}

        public bool Overwrite{get; set;}

        public bool SkipExisting{get; set;}
    }
}
