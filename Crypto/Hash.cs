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
            // First, encode the input string into a byte array using UTF-8, then hash it using SHA256.
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            // Debug.Assert is used to ensure that the length of the resulting hash is as expected (32 bytes).
            Debug.Assert(hash.Length == 32);
            // Convert the byte array to a hexadecimal string and remove the hyphens.
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}