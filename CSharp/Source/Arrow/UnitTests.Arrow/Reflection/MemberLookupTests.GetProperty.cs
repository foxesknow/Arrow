using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Arrow.Reflection;

using NUnit.Framework;

namespace UnitTests.Arrow.Reflection
{
    public partial class MemberLookupTests
    {
        [TestCase]
        public void GetProperty_Static()
        {
            var property = MemberLookup.GetProperty(() => Console.WindowHeight);
            Assert.That(property, Is.Not.Null);
        }

        [TestCase]
        public void GetProperty_Instance()
        {
            var property = MemberLookup.GetProperty((List<int> list) => list.Count);
            Assert.That(property, Is.Not.Null);
        }

        [TestCase]
        public void GetProperty_Method()
        {
            Assert.Catch(() => MemberLookup.GetProperty(() => Console.ReadLine()));
        }
    }
}
