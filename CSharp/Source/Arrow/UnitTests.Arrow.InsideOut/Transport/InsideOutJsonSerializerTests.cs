using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport
{
    [TestFixture]
    public class InsideOutJsonSerializerTests
    {
        [Test]
        public void FlagsAreSupported()
        {
            var data = new TestData()
            {
                Mode = FileShare.Read | FileShare.Write | FileShare.Delete
            };

            var serializer = new InsideOutJsonSerializer();
            var json = serializer.Serialize(data);

            var roundTrip = serializer.Deserialize<TestData>(json);
            Assert.That(roundTrip.Mode, Is.EqualTo(data.Mode));
        }

        private class TestData
        {
            public FileShare Mode{get; set;}
        }
    }    
}
