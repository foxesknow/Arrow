using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Security;

using NUnit.Framework;

namespace UnitTests.Arrow.Security
{
    [TestFixture]
    public class AesTextEncryptionTests
    {
        [Test]
        [TestCase("")]
        [TestCase("A")]
        [TestCase("Hello")]
        [TestCase("This is a long bit of text")]
        public void RoundTrip(string text)
        {
            var encryptor = new AesTextEncryption();
            
            var encryptedText = encryptor.Encrypt(text);
            Assert.That(encryptedText, Is.Not.Null & Has.Length.GreaterThan(0));

            var decryptedText = encryptor.Decrypt(encryptedText);
            Assert.That(decryptedText, Is.Not.Null);

            Assert.That(decryptedText, Is.EqualTo(text));
        }
    }
}
