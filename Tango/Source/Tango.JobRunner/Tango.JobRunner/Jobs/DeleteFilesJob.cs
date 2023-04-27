using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Execution;
using Arrow.IO;

namespace Tango.JobRunner.Jobs
{
    /// <summary>
    /// The job deletes files which are over a particular age
    /// </summary>
    [Job("DeleteFiles")]
    public sealed class DeleteFilesJob : Job
    {
        public override ValueTask Run()
        {
            if(this.DaysOld < 0)
            {
                this.Score.ReportWarning("As DaysOld is less than zero nothing will be deleted");
                return default;
            }

            DeleteFiles();

            return default;
        }

        private void DeleteFiles()
        {
            var now = Clock.UtcNow;

            foreach(var spec in this.FilesToDelete)
            {
                var (directory, pattern) = SplitFilename(spec);

                foreach(var path in DirectoryExpander.Expand(directory))
                {
                    if(Directory.Exists(path) == false)
                    {
                        if(this.IgnoreMissing) continue;

                        throw new JobRunnerException($"directory does not exist: {path}");
                    }

                    try
                    {
                        var files = Directory.GetFiles(path, pattern);
                        foreach(var file in files)
                        {
                            TryToDeleteFile(now, file);
                        }
                    }
                    catch(Exception e)
                    {
                        if(this.IgnoreMissing == false)
                        {
                            this.Score.ReportError($"Error processing {path}. Message = {e.Message}");
                        }
                    }
                }
            }
        }

        private void TryToDeleteFile(DateTime now, string file)
        {
            var lastWrite = File.GetLastWriteTimeUtc(file);
            var timeSinceLastWrite = now - lastWrite;

            if(timeSinceLastWrite.TotalDays >= this.DaysOld)
            {
                Log.InfoFormat($"deleting {file}");
                MethodCall.AllowFail(file, static file => File.Delete(file));
            }
        }

        private (string Directory, string Filespec) SplitFilename(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if(directory is null) throw new JobRunnerException($"no directory in filename");

            var filespec = Path.GetFileName(filename);
            if(filespec is null) throw new JobRunnerException($"no filespec in filename");

            return (directory, filespec);
        }

        /// <summary>
        /// Deletes fils older than this number of days
        /// </summary>
        public int DaysOld{get; set;}

        /// <summary>
        /// The file spec to delete. Wildcards are supported
        /// </summary>
        public IReadOnlyList<string> FilesToDelete{get; init;} = Array.Empty<string>();

        /// <summary>
        /// True to ignore any missing directories
        /// </summary>
        public bool IgnoreMissing{get; set;}
    }
}
