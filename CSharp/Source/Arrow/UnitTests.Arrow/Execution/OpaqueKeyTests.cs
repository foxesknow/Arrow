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
    public class OpaqueKeyTests
    {
        [Test]
        public void TestEquality_SameKey()
        {
            var key=new OpaqueKey();

            Assert.That(key.Equals(key),Is.True);
            Assert.That(key.Equals((object)key),Is.True);
            Assert.That(key.Equals(null),Is.False);
            Assert.That(key.CompareTo(key),Is.EqualTo(0));
        }

        [Test]
        public void TestEquality_DifferentKeys()
        {
            var key1=new OpaqueKey();
            var key2=new OpaqueKey();

            Assert.That(key1.Equals(key2),Is.False);
            Assert.That(key1.Equals((object)key2),Is.False);
            Assert.That(key1.CompareTo(key2),Is.Not.EqualTo(0));
        }

        [Test]
        public void TestEquality_CompareKeys()
        {
            var key1=new OpaqueKey();
            var key2=new OpaqueKey();

            int test1=key1.CompareTo(key2);
            int test2=key2.CompareTo(key1);

            // As the keys are not the same the comparison should be 0 (not equal)
            Assert.That(test1,Is.Not.EqualTo(0));
            Assert.That(test2,Is.Not.EqualTo(0));

            // Because we've tested for key1<key2 and key2<key1 we know that
            // the ordering will be less than and greater than. Therefore
            // the results should have different signs
            Assert.That(Math.Sign(test1),Is.Not.EqualTo(Math.Sign(test2)));
        }
    }
}
