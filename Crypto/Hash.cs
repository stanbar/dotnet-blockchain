// Implement the Sha256 method in the Hash class. The method should accept a string and return a string. The method should return the SHA256 hash of the input string.

using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace Crypto;
public class Hash
{
    public static byte[] Sha256(byte[] input) =>
     SHA256.HashData(input);
    public static string Sha256String(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        Debug.Assert(hash.Length == 32);
        var hashString = BitConverter.ToString(hash).Replace("-", "");
        return hashString;
    }
}