using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    public static class NaiveCommitment
    {
        // Method to generate a commitment hash for a list of strings.
        public static byte[] Commit(List<string> S)
        {
            // Combine all strings in the list into one long string and encode it to bytes using UTF-8.
            var totalBytes = Encoding.UTF8.GetBytes(string.Join("", S));

            // Generate a SHA256 hash from the combined byte array.
            return SHA256.HashData(totalBytes);
        }

        // Method to verify if a commitment is valid given an index, a value, and the proof list.
        public static bool Verify(byte[] commitment, int index, string m_i, List<string> proof)
        {
            // Initialize a StringBuilder to construct the entire list with the given value inserted.
            var totalBytes = new StringBuilder();

            // Iterate through the proof list and reconstruct the combined string.
            for (int i = 0; i < proof.Count; i++)
            {
                if (i == index)
                {
                    // If the current index matches the given index, use the provided value (m_i).
                    totalBytes.Append(m_i);
                }
                else
                {
                    // Otherwise, use the corresponding element from the proof list.
                    totalBytes.Append(proof[i]);
                }
            }

            // Hash the reconstructed string using SHA256.
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(totalBytes.ToString()));

            // Check if the computed hash matches the original commitment.
            return hashBytes.SequenceEqual(commitment);
        }
    }
}