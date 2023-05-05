using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.Mock
{
    public partial class MockDatabaseDetailsTests
    {
        [Test]
        public void Parameter()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                connection.Open();

                using(var command = connection.CreateCommand())
                {
                    var jack = command.CreateParameter();

                    Assert.That(jack, Is.Not.Null);
                    Assert.That(jack.ParameterName, Is.EqualTo(""));
                    Assert.That(jack.SourceColumn, Is.EqualTo(""));
                    Assert.That(jack.DbType, Is.EqualTo(DbType.String));
                    Assert.That(jack.Direction, Is.EqualTo(ParameterDirection.Input));
                    Assert.That(jack.IsNullable, Is.False);
                    Assert.That(jack.Size, Is.EqualTo(0));
                    Assert.That(jack.Precision, Is.EqualTo(0));
                    Assert.That(jack.Value, Is.Null);
                }
            }
        }

        [Test]
        public void ParametersCollection_Properties()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                connection.Open();

                using(var command = connection.CreateCommand())
                {
                    Assert.That(command.Parameters.Count, Is.EqualTo(0));
                    Assert.That(command.Parameters.SyncRoot, Is.Not.Null);
                    Assert.That(command.Parameters.IsReadOnly, Is.False);
                }
            }
        }

        [Test]
        public void ParametersCollection_Methods()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                connection.Open();

                using(var command = connection.CreateCommand())
                {
                    var jack = command.CreateParameter(DbType.String, "Name", "Jack");
                    Assert.That(command.Parameters.Count, Is.EqualTo(0));

                    var jackIndex = command.Parameters.Add(jack);
                    Assert.That(command.Parameters.Count, Is.EqualTo(1));
                    Assert.That(jackIndex, Is.EqualTo(0));
                    Assert.That(command.Parameters.Count, Is.EqualTo(1));
                    Assert.That(command.Parameters.Contains("Name"), Is.True);
                    Assert.That(command.Parameters.Contains(jack), Is.True);

                    var age = command.CreateParameter(DbType.Int32, "Age", 58);
                    var ageIndex = command.Parameters.Add(age);
                    Assert.That(command.Parameters.Count, Is.EqualTo(2));
                    Assert.That(ageIndex, Is.EqualTo(1));
                    Assert.That(command.Parameters.IndexOf("Age"), Is.EqualTo(1));

                    command.Parameters.Clear();
                    Assert.That(command.Parameters.Count, Is.EqualTo(0));
                }
            }
        }
    }
}
