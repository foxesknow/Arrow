using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.IO
{
	/// <summary>
	/// Usefull path methods
	/// </summary>
	public static class PathEx
	{
		private static readonly char[] DirectorySeparators=new char[]{Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar};
		private static readonly char Wildcard='*';

		/// <summary>
		/// Expands a path expression into all matchig directories.
		/// For example c:\temp\*\bin will return all directories under c:\temp that have a child directory called bin
		/// </summary>
		/// <param name="pathExpression">A path expression that may contain a wildcard instead of a directory</param>
		/// <returns>The absolute paths of all matching directories</returns>
		public static IEnumerable<string> ExpandPath(string pathExpression)
		{
			if(pathExpression==null) throw new ArgumentNullException("pathExpression");
			if(string.IsNullOrWhiteSpace(pathExpression)) throw new ArgumentException("pathExpression");

			if(Path.IsPathRooted(pathExpression)==false)
			{
				pathExpression=Path.Combine(Directory.GetCurrentDirectory(),pathExpression);
			}
	
			Queue<string> active=new Queue<string>();
			Queue<string> swap=new Queue<string>();

			string[] parts=pathExpression.Split(DirectorySeparators,StringSplitOptions.RemoveEmptyEntries);

			int startPart=0;
			string? root=null;
			
			if(Path.IsPathRooted(parts[0])) 
			{
				// It's an absolute path
				root=string.Format("{0}{1}",parts[0],Path.DirectorySeparatorChar);
				startPart=1;
			}
			else
			{
				// It's relative to where we are.
				// By setting the root to the current directory all paths fill come back fully qualified
				root=Directory.GetCurrentDirectory();
				startPart=0;
			}

			active.Enqueue(root);

			for(int i=startPart; i<parts.Length && active.Count!=0; i++)
			{
				string subDir=parts[i];

				// Walk all the directories we expanded so far
				while(active.Count!=0)
				{
					string dir=active.Dequeue();

					if(subDir.Length==1 && subDir[0]==Wildcard)
					{
						// We need all the directories
						foreach(var child in Directory.GetDirectories(dir))
						{
							string fullpath=Path.GetFullPath(Path.Combine(dir,child));
							swap.Enqueue(fullpath);
						}
					}
					else
					{
						// We can ignore any that don't exist
						string fullpath=Path.GetFullPath(Path.Combine(dir,subDir));
						if(Directory.Exists(fullpath))
						{
							swap.Enqueue(fullpath);
						}
					}
				}

				var temp=active;
				active=swap;
				swap=temp;
			}

			foreach(var dir in active)
			{
				yield return dir;
			}
		}
	}
}
