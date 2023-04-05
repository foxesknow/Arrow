using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution
{
    [TestFixture]
    public class ReferenceToTests
    {
        [Test]
        public void Initialization_Default()
        {
            var reference = new ReferenceTo<string>();
            Assert.That(reference.Value, Is.Null);
            Assert.That(reference.ToString(), Is.Not.Null);
        }

        [Test]
        public void Initialization_WithValue()
        {
            var reference = new ReferenceTo<string>("Jack");
            Assert.That(reference.Value, Is.EqualTo("Jack"));
            Assert.That(reference.ToString(), Is.Not.Null);
        }

        [Test]
        public async Task SetFromTask()
        {
            var reference = new ReferenceTo<string>("Ben");
            Assert.That(reference.Value, Is.EqualTo("Ben"));

            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                reference.Value = "Kate";
            });

            Assert.That(reference.Value, Is.EqualTo("Kate"));
        }
    }
}
