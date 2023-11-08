namespace Crypto;
public class ProofOfWork
{
    public static (long, string) Mine(string data, int difficulty)
    {
        string targetPrefix = new string('0', difficulty);

        for (long counter = 0; ; counter++)
        {
            string hash = Hash.Sha256String(data + counter);

            if (hash.StartsWith(targetPrefix))
            {
                return (counter, hash);
            }
        }
    }

}