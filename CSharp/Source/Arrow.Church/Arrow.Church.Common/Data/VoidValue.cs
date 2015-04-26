using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Data
{
	[Serializable]
	public sealed class VoidValue
	{
		public static readonly VoidValue Instance=new VoidValue();

		private VoidValue()
		{
		}

		public override string ToString()
		{
			return "void";
		}
	}
}
