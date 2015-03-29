using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	interface IDirectoryNode : INode
	{
		IList<string> GetFiles();
		IList<string> GetDirectories();

		IDirectoryNode CreateDirectory(string name);
		IFileNode CreateFile(string name, Func<Stream> file);

		Stream OpenFile(string name);
		IDirectoryNode GetDirectory(string name);

		bool TryGetDirectory(string name, out IDirectoryNode directory);
		bool TryGetFile(string name, out IFileNode file);

		bool Delete(string name);
	}
}
