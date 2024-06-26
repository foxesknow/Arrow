﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Calendar;
using Arrow.Calendar.ClockDrivers;

using NUnit.Framework;

namespace UnitTests.Arrow.Calendar
{
    [TestFixture]
    public class ClockTests
    {
        private IClockDriver m_Clock;

        [SetUp]
        public void Init()
        {
            m_Clock = GlobalClockDriverManager.Current();
        }

        [TearDown]
        public void TidyUp()
        {
            GlobalClockDriverManager.Install(m_Clock);
        }

        [Test]
        public void DefaultClock()
        {
            DateTime now1 = DateTime.Now;
            DateTime now2 = Clock.Now;

            Assert.That(now1.Date, Is.EqualTo(now2.Date));
            Assert.That(now2.Kind, Is.EqualTo(DateTimeKind.Local));

            // Make sure we really get a UTC
            Assert.That(Clock.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void FixedClock()
        {
            // We always want the same value back
            DateTime dateTime = new DateTime(1973, 2, 22);
            FixedClockDriver fixedClock = new FixedClockDriver(dateTime);
            GlobalClockDriverManager.Install(fixedClock);

            DateTime now2 = Clock.Now;

            Assert.That(Clock.Now, Is.EqualTo(dateTime));
            Assert.That(Clock.Now.Kind, Is.EqualTo(DateTimeKind.Local));

            // Make sure we really get a UTC
            Assert.That(Clock.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void BadClock()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                GlobalClockDriverManager.Install(null);
            });
        }
    }
}
