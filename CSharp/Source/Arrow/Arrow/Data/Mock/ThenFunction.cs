using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public delegate T ThenFunction<T>(DbCommand command);
}
