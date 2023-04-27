using System;

using Arrow.Security;

namespace ArrowEncrypt
{
    internal class Program
    {
        private const string ProviderPrefix = "${aesdecrypt:";
        private const string ProviderSuffix = "}";

        static int Main(string[] args)
        {
            try
            {
                if(args.Length == 2)
                {
                    switch(args[0].ToLower())
                    {
                        case "/d":
                        case "/decrypt":
                        case "-d":
                        case "-decrypt":
                            Decrypt(args[1]);
                            break;

                        case "/e":
                        case "/encrypt":
                        case "-e":
                        case "-encrypt":
                            Encrypt(args[1]);
                            break;

                        default:
                            Console.Error.WriteLine($"Unknown option: {args[0]}");
                            return -1;
                    }
                }
                else
                {
                    Console.Error.WriteLine("Usage:");
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("To encrypt:      ArrowEncrypt /e phrase");
                    Console.Error.WriteLine("To decrypt:      ArrowEncrypt /e phrase");

                    return -1;
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }

        static void Encrypt(string phrase)
        {
            var encryption = new AesTextEncryption();
            var value = encryption.Encrypt(phrase);

            Console.WriteLine(value);
            Console.WriteLine(ProviderPrefix + value + ProviderSuffix);
        }

        static void Decrypt(string phrase)
        {
            var normalizedPhrase = (phrase.StartsWith(ProviderPrefix) && phrase.EndsWith(ProviderSuffix)) switch
            {
                true => phrase[ProviderPrefix.Length..^ProviderSuffix.Length],
                false => phrase
            };

            var encryption = new AesTextEncryption();
            var value = encryption.Decrypt(normalizedPhrase);
            Console.WriteLine(value);
        }
    }
}