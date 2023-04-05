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
    public class DoorTests
    {
        [Test]
        public void Initialization()
        {
            var door = new Door();
            Assert.That(door.IsOpen, Is.True);
            Assert.That(door.IsClosed, Is.False);
        }

        [Test]
        public void TryClose()
        {
            var door = new Door();
            
            if(door.TryClose(out var releaser))
            {
                Assert.That(releaser, Is.Not.Null);
                Assert.That(door.IsOpen, Is.False);
                Assert.That(door.IsClosed, Is.True);

                using(releaser)
                {
                    Assert.That(door.IsOpen, Is.False);
                    Assert.That(door.IsClosed, Is.True);
                }

                Assert.That(door.IsOpen, Is.True);
                Assert.That(door.IsClosed, Is.False);
            }
            else
            {
                Assert.Fail("Door should have closed");
            }
        }

        [Test]
        public void TryClose_Nested()
        {
            var door = new Door();
            
            if(door.TryClose(out var releaser))
            {
                Assert.That(door.TryClose(out var nestedReleaser), Is.False);
                Assert.That(nestedReleaser, Is.Null);
            }
            else
            {
                Assert.Fail("Door should have closed");
            }
        }
    }
}
