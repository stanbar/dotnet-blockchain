using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Crypto;
public class Hash
{
    public static byte[] Sha256Bytes(byte[] input) =>
     SHA256.HashData(input);

    public static byte[] Sha256(string input) =>
     Sha256Bytes(Encoding.UTF8.GetBytes(input));

    public static string Sha256String(string input)
    {
        var hash = Sha256(input);
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return hashString;
    }
}