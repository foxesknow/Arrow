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
        [Test]
        public void GetField_Static()
        {
            var field = MemberLookup.GetField(() => FieldTest.StaticField);
            Assert.That(field, Is.Not.Null);
        }

        [Test]
        public void GetField_Instance()
        {
            var field = MemberLookup.GetField((FieldTest f) => f.InstanceField);
            Assert.That(field, Is.Not.Null);
        }

        [Test]
        public void GetField_Property()
        {
            Assert.Catch(() => MemberLookup.GetField(() => Console.WindowHeight));
        }

        class FieldTest
        {
            public static int StaticField = 0;
            public int InstanceField = 1;
        }
    }
}
