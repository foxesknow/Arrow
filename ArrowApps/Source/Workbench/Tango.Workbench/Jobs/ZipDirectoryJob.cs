using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using Arrow.Calendar;
using System.Xml.Schema;
using Arrow.Execution;
using System.Threading;

namespace Tango.Workbench.Jobs
{
    /// <summary>
    /// Zips a directory into a zip archive, and optionally removes the source directory
    /// </summary>
    [Job("ZipDirectory")]
    public sealed partial class ZipDirectoryJob : Job
    {
        public override async ValueTask Run()
        {
            var now = Clock.Now.Date;

            var start = this.GoBack;
            var stop = start + this.DaysToCheck;

            using(var throttle = new SemaphoreSlim(this.Throttle, this.Throttle))
            {
                foreach(var details in this.Directories)            
                {
                    if(details.From is null || details.To is null) continue;

                    var tasks = new List<Task>();
                    for(var delta = start; delta < stop; delta++)
                    {
                        var dateToArchive = now.AddDays(-delta);

                        var expander = MakeExpander();
                        expander.AddDates(dateToArchive);

                        var fromDirectory = expander.Expand(details.From);
                        var toZip = expander.Expand(details.To);

                        if(Directory.Exists(fromDirectory))
                        {
                            var task = ThrottledRun(throttle, () => ArchiveDirectory(fromDirectory, toZip, details.DeleteFrom));
                            tasks.Add(task);
                        }
                        else
                        {
                            VerboseLog.Warn($"{fromDirectory} does not exist");
                        }
                    }

                    try
                    {
                        await Task.WhenAll(tasks);
                    }                                
                    catch(Exception e)
                    {
                        this.Score.ReportError(e);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Runs an action as a task when there is room in the throttle
        /// </summary>
        /// <param name="semaphore"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private Task ThrottledRun(SemaphoreSlim semaphore, Action action)
        {
            semaphore.Wait();

            return Task.Run(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }

        /// <summary>
        /// Archives a directory to a zip file
        /// </summary>
        /// <param name="fromDirectory"></param>
        /// <param name="toZip"></param>
        /// <param name="deleteOnSuccess"></param>
        private void ArchiveDirectory(string fromDirectory, string toZip, bool deleteOnSuccess)
        {
            if(File.Exists(toZip) && this.OverwriteZip == false)
            {
                VerboseLog.Info($"{toZip} already exists, skipping");
                return;
            }

            if(File.Exists(toZip) && this.OverwriteZip)
            {
                VerboseLog.Info($"{toZip} already exists, deleting");
                File.Delete(toZip);
            }

            MakeZipFileDirectory(toZip);

            Log.Info($"zipping {fromDirectory} into {toZip}");

            try
            {
                ZipFile.CreateFromDirectory(fromDirectory, toZip);
            }
            catch
            {
                // If there's a problem zipping the files then we can still get a 
                // zip file, but it's in an undefined state. It's best to remove it.
                MethodCall.AllowFail(toZip, static file => File.Delete(file));
                
                throw;
            }
            
            Log.Info($"zipped {fromDirectory} into {toZip}");

            if(deleteOnSuccess)
            {
                Log.Info($"deleting {fromDirectory}");
                
                // We won't class this as an error as we have at least created the zip
                MethodCall.AllowFail(fromDirectory, static directory => Directory.Delete(directory, true));
            }
        }

        private void MakeZipFileDirectory(string zipFilename)
        {
            var info = new FileInfo(zipFilename);
            info.Directory?.Create();
        }

        /// <summary>
        /// The directories to zip
        /// </summary>
        public List<Details> Directories{get;} = new();

        /// <summary>
        /// True to overwrite the zip file if it already exists
        /// </summary>
        public bool OverwriteZip{get; set;}

        /// <summary>
        /// How many days to go back from today to start looking for files
        /// </summary>
        public int GoBack{get; set;} = 0;

        /// <summary>
        /// How many days to check to see if we need to archive
        /// </summary>
        public int DaysToCheck{get; set;} = 1;

        /// <summary>
        /// How many zip tasks to run in parallel
        /// </summary>
        public int Throttle{get; set;} = 8;
    }
}
