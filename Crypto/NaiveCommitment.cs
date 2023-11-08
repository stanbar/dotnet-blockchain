using System.Security.Cryptography;
using System.Text;

namespace Crypto;

public class NaiveCommitment
{
    public static byte[] Commit(List<string> S)
    {
        var totalBytes = S.SelectMany(s => Encoding.UTF8.GetBytes(s)).ToArray()!;
        var hashBytes = Hash.Sha256(totalBytes);
        return hashBytes;
    }
    public static bool Verify(byte[] commitment, int index, string m_i, List<string> proof)
    {
        var totalBytes = new byte[] { };
        for (int i = 0; i < proof.Count; i++)
        {
            if (i == index)
            {
                totalBytes = totalBytes.Concat(Encoding.UTF8.GetBytes(m_i)).ToArray();
            }
            else
            {
                totalBytes = totalBytes.Concat(Encoding.UTF8.GetBytes(proof[i])).ToArray();
            }
        }
        var hashBytes = Hash.Sha256(totalBytes);
        return hashBytes.SequenceEqual(commitment);
    }
}