using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace Arrow.Security
{
    /// <summary>
    /// Encrypts text using the AES method
    /// </summary>
    public sealed class AesTextEncryption : TextEncryption
    {        
        private static readonly byte[] s_IV = new byte[]{64,231, 250, 71, 239, 103, 182, 164, 120, 14, 86, 160, 25, 12, 200, 107};
        private static readonly byte[] s_Key = new byte[]
        {
            215, 182, 144, 153, 229, 253, 174, 94, 53, 198, 156, 124, 157, 162, 188, 5,
            59, 40, 210, 199, 148, 62, 162, 176, 159, 73, 31, 35, 244, 147, 2, 123 
        };

        private const string s_Salt = "wmdCyshYARNnpBTLpyhEryTK"; 

        protected override string GetSalt()
        {
            return s_Salt;
        }

        protected override SymmetricAlgorithm MakeAlgorithm()
        {
            var algorithm = Aes.Create();
            algorithm.IV = s_IV;
            algorithm.Key = s_Key;

            return algorithm;
        }
    }
}
