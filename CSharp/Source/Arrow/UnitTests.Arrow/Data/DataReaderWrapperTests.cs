using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Execution;
using NUnit.Framework;

namespace UnitTests.Arrow.Data
{
    [TestFixture]
    public class DataReaderWrapperTests
    {
        [Test]
        public void Initialization_NullReader()
        {
            Assert.Catch(() => new DataReaderWrapper(null));
            Assert.Catch(() => new DataReaderWrapper(DataReaderWrapperCloseMode.Reader, null));
        }

        [Test]
        public void PropertiesAndMethods()
        {
            var now = DateTime.Now;
            var guid = Guid.NewGuid();

            var outer = UntypedDataReader.MakeSingleRow
            (
                new string[]{"Name", "bool", "byte", "int16", "int32", "int64", "float", "double", "decimal", "char", "DateTime", "Guid"},
                new object[]{"Jack", "true", "1",    "2",     "4",     "8",     "10.0",  "20.0",   "40.0",    "X",    now,        guid}
            );

            using(var reader = new DataReaderWrapper(outer))
            {
                Assert.That(reader.Read(), Is.True);
                
                Assert.That(reader.FieldCount, Is.EqualTo(12));
                Assert.That(reader.IsClosed, Is.False);
                Assert.That(reader.Depth, Is.EqualTo(0));
                Assert.That(reader.RecordsAffected, Is.EqualTo(-1));
                Assert.That(reader.GetSchemaTable, Is.Null);

                Assert.That(reader.GetValue(0), Is.EqualTo("Jack"));

                Assert.That(reader.GetString(0), Is.EqualTo("Jack"));
                Assert.That(reader.GetBoolean(1), Is.True);
                Assert.That(reader.GetByte(2), Is.EqualTo(1));
                Assert.That(reader.GetInt16(3), Is.EqualTo(2));
                Assert.That(reader.GetInt32(4), Is.EqualTo(4));
                Assert.That(reader.GetInt64(5), Is.EqualTo(8));
                Assert.That(reader.GetFloat(6), Is.EqualTo(10.0f));
                Assert.That(reader.GetDouble(7), Is.EqualTo(20.0d));
                Assert.That(reader.GetDecimal(8), Is.EqualTo(40m));                
                Assert.That(reader.GetChar(9), Is.EqualTo('X'));
                Assert.That(reader.GetDateTime(10), Is.EqualTo(now));
                Assert.That(reader.GetGuid(11), Is.EqualTo(guid));

                Assert.That(reader.GetOrdinal("bool"), Is.EqualTo(1));
                Assert.That(reader["bool"], Is.EqualTo("true"));
                Assert.That(reader[1], Is.EqualTo("true"));

                IWrapper<IDataReader> wrapper = reader;
                Assert.That(wrapper.WrappedItem, Is.SameAs(outer));
            }
        }
    }
}
