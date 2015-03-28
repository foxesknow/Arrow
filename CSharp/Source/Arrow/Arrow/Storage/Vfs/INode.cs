using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	interface INode
	{
		bool IsFile{get;}
		bool IsDirectory{get;}		
	}
}
