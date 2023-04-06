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

        [Test]
        public void Make_InvalidColumns()
        {
            var row = new[]
            {
                new[]{"hello", "world"}
            };

            Assert.Catch(() => UntypedDataReader.Make(null, row));
            Assert.Catch(() => UntypedDataReader.Make(new string[]{null, null}, row));
            Assert.Catch(() => UntypedDataReader.Make(new string[]{null, "where"}, row));
            Assert.Catch(() => UntypedDataReader.Make(new string[]{null, "   "}, row));
        }

        [Test]
        public void NullRowInSequence()
        {
            var row = new[]
            {
                new[]{"hello", "world"},
                null
            };

            var reader = UntypedDataReader.Make(new string[]{"A", "B"}, row);
            Assert.That(reader, Is.Not.Null);

            using(reader)
            {
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("hello"));
                Assert.That(reader.GetString(1), Is.EqualTo("world"));

                // The null row will cause an exception
                Assert.Catch(() => reader.Read());
            }
        }

        [Test]
        public void WrongRowLength()
        {
            var row = new[]
            {
                new[]{"hello", "world"},
                new[]{"hi"}
            };

            var reader = UntypedDataReader.Make(new string[]{"A", "B"}, row);
            Assert.That(reader, Is.Not.Null);

            using(reader)
            {
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("hello"));
                Assert.That(reader.GetString(1), Is.EqualTo("world"));

                // The null row will cause an exception
                Assert.Catch(() => reader.Read());
            }
        }

        [Test]
        public void SingleRow_AllStrings()
        {
            var columns = new[]{"name", "bool", "byte", "int16", "int32", "int64", "single", "double", "decimal", "char"};
            var row = new[]{"Row1", "true", "1", "2", "4", "8", "10.0", "20.0", "40", "X"};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader, Is.Not.Null);
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetSchemaTable(), Is.Null);

                Assert.That(reader.GetString(0), Is.EqualTo("Row1"));
                Assert.That(reader.GetBoolean(1), Is.EqualTo(true));
                Assert.That(reader.GetByte(2), Is.EqualTo((byte)1));
                Assert.That(reader.GetInt16(3), Is.EqualTo((short)2));
                Assert.That(reader.GetInt32(4), Is.EqualTo((int)4));
                Assert.That(reader.GetInt64(5), Is.EqualTo((long)8));
                Assert.That(reader.GetFloat(6), Is.EqualTo((float)10.0));
                Assert.That(reader.GetDouble(7), Is.EqualTo((double)20.0));
                Assert.That(reader.GetDecimal(8), Is.EqualTo((decimal)40));
                Assert.That(reader.GetChar(9), Is.EqualTo('X'));

                IDataRecord record = reader;
                Assert.That(record.FieldCount, Is.EqualTo(10));
                Assert.That(reader.GetName(0), Is.EqualTo("name"));
                Assert.That(reader.GetOrdinal("byte"), Is.EqualTo(2));

                Assert.That(record[0], Is.EqualTo("Row1"));
                Assert.That(record[1], Is.EqualTo("true"));

                var values = new object[2];
                Assert.That(reader.GetValues(values), Is.EqualTo(2));
                Assert.That(values[0], Is.EqualTo("Row1"));
                Assert.That(values[1], Is.EqualTo("true"));

                Assert.That(reader.Read(), Is.False);
            }
        }

        [Test]
        public void SingleRow_MixedTypes()
        {
            var now = DateTime.Now;
            var guid = Guid.NewGuid();

            var columns = new[]{"name", "bool", "byte", "int16", "int32", "int64", "single", "double", "decimal", "char", "now", "guid"};
            var row = new object[]{"Row1", true, 1, 2, 4, 8, 10.0, 20.0, 40, 'X', now, guid};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader, Is.Not.Null);
                Assert.That(reader.Read(), Is.True);

                Assert.That(reader.GetString(0), Is.EqualTo("Row1"));
                Assert.That(reader.GetBoolean(1), Is.EqualTo(true));
                Assert.That(reader.GetByte(2), Is.EqualTo((byte)1));
                Assert.That(reader.GetInt16(3), Is.EqualTo((short)2));
                Assert.That(reader.GetInt32(4), Is.EqualTo((int)4));
                Assert.That(reader.GetInt64(5), Is.EqualTo((long)8));
                Assert.That(reader.GetFloat(6), Is.EqualTo((float)10.0));
                Assert.That(reader.GetDouble(7), Is.EqualTo((double)20.0));
                Assert.That(reader.GetDecimal(8), Is.EqualTo((decimal)40));
                Assert.That(reader.GetChar(9), Is.EqualTo('X'));
                Assert.That(reader.GetDateTime(10), Is.EqualTo(now));
                Assert.That(reader.GetGuid(11), Is.EqualTo(guid));
            }
        }

        [Test]
        public void SingleRow_Nulls()
        {
            var now = DateTime.Now;
            var guid = Guid.NewGuid();

            var columns = new[]{"name", "bool", "byte", "int16", "int32", "int64", "single", "double", "decimal", "char", "now", "guid"};
            var row = new object[]{"Row1", true, null, 2, 4, null, 10.0, 20.0, 40, 'X', now, guid};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader, Is.Not.Null);
                Assert.That(reader.Read(), Is.True);

                Assert.That(reader.GetString(0), Is.EqualTo("Row1"));
                Assert.That(reader.GetBoolean(1), Is.EqualTo(true));
                Assert.That(reader.IsDBNull(2), Is.True);
                Assert.That(reader.GetInt16(3), Is.EqualTo((short)2));
                Assert.That(reader.GetInt32(4), Is.EqualTo((int)4));
                Assert.That(reader.IsDBNull(5), Is.True);
                Assert.That(reader.GetFloat(6), Is.EqualTo((float)10.0));
                Assert.That(reader.GetDouble(7), Is.EqualTo((double)20.0));
                Assert.That(reader.GetDecimal(8), Is.EqualTo((decimal)40));
                Assert.That(reader.GetChar(9), Is.EqualTo('X'));
                Assert.That(reader.GetDateTime(10), Is.EqualTo(now));
                Assert.That(reader.GetGuid(11), Is.EqualTo(guid));
            }
        }

        [Test]
        public void MultipleRows()
        {
            var columns = new[]{"Name", "Age"};
            var rows = new[]
            {
                new[]{"Jack", "36"},
                new[]{"Ben", "41"},
                new[]{"Sawyer", "33"}
            };

            var reader = UntypedDataReader.Make(columns, rows);
            using(reader)
            {
                Assert.That(reader, Is.Not.Null);

                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("Jack"));
                Assert.That(reader.GetInt32(1), Is.EqualTo(36));

                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("Ben"));
                Assert.That(reader.GetInt32(1), Is.EqualTo(41));

                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetString(0), Is.EqualTo("Sawyer"));
                Assert.That(reader.GetInt32(1), Is.EqualTo(33));
                
                Assert.That(reader.Read(), Is.False);
            }

            Assert.That(reader.IsClosed, Is.True);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        [TestCase(10)]
        public void BadIndex(int index)
        {
            var columns = new[]{"Name", "Age"};
            var rows = new[]
            {
                new[]{"Jack", "36"},
                new[]{"Ben", "41"},
                new[]{"Sawyer", "33"}
            };

            using(var reader = UntypedDataReader.Make(columns, rows))
            {
                Assert.That(reader, Is.Not.Null);

                Assert.That(reader.Read(), Is.True);
                Assert.Catch(() => reader.GetString(index));
            }
        }

        [Test]
        public void ColumnsWithSameName()
        {
            var columns = new[]{"Name", "Age", "name"};
            var rows = new[]
            {
                new[]{"Jack", "36", "a"},
                new[]{"Ben", "41", "b"},
                new[]{"Sawyer", "33", "c"}
            };

            using(var reader = UntypedDataReader.Make(columns, rows))
            {
                Assert.That(reader, Is.Not.Null);
                
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader["Name"], Is.EqualTo("Jack"));
                Assert.That(reader["Age"], Is.EqualTo("36"));
                Assert.That(reader["name"], Is.EqualTo("Jack"));

                Assert.That(reader[0], Is.EqualTo("Jack"));
                Assert.That(reader[1], Is.EqualTo("36"));
                Assert.That(reader[2], Is.EqualTo("a"));

                Assert.That(reader.GetString(0), Is.EqualTo("Jack"));
                Assert.That(reader.GetString(1), Is.EqualTo("36"));
                Assert.That(reader.GetString(2), Is.EqualTo("a"));
            }
        }

        [Test]
        public void ByteArray_FullRead()
        {
            var columns = new[]{"buffer"};
            var row = new object[]{new byte[]{5, 10, 15, 20, 25}};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader.Read(), Is.True);
                
                var bufferLength = reader.GetBytes(0, 0, null, 0, 0);
                Assert.That(bufferLength, Is.EqualTo(5));

                var buffer = new byte[bufferLength];
                Assert.That(reader.GetBytes(0, 0, buffer, 0, (int)bufferLength), Is.EqualTo(5));
                Assert.That(buffer[0], Is.EqualTo(5));
                Assert.That(buffer[1], Is.EqualTo(10));
                Assert.That(buffer[2], Is.EqualTo(15));
                Assert.That(buffer[3], Is.EqualTo(20));
                Assert.That(buffer[4], Is.EqualTo(25));
            }
        }

        [Test]
        public void ByteArray_PartialRead()
        {
            var columns = new[]{"buffer"};
            var row = new object[]{new byte[]{5, 10, 15, 20, 25}};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader.Read(), Is.True);
                
                var bufferLength = reader.GetBytes(0, 0, null, 0, 0);
                Assert.That(bufferLength, Is.EqualTo(5));

                var buffer = new byte[bufferLength];
                Assert.That(reader.GetBytes(0, 2, buffer, 0, (int)bufferLength), Is.EqualTo(3));
                Assert.That(buffer[0], Is.EqualTo(15));
                Assert.That(buffer[1], Is.EqualTo(20));
                Assert.That(buffer[2], Is.EqualTo(25));
            }
        }

        [Test]
        public void CharArray_FullRead()
        {
            var columns = new[]{"buffer"};
            var row = new object[]{new char[]{'a', 'b', 'c', 'd', 'e'}};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader.Read(), Is.True);
                
                var bufferLength = reader.GetChars(0, 0, null, 0, 0);
                Assert.That(bufferLength, Is.EqualTo(5));

                var buffer = new char[bufferLength];
                Assert.That(reader.GetChars(0, 0, buffer, 0, (int)bufferLength), Is.EqualTo(5));
                Assert.That(buffer[0], Is.EqualTo('a'));
                Assert.That(buffer[1], Is.EqualTo('b'));
                Assert.That(buffer[2], Is.EqualTo('c'));
                Assert.That(buffer[3], Is.EqualTo('d'));
                Assert.That(buffer[4], Is.EqualTo('e'));
            }
        }

        [Test]
        public void CharArray_PartialRead()
        {
            var columns = new[]{"buffer"};
            var row = new object[]{new char[]{'a', 'b', 'c', 'd', 'e'}};

            using(var reader = UntypedDataReader.MakeSingleRow(columns, row))
            {
                Assert.That(reader.Read(), Is.True);
                
                var bufferLength = reader.GetChars(0, 0, null, 0, 0);
                Assert.That(bufferLength, Is.EqualTo(5));

                var buffer = new char[bufferLength];
                Assert.That(reader.GetChars(0, 2, buffer, 0, (int)bufferLength), Is.EqualTo(3));
                Assert.That(buffer[0], Is.EqualTo('c'));
                Assert.That(buffer[1], Is.EqualTo('d'));
                Assert.That(buffer[2], Is.EqualTo('e'));
            }
        }
    }
}
