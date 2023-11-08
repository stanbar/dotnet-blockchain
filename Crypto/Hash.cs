using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    public static class Hash
    {
        public static byte[] Sha256(byte[] input) => SHA256.HashData(input);

        public static string Sha256String(string input)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            Debug.Assert(hash.Length == 32);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}