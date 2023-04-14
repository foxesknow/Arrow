using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Arrow.IO
{
    /// <summary>
    /// Takes a directory query and returns all matching directories.
    /// 
    /// For example, c:\Foo will just return c:\Foo
    /// However, C:\Foo\** will return all directories within C:\Foo
    /// In addition, C:\Foo\**\bin will return all directories that have a parent of C:\Foo and a child of \bin
    /// </summary>
    public static class DirectoryExpander
    {
        /// <summary>
        /// The directory expansion wildcard
        /// </summary>
        public static readonly string ExpansionPoint = "**";

        /// <summary>
        /// Returns all directories that match the query, using the OnlyExisting mode
        /// </summary>
        /// <param name="pathQuery"></param>
        /// <returns></returns>
        public static IEnumerable<string> Expand(string pathQuery)
        {
            return Expand(pathQuery, DirectoryExpanderMode.OnlyExisting);
        }

        /// <summary>
        /// Returns all directories that match the query
        /// </summary>
        /// <param name="pathQuery"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<string> Expand(string pathQuery, DirectoryExpanderMode mode)
        {
            if(pathQuery is null) throw new ArgumentNullException(nameof(pathQuery));
            if(string.IsNullOrWhiteSpace(pathQuery)) throw new ArgumentException("pathQuery is empty", nameof(pathQuery));
        
            return Execute(pathQuery, mode);

            static IEnumerable<string> Execute(string pathQuery, DirectoryExpanderMode mode)
            {
                var queue = new Queue<string>();
                queue.Enqueue(pathQuery);

                while(queue.Count != 0)
                {
                    var path = queue.Dequeue();

                    var pivot = path.IndexOf(ExpansionPoint);
                    if(pivot == -1)
                    {
                        // There's nothing left to expand
                        if(mode == DirectoryExpanderMode.OnlyExisting && Directory.Exists(path) == false)
                        {
                            continue;
                        }

                        yield return path;
                        continue;
                    }

                    // There's an expansion point, so we need to do a partial match
                    var directory = path.Substring(0, pivot);
                    var rest = path.Substring(pivot + ExpansionPoint.Length);

                    if(Directory.Exists(directory))
                    {
                        var subDirectories = Directory.GetDirectories(directory);

                        rest = rest.Trim();

                        // Remove any path separators
                        // NOTE: Windows is pretty forgiving and supports \ and /
                        if(rest.Length != 0 && (rest[0] == Path.DirectorySeparatorChar || rest[0] == Path.AltDirectorySeparatorChar))
                        {
                            rest = rest.Substring(1);
                        }

                        foreach(var subDirectory in subDirectories)
                        {
                            var newDirectory = Path.Combine(subDirectory, rest);
                            queue.Enqueue(newDirectory);
                        }
                    }
                }
            }
        }
    }
}
