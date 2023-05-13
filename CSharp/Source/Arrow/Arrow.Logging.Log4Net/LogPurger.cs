using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.Execution;

namespace Arrow.Logging.Log4Net
{
    class LogPurger
    {
        public static void Purge(string purgeMask, int purgeDays)
        {
            if(purgeMask == null || purgeDays == 0) return;

            MethodCall.AllowFail(() =>
            {
                DateTime now = DateTime.UtcNow;

                string path = ".";
                string spec = null;
                int pivot = purgeMask.LastIndexOf('\\');
                if(pivot == -1)
                {
                    spec = purgeMask;
                }
                else
                {
                    path = purgeMask.Substring(0, pivot);
                    spec = purgeMask.Substring(pivot + 1);
                }

                string[] files = Directory.GetFiles(path, spec);
                foreach(string filename in files)
                {
                    DateTime lastWriteUtc = System.IO.File.GetLastWriteTimeUtc(filename);
                    TimeSpan delta = now - lastWriteUtc;
                    if(delta.TotalDays >= purgeDays)
                    {
                        System.IO.File.Delete(filename);
                    }
                }
            });
        }
    }
}
