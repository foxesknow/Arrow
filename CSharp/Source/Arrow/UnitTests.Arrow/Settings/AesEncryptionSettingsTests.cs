using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Security;
using Arrow.Settings;

using NUnit.Framework;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class AesEncryptionSettingsTests
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

            var settings = new AesEncryptionSettings();
            Assert.That(settings.TryGetSetting(encryptedText, out var decryptedText), Is.True);
            Assert.That(decryptedText, Is.EqualTo(text));
        }
    }
}
