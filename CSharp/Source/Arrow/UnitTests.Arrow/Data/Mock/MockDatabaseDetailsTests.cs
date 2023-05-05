using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Data.Mock;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.Mock
{
    [TestFixture]
    public partial class MockDatabaseDetailsTests
    {
        [Test]
        public void ExecuteNonQuery()
        {
            var details = MakeDetails()
                          .OnExecuteNonQuery(command => 42);

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "This can be anything";
                    var rowsAffected = command.ExecuteNonQuery();

                    Assert.That(rowsAffected, Is.EqualTo(42));
                }
            }
        }

        [Test]
        public void ExecuteNonQuery_HasConditions()
        {
            var details = MakeDetails()
                          .OnExecuteNonQuery(command => command.CommandText == "Jack", command => 42)
                          .OnExecuteNonQuery(command => command.CommandText == "Ben", command => 58);

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Jack";
                    var rowsAffected = command.ExecuteNonQuery();

                    Assert.That(rowsAffected, Is.EqualTo(42));
                }
            }

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Ben";
                    var rowsAffected = command.ExecuteNonQuery();

                    Assert.That(rowsAffected, Is.EqualTo(58));
                }
            }
        }

        [Test]
        public void ExecuteNonQuery_NoHandler()
        {
            var details = MakeDetails()
                          .OnExecuteNonQuery(command => command.CommandText == "Jack", command => 42)
                          .OnExecuteNonQuery(command => command.CommandText == "Ben", command => 58);

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Sawyer";
                    Assert.Catch(() => command.ExecuteNonQuery());
                }
            }
        }

        [Test]
        public void ExecuteReader()
        {
            var details = MakeDetails()
                          .OnExecuteReader(command => UntypedDataReader.MakeSingleColumn("Name", new[]{"Jack", "Ben"}));

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "This can be anything";
                    using(var reader = command.ExecuteReader())
                    {
                        Assert.That(reader, Is.Not.Null);
                    }
                }
            }
        }

        [Test]
        public void ExecuteReader_HasConditions()
        {
            var details = MakeDetails()
                          .OnExecuteReader
                          (
                            command => command.CommandText == "Jack",
                            command => UntypedDataReader.MakeSingleColumn("Name", new[]{"Sawyer"})
                          )
                          .OnExecuteReader
                          (
                            command => command.CommandText == "Ben",
                            command => UntypedDataReader.MakeSingleColumn("Name", new[]{"Juliet"})
                          );

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Jack";
                    using(var reader = command.ExecuteReader())
                    {
                        Assert.That(reader, Is.Not.Null);
                        reader.Read();
                        Assert.That(reader.GetString(0), Is.EqualTo("Sawyer"));
                    }
                }
            }

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Ben";
                    using(var reader = command.ExecuteReader())
                    {
                        Assert.That(reader, Is.Not.Null);
                        reader.Read();
                        Assert.That(reader.GetString(0), Is.EqualTo("Juliet"));
                    }
                }
            }
        }

        [Test]
        public void ExecuteReader_NoHandler()
        {
            var details = MakeDetails()
                          .OnExecuteReader
                          (
                            command => command.CommandText == "Jack",
                            command => UntypedDataReader.MakeSingleColumn("Name", new[]{"Sawyer"})
                          )
                          .OnExecuteReader
                          (
                            command => command.CommandText == "Ben",
                            command => UntypedDataReader.MakeSingleColumn("Name", new[]{"Juliet"})
                          );

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Hurley";
                    Assert.Catch(() => command.ExecuteReader());
                }
            }
        }

        [Test]
        public void ExecuteScalar()
        {
            var details = MakeDetails()
                          .OnExecuteScalar(command => "Hello");

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "This can be anything";
                    var scalar = command.ExecuteScalar();

                    Assert.That(scalar, Is.EqualTo("Hello"));
                }
            }
        }

        [Test]
        public void ExecuteScalar_HasConditions()
        {
            var details = MakeDetails()
                          .OnExecuteScalar(command => command.CommandText == "Jack", command => "Hello")
                          .OnExecuteScalar(command => command.CommandText == "Ben", command => "Goodbye");

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Jack";
                    var scalar = command.ExecuteScalar();

                    Assert.That(scalar, Is.EqualTo("Hello"));
                }
            }

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Ben";
                    var scalar = command.ExecuteScalar();

                    Assert.That(scalar, Is.EqualTo("Goodbye"));
                }
            }
        }

        [Test]
        public void ExecuteScalar_NoHandler()
        {
            var details = MakeDetails()
                          .OnExecuteScalar(command => command.CommandText == "Jack", command => "Hello")
                          .OnExecuteScalar(command => command.CommandText == "Ben", command => "Goodbye");

            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "Sawyer";
                    Assert.Catch(() => command.ExecuteScalar());
                }
            }
        }

        private MockDatabaseDetails MakeDetails()
        {
            return new MockDatabaseDetails()
            {
                ConnectionString = "DataSource=TheIsland"
            };
        }
    }
}
