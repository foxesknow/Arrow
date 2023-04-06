using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

using NUnit.Framework;

namespace UnitTests.Arrow.Data
{
    [TestFixture]
    public class TypedDataReaderTests
    {
        [Test]
        public void MakeSingleColumn_InvalidArgs()
        {
            Assert.Catch(() => TypedDataReader.MakeSingleColumn((null, typeof(string)), new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleColumn(("", typeof(string)), new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleColumn(("    ", typeof(string)), new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleColumn(("name", null), new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleColumn(("name", typeof(string)), null));
        }

        [Test]
        public void MakeSingleRow_InvalidColumnNames()
        {
            var type = typeof(string);
            Type nullType = null;
            string nullName = null;

            Assert.Catch(() => TypedDataReader.MakeSingleRow(new[]{(nullName, type)}, new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleRow(new[]{("", type)}, new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleRow(new[]{("    ", type)}, new[]{"world"}));
            Assert.Catch(() => TypedDataReader.MakeSingleRow(new[]{("Foo", nullType)}, new[]{"world"}));
        }

        [Test]
        public void NullRow()
        {
            var columns = new[]
            {
                ("Name", typeof(string)),
                ("Age", typeof(int))
            };

            var rows = new[]
            {
                new object[]{"Jack", 33},
                null
            };

            using(var reader = TypedDataReader.Make(columns, rows))
            {
                Assert.That(reader, Is.Not.Null);
                Assert.That(reader.Read(), Is.True);

                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
                Assert.That(reader.GetString(0), Is.EqualTo("Jack"));

                Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(int)));
                Assert.That(reader.GetInt32(1), Is.EqualTo(33));

                Assert.Catch(() => reader.Read());
            }
        }

        [Test]
        public void Read()
        {
            var columns = new[]
            {
                ("Name", typeof(string)),
                ("Age", typeof(int))
            };

            var rows = new[]
            {
                new object[]{"Jack", 33},
                new object[]{"Ben", 40},
            };

            using(var reader = TypedDataReader.Make(columns, rows))
            {
                Assert.That(reader, Is.Not.Null);

                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("Jack"));
                Assert.That(reader.GetInt32(1), Is.EqualTo(33));

                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("Ben"));
                Assert.That(reader.GetInt32(1), Is.EqualTo(40));

                Assert.That(reader.Read(), Is.False);
            }
        }

        [Test]
        public void PopulateDataTable()
        {
            var columns = new[]
            {
                ("Name", typeof(string)),
                ("Age", typeof(int))
            };

            var rows = new[]
            {
                new object[]{"Jack", 33},
                new object[]{"Ben", 40},
            };

            using(var reader = TypedDataReader.Make(columns, rows))
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                Assert.That(dataTable.Columns.Count, Is.EqualTo(reader.FieldCount));
            }
        }
    }
}