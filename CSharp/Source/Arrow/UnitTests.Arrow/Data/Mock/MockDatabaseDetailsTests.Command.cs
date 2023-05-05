using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Data.Mock;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.Mock
{
    public partial class MockDatabaseDetailsTests
    {
        [Test]
        public void CommandProperties()
        {
            var details = MakeDetails();

            using(var connection = details.CreateConnection())
            {
                connection.Open();

                using(var command = connection.CreateCommand())
                {
                    Assert.That(command.CommandTimeout, Is.EqualTo(connection.ConnectionTimeout));
                    Assert.That(command.CommandType, Is.EqualTo(CommandType.Text));
                    Assert.That(command.CommandText, Is.EqualTo(""));
                    Assert.That(command.Parameters, Is.Not.Null);
                    Assert.That(command.Transaction, Is.Null);
                    Assert.That(command.UpdatedRowSource, Is.EqualTo(UpdateRowSource.Both));
                    Assert.That(command.Connection, Is.SameAs(connection));
                }
            }
        }
    }
}
