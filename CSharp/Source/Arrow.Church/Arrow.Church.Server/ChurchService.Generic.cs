using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Server
{
	public abstract class ChurchService<TServiceInterface> : ChurchService where TServiceInterface:class
	{
		protected ChurchService() : base(typeof(TServiceInterface))
		{
		}		
	}
}
