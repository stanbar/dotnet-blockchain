namespace Crypto;
public class ProofOfWork
{
    // Static method that performs mining to find a hash starting with a specific number of leading zeros.
    public static (long, string) Mine(string data, int difficulty)
    {
        // Define the target prefix - a string of 'difficulty' number of zeros.
        string targetPrefix = new string('0', difficulty);

        // Incrementally search for a hash that matches the target prefix.
        for (long counter = 0; ; counter++)  // Infinite loop that increments the counter.
        {
            // Concatenate the input data with the counter value and compute the SHA256 hash.
            string hash = Hash.Sha256String(data + counter);

            // Check if the hash starts with the required number of leading zeros (i.e., matches the target).
            if (hash.StartsWith(targetPrefix))
            {
                // If a valid hash is found, return the counter value and the resulting hash.
                return (counter, hash);
            }
        }
    }
}