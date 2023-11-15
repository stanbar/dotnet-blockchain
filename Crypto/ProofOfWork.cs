namespace Crypto;
public class ProofOfWork {
    public static long Mine(string data, int difficulty)
    {
        string targetPrefix = new('0', difficulty); // Target prefix of zeros

        var counter = 0;
        while(true)
        {
            string nonce = counter.ToString(); //random.Next().ToString();
            string blockData = data + nonce;
            string hash = Hash.Sha256String(Hash.Sha256String(blockData));

            if (hash.StartsWith(targetPrefix))
            {
                return counter;
            }
            counter++;
        }
    }

}