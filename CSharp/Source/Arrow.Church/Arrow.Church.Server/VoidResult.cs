using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	public sealed class VoidResult
	{
		public static readonly VoidResult Instance=new VoidResult();

		private VoidResult()
		{
		}

		public override string ToString()
		{
			return "void";
		}
	}
}
