using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace Arrow.Security
{
    /// <summary>
    /// Base class for encrypting and decrypting text
    /// </summary>
    public abstract class TextEncryption
    {        
        /// <summary>
        /// Returns the algorithm used to encrypt the 
        /// </summary>
        /// <returns></returns>
        protected abstract SymmetricAlgorithm MakeAlgorithm();

        /// <summary>
        /// Returns the salt that will be applied to the password
        /// </summary>
        /// <returns></returns>
        protected abstract string GetSalt();

        /// <summary>
        /// Encrypts text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Encrypt(string text)
        {
            if(text is null) throw new ArgumentNullException(nameof(text));

            using(var algorithm = MakeAlgorithm())
            using(var memoryStream = new MemoryStream())
            using(var cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            using(var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
            {
                var salt = GetSalt();
                writer.Write(text + salt);
                writer.Close();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Decrypts text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CryptographicException"></exception>
        public string Decrypt(string text)
        {
            if(text is null) throw new ArgumentNullException(nameof(text));

            var asBytes = Convert.FromBase64String(text);

            using(var algorithm = MakeAlgorithm())
            using(var memoryStream = new MemoryStream(asBytes))
            using(var cryptoStream = new CryptoStream(memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
            using(var reader = new StreamReader(cryptoStream, Encoding.UTF8))
            {
                var unencrypted = reader.ReadToEnd();

                var salt = GetSalt();
                if(unencrypted.Length < salt.Length) throw new CryptographicException("password could not be decrypted");

                return unencrypted.Substring(0, unencrypted.Length - salt.Length);
            }
        }
    }
}
