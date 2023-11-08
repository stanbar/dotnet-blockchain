using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    public static class NaiveCommitment
    {
        public static byte[] Commit(List<string> S)
        {
            var totalBytes = Encoding.UTF8.GetBytes(string.Join("", S));
            return SHA256.HashData(totalBytes);
        }

        public static bool Verify(byte[] commitment, int index, string m_i, List<string> proof)
        {
            var totalBytes = new StringBuilder();
            for (int i = 0; i < proof.Count; i++)
            {
                if (i == index)
                {
                    totalBytes.Append(m_i);
                }
                else
                {
                    totalBytes.Append(proof[i]);
                }
            }
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(totalBytes.ToString()));
            return hashBytes.SequenceEqual(commitment);
        }
    }
}