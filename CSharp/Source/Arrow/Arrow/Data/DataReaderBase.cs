using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data
{
    public abstract class DataReaderBase : DbDataReader
    {
        protected long GetArray<T>(T[] source, long fieldOffset, T[]? buffer, int bufferOffset, int length)
        {
            if(buffer is null) return source.Length;

            var bytesCopied = 0;

            for(long sourceIndex = fieldOffset, destDelta = 0; sourceIndex < source.Length && destDelta < length; sourceIndex++, destDelta++)
            {
                buffer[bufferOffset + destDelta] = source[sourceIndex];
                bytesCopied++;
            }

            return bytesCopied;
        }

        protected DataTable? MakeSchemaTable(IDataReader reader)
        {
            return null;
        }
    }
}
