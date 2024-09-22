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
    public class CSharpCodeGeneratorTests
    {
        [Test]
        public void Initialization()
        {
            var generator = new CSharpCodeGenerator();
            Assert.That(generator.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void Block()
        {
            var generator = new CSharpCodeGenerator()
            {
                IndentText = "\t"
            };
            
            using(generator.Block())
            {
                generator.WriteLine("Hello");
            }

            var code = generator.ToString();
            Assert.That(code, Does.Contain("{"));
            Assert.That(code, Does.Contain("}"));
            Assert.That(code, Does.Contain("\tHello"));
        }

        [Test]
        public void BlockWithStatement()
        {
            var generator = new CSharpCodeGenerator()
            {
                IndentText = "\t"
            };
            
            using(generator.Block("if(length == 1)"))
            {
                generator.WriteLine("Debugger.Break();");
            }

            var code = generator.ToString();
            Assert.That(code, Does.Contain("{"));           
        }

        [Test]
        public void SwitchExpression()
        {
            var generator = new CSharpCodeGenerator()
            {
                IndentText = "\t"
            };
            
            using(generator.SwitchExpression("var text = length switch"))
            {
                generator.WriteLines
                (
                    "0 => \"empty\",",
                    "1 => \"single\",",
                    "_ => \"lots\""
                );
            }

            var code = generator.ToString();
            Assert.That(code, Does.Contain("{"));
            Assert.That(code, Does.Contain("};"));
            Assert.That(code, Does.Contain("switch"));
        }
    }
}
