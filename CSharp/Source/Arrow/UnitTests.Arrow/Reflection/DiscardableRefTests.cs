using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Reflection;
using NUnit.Framework;

namespace UnitTests.Arrow.Reflection
{
    public class DiscardableRefTests
    {
        [Test]
        public void MethodTakesAReference()
        {
            Assert.DoesNotThrow(() => MemberLookup.GetMethod(() => Monitor.Enter(null!, ref DiscardableRef<bool>.Value)));
        }
    }
}
