using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Security;

namespace Arrow.Settings
{
    /// <summary>
    /// Decrypts settings
    /// </summary>
    public sealed class AesEncryptionSettings : ISettings
    {
        /// <summary>
        /// The setting name is an encrypted string.
        /// The setting value will be the unencryped value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            // The setting name is an encrypted string.
            // The setting value will be the unencryped value

            var algorithm = new AesTextEncryption();
            value = algorithm.Decrypt(name);

            return name is not null;
        }
    }
}
