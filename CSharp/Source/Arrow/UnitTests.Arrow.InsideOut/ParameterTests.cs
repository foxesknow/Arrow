using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut
{
    [TestFixture]
    public class ParameterTests : TestBase
    {
        [Test]
        public void BoolParameter_Checks()
        {
            Check(new BoolParameter("Enabled"));
            Check(new BoolParameter("Enabled"){Description = "foo"});

            var parameter = new BoolParameter("Enabled");
            Check(parameter);
            
            var typedArgument = parameter.MakeArgument(true);
            Assert.That(typedArgument.Name, Is.EqualTo(parameter.Name));
            Assert.That(typedArgument.Value, Is.True);

            var untypedArgument = parameter.MakeArgumentFromObject("true");
            Assert.That(typedArgument.Name, Is.EqualTo(parameter.Name));
            Assert.That(typedArgument.Value, Is.True);
            Assert.That(typedArgument.AsObject(), Is.True);
        }

        private void Check(Parameter parameter)
        {
            var roundTrip = RoundTrip(parameter);

            Assert.That(parameter.Name, Is.EqualTo(roundTrip.Name));
            Assert.That(parameter.Description, Is.EqualTo(roundTrip.Description));
            Assert.That(parameter.Type(), Is.EqualTo(roundTrip.Type()));
            Assert.That(parameter.DefaultAsObject(), Is.EqualTo(roundTrip.DefaultAsObject()));
        }
    }
}
