using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.ObjectAccess;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using UnitTests.Arrow.Settings;

namespace UnitTests.Arrow.ObjectAccess
{
    [TestFixture]
    public class ObjectLocatorTests
    {
        [Test]
        public void StaticDepthOne()
        {
            Uri uri = new Uri(@"static://Arrow.ObjectAccess.ObjectLocator@Arrow/StaticScheme");
            string text = ObjectLocator.Locate<string>(uri, null);
            Assert.IsNotNull(text);
            Assert.That(text, Is.EqualTo("static"));
        }

        [Test]
        public void StaticDepthTwo()
        {
            Uri uri = new Uri(@"static://Arrow.ObjectAccess.ObjectLocator@Arrow/StaticScheme/Length");
            int length = ObjectLocator.Locate<int>(uri, null);
            Assert.That(length, Is.EqualTo(ObjectLocator.StaticScheme.Length));
        }

        [Test]
        public void InvalidStatic()
        {
            Assert.Throws<UriFormatException>(() =>
            {
                Uri uri = new Uri(@"static://Arrow.ObjectAccess.ObjectLocator@Arrow");
                ObjectLocator.Locate(uri, null);

                // We should never get here
            });
        }

        [Test]
        public void InstanceDepthZero()
        {
            var resolver = MakeResolver();

            Uri uri = new Uri(@"instance://testdata");
            ObjectLocatorTest data = ObjectLocator.Locate<ObjectLocatorTest>(uri, resolver);
            Assert.IsNotNull(data);
            Assert.That(data, Is.TypeOf(typeof(ObjectLocatorTest)));
        }

        [Test]
        public void InstanceDepthOne()
        {
            var resolver = MakeResolver();

            Uri uri = new Uri(@"instance://testdata/mode");
            FileMode mode = ObjectLocator.Locate<FileMode>(uri, resolver);
            Assert.That(mode, Is.EqualTo(FileMode.Open));
        }

        [Test]
        public void InstanceDepthTwo()
        {
            var resolver = MakeResolver();

            Uri uri = new Uri(@"instance://testdata/when/day");
            int day = ObjectLocator.Locate<int>(uri, resolver);
            Assert.That(day, Is.EqualTo(29));
        }

        [Test]
        public void InstanceCallMethod()
        {
            var resolver = MakeResolver();

            Uri uri = new Uri(@"instance://testdata/getLocation/length");
            int length = ObjectLocator.Locate<int>(uri, resolver);
            Assert.That(length, Is.EqualTo(6));
        }

        [Test]
        public void InstanceNull()
        {
            var resolver = MakeResolver();

            // Username is null, which is fine as it's the value for the property
            Uri uri = new Uri(@"instance://testdata/username");
            string username = ObjectLocator.Locate<string>(uri, resolver);
            Assert.IsNull(username);
        }

        [Test]
        public void InstanceNullFail()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var resolver = MakeResolver();

                // Since username is null we can get the length
                Uri uri = new Uri(@"instance://testdata/username/length");
                int length = ObjectLocator.Locate<int>(uri, resolver);
            });
        }

        private Func<string, object> MakeResolver()
        {
            ObjectLocatorTest data = new ObjectLocatorTest();

            return name =>
            {
                return name.ToLower() == "testdata" ? data : null;
            };
        }
    }

    class ObjectLocatorTest
    {
        private DateTime m_When = new DateTime(2008, 11, 29);
        private FileMode m_Mode = FileMode.Open;

        public DateTime When
        {
            get{return m_When;}
        }

        public FileMode Mode
        {
            get{return m_Mode;}
        }

        public string Username
        {
            get{return null;}
        }

        public string GetLocation()
        {
            return "Island";
        }
    }
}
