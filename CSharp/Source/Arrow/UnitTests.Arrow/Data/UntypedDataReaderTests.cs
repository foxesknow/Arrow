using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

using NUnit.Framework;

namespace UnitTests.Arrow.Data
{
    [TestFixture]
    public class UntypedDataReaderTests
    {
        [Test]
        public void MakeSingleColumn_Null()
        {
            Assert.Catch(() => UntypedDataReader.MakeSingleColumn(null, new[]{"world"}));
            Assert.Catch(() => UntypedDataReader.MakeSingleColumn("Where", null));
        }

        [Test]
        public void MakeSingleRow_InvalidColumnnames()
        {
            Assert.Catch(() => UntypedDataReader.MakeSingleRow(null, new[]{"hello", "world"}));
            Assert.Catch(() => UntypedDataReader.MakeSingleRow(new string[]{null, null}, new[]{"hello", "world"}));
            Assert.Catch(() => UntypedDataReader.MakeSingleRow(new string[]{"  ", null}, new[]{"hello", "world"}));
            Assert.Catch(() => UntypedDataReader.MakeSingleRow(new string[]{"greeting", null}, new[]{"hello", "world"}));
        }

        [Test]
        public void MakeSingleRow_IncorrectWidth()
        {
            Assert.Catch(() => UntypedDataReader.MakeSingleRow(new[]{"A", "B", "C"}, new[]{"hello", "world"}));
        }
    }
}
