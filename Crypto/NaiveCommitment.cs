using System.Security.Cryptography;
using System.Text;

namespace Crypto;
internal class NaiveCommitment
{
    public static byte[] Commit(string[] input)
    {
        using var sha256 = SHA256.Create();

        var totalBytes = new byte[]{};
        for (int i = 0; i < input.Length; i++)
        {
            var m_i = input[i];
            totalBytes = totalBytes.Concat(Encoding.UTF8.GetBytes(m_i)).ToArray();
        }
        var hash = sha256.ComputeHash(totalBytes);
        return hash;
    }

    public static bool Verify (byte[] commitment, int index, string m_i, string[] proof)
    {
        using var sha256 = SHA256.Create();

        var totalBytes = new byte[]{};
        for (int i = 0; i < proof.Length; i++)
        {
            var m = i == index ? m_i : proof[i];
            totalBytes = totalBytes.Concat(Encoding.UTF8.GetBytes(m)).ToArray();
        }
        var hash = sha256.ComputeHash(totalBytes);
        // compare byte arrays
        return hash.AsSpan().SequenceEqual(commitment);
    }
}