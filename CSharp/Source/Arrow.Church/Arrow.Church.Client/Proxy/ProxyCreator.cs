﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.Proxy
{
	public delegate object ProxyCreator(Uri endpoint, string serviceName);
}