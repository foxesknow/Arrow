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
        public void GetConstructor()
        {
            var constructor = MemberLookup.GetConstructor(() => new List<int>());
            Assert.That(constructor, Is.Not.Null);
        }

        [Test]
        public void GetConstructor_NotNew()
        {
            Assert.Catch(() => MemberLookup.GetConstructor(() => "hello"));
        }
    }
}
