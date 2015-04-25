using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	delegate Task<object> ServiceMethod(ChurchServiceBase service, object argument);
}
