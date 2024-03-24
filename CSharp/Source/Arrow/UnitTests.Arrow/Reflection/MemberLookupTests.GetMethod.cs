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
        public void GetMethod_Static_Action()
        {
            var method = MemberLookup.GetMethod(() => Console.WriteLine((object)null));

            Assert.That(method, Is.Not.Null);
        }

        [TestCase]
        public void GetMethod_Static_Action_Input()
        {
            var method = MemberLookup.GetMethod((List<int> list) => list.Clear());

            Assert.That(method, Is.Not.Null);
        }

        [TestCase]
        public void GetMethod_Static_Function()
        {
            var method = MemberLookup.GetMethod(() => Console.ReadKey(true));

            Assert.That(method, Is.Not.Null);
        }

        [TestCase]
        public void GetMethod_Property()
        {
            Assert.Catch(() => MemberLookup.GetMethod(() => Console.WindowHeight));
        }

        [TestCase]
        public void GetMethod_Instance_Action()
        {
            var method = MemberLookup.GetMethod((List<int> list) => list.Add(1));

            Assert.That(method, Is.Not.Null);
        }

        [TestCase]
        public void GetMethod_Instance_Function()
        {
            var method = MemberLookup.GetMethod((List<int> list) => list.IndexOf(1));

            Assert.That(method, Is.Not.Null);
        }
    }
}
