using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Crypto;
public class Hash
{
    public static byte[] Sha256(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return hash;
    }

    public static string Sha256String(string input)
    {
        var hash = Sha256(input);
        Debug.Assert(hash.Length == 32);
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return hashString;
    }
}