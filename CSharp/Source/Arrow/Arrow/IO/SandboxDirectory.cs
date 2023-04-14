using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.IO
{
	/// <summary>
	/// Treats a root directory as a sandbox, and allows you to apply a path
	/// to the sandbox so that the path is guaranteed not to break outside 
	/// the sandbox
	/// </summary>
	public sealed class SandboxDirectory
	{
		private static readonly char[] DirectorySeparators = new char[]{'/','\\'};

		private readonly string m_Root;
		private readonly string[] m_RootParts;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="root">The root directory of the sandbox</param>
		public SandboxDirectory(string root)
		{
            if(root == null) throw new ArgumentNullException("root");

            root = root.TrimStart();
            if(string.IsNullOrWhiteSpace(root)) throw new ArgumentException("root is empty", "root");

            m_Root = root;
            m_RootParts = root.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);

            if(m_RootParts.Length == 0) throw new ArgumentException("root is empty", "root");
        }

        /// <summary>
        /// The root of the sandbox
        /// </summary>
        public string Root
		{
			get{return m_Root;}
		}

        /// <summary>
        /// Applies the path to the sandbox root to create a new path relative to the sandbox
        /// </summary>
        /// <param name="path">The path to apply to the sandbox</param>
        /// <returns>A normalized path</returns>
        public string Normalize(string path)
        {
            if(path == null) throw new ArgumentNullException("path");

            var stack = new Stack<string>(m_RootParts);

            int rootDepth = stack.Count;

            foreach(var part in path.Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                switch(part.Trim())
                {
                    case ".":
                        // Do nothing
                        break;

                    case "..":
                        // Attempting to go outside the sandbox will keep us at the root of the sandbox.
                        // This is how DOS works, and it seems a reasonable default
                        if(stack.Count > rootDepth)
                        {
                            stack.Pop();
                        }
                        break;

                    default:
                        stack.Push(part);
                        break;
                }
            }

            var normalizedPath = string.Join(@"\", stack.Reverse());
            return normalizedPath;
        }

        /// <summary>
        /// Renders the sandbox as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
			return m_Root;
		}
	}
}
