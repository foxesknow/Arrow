using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.CodeGen;

using NUnit.Framework;

namespace UnitTests.Arrow.CodeGen
{
    [TestFixture]
    public class CodeGeneratorTests
    {
        [Test]
        public void Initialization()
        {
            var generator = new CodeGenerator();
            Assert.That(generator.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void WriteLine()
        {
            var generator = new CodeGenerator();
            generator.WriteLine("Jack");
            
            Assert.That(generator.ToString(), Does.StartWith("Jack"));
        }

        [Test]
        public void WriteLines()
        {
            var generator = new CodeGenerator();
            generator.WriteLines("Jack", "Ben");
            
            Assert.That(generator.ToString(), Does.Contain("Jack"));
            Assert.That(generator.ToString(), Does.Contain("Ben"));
        }

        [Test]
        public void Indent()
        {
            var generator = new CodeGenerator()
            {
                IndentText = "XX"
            };

            using(generator.Indent())
            {
                generator.WriteLine();
            }

            Assert.That(generator.ToString(), Does.StartWith("XX"));
        }
    }
}
