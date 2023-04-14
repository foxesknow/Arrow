using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Calendar;

using NUnit.Framework;

namespace UnitTests.Arrow.Calendar
{
    [TestFixture]
    public class ReminderIDTests
    {
        [Test]
        public void None()
        {
            Assert.That(ReminderID.None, Is.EqualTo(ReminderID.None));
            Assert.That(ReminderID.None.CompareTo(ReminderID.None), Is.EqualTo(0));
        }

        [Test]
        public void Allocate()
        {
            var id = ReminderID.Allocate();
            Assert.That(id, Is.Not.EqualTo(ReminderID.None));
        }

        [Test]
        public void Allocate_2()
        {
            var id1 = ReminderID.Allocate();
            var id2 = ReminderID.Allocate();
            
            Assert.That(id1, Is.Not.EqualTo(id2));
            Assert.That(id2, Is.GreaterThan(id1));
        }

        [Test]
        public void CompareTo()
        {
            var id1 = ReminderID.Allocate();
            var id2 = ReminderID.Allocate();
            
            Assert.That(id1.CompareTo(id1), Is.EqualTo(0));
            Assert.That(id1.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1), Is.GreaterThan(0));
        }

        [Test]
        public void ObjectEquality()
        {
            object id1 = ReminderID.Allocate();
            object id2 = ReminderID.Allocate();

            Assert.That(id1.Equals(id1), Is.True);
            Assert.That(id1.Equals(id2), Is.False);
        }

        [Test]
        public void TypedEquality()
        {
            var id1 = ReminderID.Allocate();
            var id2 = ReminderID.Allocate();

            Assert.That(id1.Equals(id1), Is.True);
            Assert.That(id1.Equals(id2), Is.False);
        }

        [Test]
        public void AsString()
        {
            var id1 = ReminderID.Allocate();
            Assert.That(id1.ToString(), Is.Not.Null & Has.Length.GreaterThan(0));
        }
    }
}
