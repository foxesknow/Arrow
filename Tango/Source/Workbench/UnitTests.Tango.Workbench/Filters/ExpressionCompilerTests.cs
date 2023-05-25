using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tango.Workbench.Data;

using NUnit.Framework;
using Tango.Workbench.Filters;
using Arrow.Logging.Loggers;
using System.Linq.Expressions;

namespace UnitTests.Tango.Workbench.Filters
{
    [TestFixture]
    public class ExpressionCompilerTests
    {
        [Test]
        [TestCase("1 + 1", 2)]
        [TestCase("1 + item.Length", 6)]
        [TestCase("@ 1 + 1", 2)]
        [TestCase("@ 1 + item.Length", 6)]
        public void Add(string expression, int expectedValue)
        {
            var compiler = new ExpressionCompiler<int>();
            var function = compiler.GetFunction(expression, typeof(string), NullLog.Instance);
            Assert.That(function("hello", 0), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("item.age", 42)]
        [TestCase("item.Age", 42)]
        [TestCase("Item.Age", 42)]
        public void ExpandoObjectSupport(string expression, int expectedValue)
        {
            // Anything that implements ISupportDynamic causes the
            // script to be compiled as a dynamic expression
            var so = new StructuredObject()
            {
                {"Name", "Jack"},
                {"Age", 42}
            };

            var compiler = new ExpressionCompiler<int>();
            var function = compiler.GetFunction(expression, so.GetType(), NullLog.Instance);
            Assert.That(function(so, 0), Is.EqualTo(expectedValue));
        }
    }
}
